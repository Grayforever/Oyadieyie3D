using Android.App;
using Android.Content;
using Android.Database;
using Android.Gms.Location;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.Core.View;
using AndroidX.Core.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using BumpTech.GlideLib;
using BumpTech.GlideLib.Requests;
using CN.Pedant.SweetAlert;
using Com.Yalantis.Ucrop;
using DE.Hdodenhof.CircleImageViewLib;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using Google.Android.Material.AppBar;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Java.IO;
using Java.Util;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Adapters;
using Oyadieyie3D.Callbacks;
using Oyadieyie3D.Events;
using Oyadieyie3D.HelperClasses;
using Oyadieyie3D.Models;
using Oyadieyie3D.Parcelables;
using Oyadieyie3D.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static AndroidX.Core.Content.FileProvider;
using ActionMode = Android.Views.ActionMode;
using SearchView = AndroidX.AppCompat.Widget.SearchView;
using Task = Android.Gms.Tasks.Task;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace Oyadieyie3D
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class MainActivity : AppCompatActivity
    {
        
        private RecyclerView mainRecycler;
        private ConstraintLayout emptyRoot;
        private SwipeRefreshLayout swipe_container;
        private FloatingActionButton addPostFab;
        private PostAdapter postAdapter;
        private RecyclerViewEmptyObserver emptyObserver;
        private User user { get; set; }
        private List<Post> posts;
        PostEventListener postEventListener = new PostEventListener();
        private ProfileParcelable profileParcelable = new ProfileParcelable();
        private SweetAlertDialog loaderDialog;
        private ActionModeCallback actionModeCallback;
        private ActionMode actionMode;


        private const int UCROP_REQUEST = 69;
        private const string HAS_IMAGE_KEY = "has_image";
        private static int SELECT_PICTURE = 1;
        private ImageView postImageView;
        private CircleImageView profile_image_view;
        private TextInputLayout commentEt;
        private MaterialButton doneBtn;
        public int REQUEST_IMAGE_CAPTURE = 500;
        private bool lockAspectRatio = false, setBitmapMaxWidthHeight = false;
        private int ASPECT_RATIO_X = 16, ASPECT_RATIO_Y = 9, bitmapMaxWidth = 1000, bitmapMaxHeight = 1000;
        private int IMAGE_COMPRESSION = 80;
        public static string fileName;
        private string profile_url;
        private ImageCaptureUtils icu;
        private bool hasImage = false;
        private ProgressBar postProgress;
        private static StorageReference imageRef;
        private ImageButton closeBtn;
        private Uri uri;
        private RequestOptions requestOptions;

        private BottomSheetBehavior postBottomsheetBehavior;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            mainRecycler = FindViewById<RecyclerView>(Resource.Id.main_recycler);
            emptyRoot = FindViewById<ConstraintLayout>(Resource.Id.rv_empty_view);
            swipe_container = FindViewById<SwipeRefreshLayout>(Resource.Id.main_refresher);
            addPostFab = FindViewById<FloatingActionButton>(Resource.Id.post_fab);
            var appbar = FindViewById<AppBarLayout>(Resource.Id.activity_main_appbar);
            var toolbar = appbar.FindViewById<Toolbar>(Resource.Id.main_toolbar);
            SetSupportActionBar(toolbar);
            addPostFab.Click += AddPostFab_Click;
            mainRecycler.AddOnScrollListener(new OnscrollListener(
            onScrolled: (r, dx, dy) =>
            {
                if (dy > 0)
                {
                    addPostFab.Hide();
                }
                else if (dy < 0)
                {
                    addPostFab.Show();
                }
            }));

            DividerItemDecoration decoration = new DividerItemDecoration(mainRecycler.Context, DividerItemDecoration.Vertical);
            mainRecycler.AddItemDecoration(decoration);
            
           

            profileParcelable.WriteTOParcelFailed += ProfileParcelable_WriteTOParcelFailed;
            await GetUserFromFireAsync();
            postEventListener.FetchPost();
            postEventListener.OnPostRetrieved += PostEventListener_OnPostRetrieved;

            

            RecyclerView.ItemAnimator itemAnimator = new DefaultItemAnimator();
            itemAnimator.AddDuration = 1000;
            itemAnimator.RemoveDuration = 1000;
            mainRecycler.SetItemAnimator(itemAnimator);

            CreateBottomSheet();
        }

        

        private void PostEventListener_OnPostRetrieved(object sender, PostEventListener.PostEventArgs e)
        {
            if (e.Posts == null)
                return;

            posts = new List<Post>();
            posts = e.Posts.OrderByDescending(p => p.PostDate).ToList();
            
            SetUpRecycler();
        }

        private void SetUpActionMode()
        {
            actionModeCallback = new ActionModeCallback(
                onActionItemClicked: (mode, item)=> 
                {
                    switch (item.ItemId) 
                    {
                        case Resource.Id.action_help:
                            mode.Finish();
                            break;
                        default:
                            break;
                    }

                }, onCreateActionMode: (mode, menu)=> 
                {
                    MenuInflater inflater = mode.MenuInflater;
                    inflater.Inflate(Resource.Menu.help_menu, menu);

                }, onDestroyActionMode: (mode)=> 
                {
                    mode = null;
                });
        }

        private void ProfileParcelable_WriteTOParcelFailed(object sender, EventArgs e) => ShowToast("No network connection");

        private void AddPostFab_Click(object sender, EventArgs e)
        {
            addPostFab.Post(() =>
            {
                try
                {
                    postBottomsheetBehavior.State = BottomSheetBehavior.StateExpanded;
                }
                catch (Exception)
                {
                    ShowToast("No network connection");
                }
            });
        }

        private void SetUpRecycler()
        {
            postAdapter = new PostAdapter(posts); 
            mainRecycler.SetAdapter(postAdapter);
            emptyObserver = new RecyclerViewEmptyObserver(mainRecycler, emptyRoot);
            postAdapter.RegisterAdapterDataObserver(emptyObserver);
            

            postAdapter.ItemLongClick += (s1, e1) =>
            {
                string postID = posts[e1.Position].ID;
                string ownerID = posts[e1.Position].OwnerId;

                if (SessionManager.GetFirebaseAuth().CurrentUser.Uid != ownerID)
                    return;

                var sweetDialog = new SweetAlertDialog(this, SweetAlertDialog.WarningType);
                sweetDialog.SetTitleText("Post Options");
                sweetDialog.SetContentText("Do you want to edit or delete selected post?");
                sweetDialog.SetCancelText("Delete");
                sweetDialog.SetConfirmText("Edit");
                sweetDialog.SetConfirmClickListener(new SweetConfirmClick(
                onClick: d1 =>
                {
                    d1.ChangeAlertType(SweetAlertDialog.SuccessType);
                    d1.SetTitleText("Done");
                    d1.SetContentText("");
                    d1.ShowCancelButton(false);
                    d1.SetConfirmText("OK");
                    d1.SetConfirmClickListener(null);
                    d1.Show();
                }));
                sweetDialog.SetCancelClickListener(new SweetConfirmClick(
                onClick: d2 =>
                {
                    SessionManager.GetFireDB().GetReference("posts").Child(postID).RemoveValue()
                        .AddOnCompleteListener(new OncompleteListener((onComplete)=>
                        {
                            switch (onComplete.IsSuccessful)
                            {
                                case false:
                                    break;
                                default:
                                    StorageReference storageReference = FirebaseStorage.Instance.GetReference("postImages/" + postID);
                                    storageReference.DeleteAsync();
                                    postAdapter.NotifyItemRemoved(e1.Position);
                                    postAdapter.NotifyItemRangeChanged(e1.Position, posts.Count);
                                    break;
                            }
                        }));
                    d2.ChangeAlertType(SweetAlertDialog.SuccessType);
                    d2.SetTitleText("Deleted");
                    d2.SetContentText("");
                    d2.ShowCancelButton(false);
                    d2.SetConfirmText("OK");
                    d2.SetConfirmClickListener(null);
                    d2.Show();
                }));
                sweetDialog.Show();
            }; 

            postAdapter.ItemClick += (s2, e2) =>
            {
                var intent = new Intent(this, typeof(FullscreenImageActivity));

                PostParcelable postParcelable = new PostParcelable();
                postParcelable.PostItem = posts[e2.Position];

                intent.PutExtra(Constants.TRANSITION_NAME, ViewCompat.GetTransitionName(e2.ImageView));
                intent.PutExtra(Constants.POST_DATA_EXTRA, postParcelable);
                intent.PutExtra(Constants.PARCEL_TYPE, 0);
                var options = ActivityOptionsCompat.MakeSceneTransitionAnimation(this, e2.ImageView,
                    ViewCompat.GetTransitionName(e2.ImageView));
                StartActivity(intent, options.ToBundle());
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            var searchView = menu.FindItem(Resource.Id.action_search).ActionView as SearchView;
            if(searchView != null)
            {
                searchView.QueryTextChange += SearchView_QueryTextChange;
            }
            return base.OnCreateOptionsMenu(menu);
        }

        private void SearchView_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            //try
            //{
            //    List<Post> searchResult =
            //            (from post in posts
            //             where post.PostBody.ToLower().Contains(e.NewText.ToLower())
            //             select post).ToList();

            //    postAdapter = new PostAdapter(this, searchResult);
            //    postAdapter.RegisterAdapterDataObserver(emptyObserver);
            //    mainRecycler.SetAdapter(postAdapter);
            //}
            //catch (Exception)
            //{
            //    Toast.MakeText(this, "No data yet", ToastLength.Short).Show();
            //}
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_refresh:

                    break;
                    
                case Resource.Id.action_finder:
                    
                    break;

                case Resource.Id.action_settings:
                    var intent = new Intent(this, typeof(SettingsActivity));
                    profileParcelable.UserProfile = user;
                    intent.PutExtra(Constants.PROFILE_DATA_EXTRA, profileParcelable);
                    StartActivity(intent);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if (resultCode == Result.Ok)
                {
                    if (requestCode == SELECT_PICTURE)
                    {
                        Uri selectedImageURI = data.Data;
                        CropImage(selectedImageURI);
                    }
                    else if (requestCode == REQUEST_IMAGE_CAPTURE)
                    {
                        CropImage(GetCacheImagePath(fileName));
                    }
                    else if (requestCode == UCrop.RequestCrop)
                    {
                        uri = UCrop.GetOutput(data);
                        Glide.With(this).Load(uri).Into(postImageView);
                        hasImage = true;
                        doneBtn.Enabled = commentEt.EditText.Text.Length >= 6 && hasImage;
                    }
                    else if (requestCode == UCrop.ResultError)
                    {
                        throw UCrop.GetError(data);
                    }
                }
            }
            catch (System.Exception)
            {

            }
        }

        public override void OnBackPressed()
        {
            switch (postBottomsheetBehavior.State)
            {
                case BottomSheetBehavior.StateExpanded:
                case BottomSheetBehavior.StateHalfExpanded:
                    postBottomsheetBehavior.State = BottomSheetBehavior.StateHidden;
                    break;
                default:
                    base.OnBackPressed();
                    break;
            }
        }

        public async System.Threading.Tasks.Task<User> GetUserFromFireAsync()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    string userId = SessionManager.GetFirebaseAuth().CurrentUser.Uid;
                    var userRef = SessionManager.GetFireDB().GetReference($"users/{userId}/profile");
                    userRef.AddValueEventListener(new SingleValueListener(
                    onDataChange: (snapshot) =>
                    {
                        if (!snapshot.Exists())
                            return;

                        var _user = new User
                        {
                            ID = snapshot.Key,
                            Username = snapshot.Child(Constants.SNAPSHOT_FNAME) != null ? snapshot.Child(Constants.SNAPSHOT_FNAME).Value.ToString() : "",
                            Status = snapshot.Child(Constants.SNAPSHOT_GENDER) != null ? snapshot.Child(Constants.SNAPSHOT_GENDER).Value.ToString() : "",
                            ProfileImgUrl = snapshot.Child(Constants.SNAPSHOT_PHOTO_URL) != null ? snapshot.Child(Constants.SNAPSHOT_PHOTO_URL).Value.ToString() : "",
                            Email = snapshot.Child(Constants.SNAPSHOT_EMAIL) != null ? snapshot.Child(Constants.SNAPSHOT_EMAIL).Value.ToString() : "",
                            Phone = snapshot.Child(Constants.SNAPSHOT_PHONE) != null ? snapshot.Child(Constants.SNAPSHOT_PHONE).Value.ToString() : ""
                        };
                        user = _user;
                    }, onCancelled: (exception) =>
                    {
                        Toast.MakeText(Application.Context, exception.Message, ToastLength.Short).Show();
                    }));
                    userRef.KeepSynced(true);
                }
                catch (Exception e)
                {
                    Toast.MakeText(this, e.Message, ToastLength.Short).Show();
                }
            });
            return user;
        }

        private void ShowLoading()
        {
            loaderDialog = new SweetAlertDialog(this, SweetAlertDialog.ProgressType);
            loaderDialog.SetTitleText("Finding the nearest Oyadieyie...");
            loaderDialog.ShowCancelButton(false);
            loaderDialog.Show();
        }

        private void ShowSuccess()
        {
            var sweetprogress = new SweetAlertDialog(this, SweetAlertDialog.SuccessType);
            sweetprogress.SetTitleText("Success");
            sweetprogress.SetConfirmText("OK");
            sweetprogress.SetConfirmClickListener(null);
            sweetprogress.ShowCancelButton(false);
            sweetprogress.Show();
        }

        private void DismissLoading()
        {
            if (!loaderDialog.IsShowing)
                return;

            loaderDialog.DismissWithAnimation();
        }
        private void ShowError(string errorMsg)
        {
            new SweetAlertDialog(this, SweetAlertDialog.ErrorType)
                .SetTitleText("OOps...")
                .SetContentText(errorMsg)
                .SetConfirmText("OK")
                .ShowCancelButton(false)
                .SetConfirmClickListener(null)
                .Show();
        }

        private void ShowToast(string msg)
        {
            Toast.MakeText(this, msg, ToastLength.Short).Show();
        }


        private void CreateBottomSheet()
        {
            var postRoot = FindViewById<NestedScrollView>(Resource.Id.createpost_bottomsheet_root);

            postBottomsheetBehavior = BottomSheetBehavior.From(postRoot);
            postBottomsheetBehavior.Hideable = true;
            postBottomsheetBehavior.State = BottomSheetBehavior.StateHidden;

            var postToggleBtn = FindViewById<MaterialButtonToggleGroup>(Resource.Id.picture_toggle_group);
            postImageView = FindViewById<ImageView>(Resource.Id.post_imageview);
            profile_image_view = FindViewById<CircleImageView>(Resource.Id.create_post_userProfile);
            commentEt = FindViewById<TextInputLayout>(Resource.Id.comment_edit_text);
            doneBtn = FindViewById<MaterialButton>(Resource.Id.done_btn);
            postProgress = FindViewById<ProgressBar>(Resource.Id.post_progress);
            closeBtn = FindViewById<ImageButton>(Resource.Id.post_cancel_btn);

            icu = new ImageCaptureUtils(this);
            icu.OnImageCaptured += Icu_OnImageCaptured;
            icu.OnImageSelected += Icu_OnImageSelected;

            postToggleBtn.AddOnButtonCheckedListener(new ButtonCheckedListener(
            onbuttonChecked: (g, id, isChecked) =>
            {
                if (isChecked)
                {
                    switch (id)
                    {
                        case Resource.Id.btn_open_camera:
                            icu.TakeCameraImage();
                            break;

                        case Resource.Id.btn_choose_photo:
                            icu.FetchImageFromGallery();
                            break;
                    }
                }
            }));

            doneBtn.Click += DoneBtn_Click;
            commentEt.EditText.TextChanged += EditText_TextChanged;
            closeBtn.Click += CloseBtn_Click;
        }



        private void CloseBtn_Click(object sender, EventArgs e)
        {
            postBottomsheetBehavior.State = BottomSheetBehavior.StateHidden;
        }

        private void DoneBtn_Click(object sender, EventArgs e)
        {
            doneBtn.Post(async () =>
            {
                postProgress.Visibility = ViewStates.Visible;
                doneBtn.Enabled = false;
                doneBtn.Text = "Posting";
                try
                {
                    var stream = new System.IO.MemoryStream();
                    var bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, uri);
                    await bitmap.CompressAsync(Bitmap.CompressFormat.Webp, 70, stream);
                    var imgArray = stream.ToArray();

                    var postRef = SessionManager.GetFireDB().GetReference("posts").Push();
                    string imageId = postRef.Key;
                    imageRef = FirebaseStorage.Instance.GetReference("postImages/" + imageId);
                    imageRef.PutBytes(imgArray).ContinueWithTask(new ContinuationTask(
                    then: t =>
                    {
                        if (!t.IsSuccessful)
                            throw t.Exception;

                    })).AddOnCompleteListener(new OncompleteListener(
                    onComplete: t =>
                    {
                        if (!t.IsSuccessful)
                            throw t.Exception;

                        var postMap = new HashMap();
                        postMap.Put("owner_id", SessionManager.GetFirebaseAuth().CurrentUser.Uid);
                        postMap.Put("post_date", DateTime.UtcNow.ToString());
                        postMap.Put("post_body", commentEt.EditText.Text);
                        postMap.Put("download_url", t.Result.ToString());
                        postMap.Put("image_id", imageId);
                        postMap.Put("post_date", DateTime.UtcNow.ToString());
                        postRef.SetValue(postMap).AddOnCompleteListener(new OncompleteListener(
                        onComplete: task =>
                        {
                            try
                            {
                                if (!task.IsSuccessful)
                                    throw task.Exception;

                                postBottomsheetBehavior.State = BottomSheetBehavior.StateHidden;
                            }
                            catch (DatabaseException de)
                            {

                            }

                        }));
                    }));

                }
                catch (DatabaseException fde)
                {

                }
                catch (FirebaseNetworkException fne)
                {

                }
                catch (StorageException se)
                {

                }
                catch (FirebaseAuthException fae)
                {

                }
                catch (Exception ex)
                {

                }
            });
        }

        private void EditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            doneBtn.Enabled = commentEt.EditText.Text.Length >= 6 && hasImage;
        }

        private void Icu_OnImageSelected(object sender, ImageCaptureUtils.ImageSelectedEventArgs e)
        {
            StartActivityForResult(Intent.CreateChooser(e.imageIntent, "Select Picture"), SELECT_PICTURE);
        }

        private void Icu_OnImageCaptured(object sender, ImageCaptureUtils.ImageSelectedEventArgs e)
        {
            StartActivityForResult(e.imageIntent, REQUEST_IMAGE_CAPTURE);
            fileName = e.fileName;
        }

        private void CropImage(Uri selectedImageURI)
        {
            Uri destinationUri = Uri.FromFile(new File(CacheDir, QueryName(ContentResolver, selectedImageURI)));
            UCrop.Options options = new UCrop.Options();
            options.SetCompressionQuality(IMAGE_COMPRESSION);
            options.SetToolbarColor(ContextCompat.GetColor(this, Resource.Color.colorPrimary));
            options.SetStatusBarColor(ContextCompat.GetColor(this, Resource.Color.colorPrimaryDark));
            options.SetActiveControlsWidgetColor(ContextCompat.GetColor(this, Resource.Color.colorAccent));

            if (lockAspectRatio)
                options.WithAspectRatio(ASPECT_RATIO_X, ASPECT_RATIO_Y);

            if (setBitmapMaxWidthHeight)
                options.WithMaxResultSize(bitmapMaxWidth, bitmapMaxHeight);

            UCrop.Of(selectedImageURI, destinationUri)
                .WithOptions(options)
                .Start(this, UCROP_REQUEST);
        }

        private static string QueryName(ContentResolver contentResolver, Uri uri)
        {
            ICursor returnCursor = contentResolver.Query(uri, null, null, null, null);
            System.Diagnostics.Debug.Assert(returnCursor != null);
            int nameIndex = returnCursor.GetColumnIndex(OpenableColumns.DisplayName);
            returnCursor.MoveToFirst();
            string name = returnCursor.GetString(nameIndex);
            returnCursor.Close();
            return name;
        }

        private Uri GetCacheImagePath(string fileName)
        {
            File path = new File(ExternalCacheDir, "camera");
            if (!path.Exists()) path.Mkdirs();
            File image = new File(path, fileName);
            return GetUriForFile(this, PackageName + ".fileprovider", image);
        }

        public void ClearCache()
        {
            File path = new File(ExternalCacheDir, "camera");
            if (path.Exists() && path.IsDirectory)
            {
                foreach (File child in path.ListFiles())
                {
                    child.Delete();
                }
            }
        }

        internal sealed class MyLocationCallback : LocationCallback
        {
            private Action<LocationResult> _onLocationresult;
            public MyLocationCallback(Action<LocationResult> onLocationresult)
            {
                _onLocationresult = onLocationresult;
            }
            public override void OnLocationResult(LocationResult result)
            {
                _onLocationresult?.Invoke(result);
            }
        }

        private class ContinuationTask : Java.Lang.Object, IContinuation
        {
            private Action<Task> _then;

            public ContinuationTask(Action<Task> then)
            {
                _then = then;
            }

            public Java.Lang.Object Then(Task task)
            {
                _then?.Invoke(task);
                return imageRef.GetDownloadUrl();
            }
        }
    }
}
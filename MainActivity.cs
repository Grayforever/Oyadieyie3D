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
using AndroidX.LocalBroadcastManager.Content;
using AndroidX.Preference;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using BumpTech.GlideLib;
using BumpTech.GlideLib.Requests;
using CN.Pedant.SweetAlert;
using Com.Yalantis.Ucrop;
using Firebase.Database;
using Firebase.Storage;
using Google.Android.Material.BottomAppBar;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Java.IO;
using Java.Util;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Adapters;
using Oyadieyie3D.BroadcastReceivers;
using Oyadieyie3D.Callbacks;
using Oyadieyie3D.Events;
using Oyadieyie3D.Fragments;
using Oyadieyie3D.HelperClasses;
using Oyadieyie3D.Models;
using Oyadieyie3D.Parcelables;
using Oyadieyie3D.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static AndroidX.Core.Content.FileProvider;
using Exception = System.Exception;
using SearchView = AndroidX.AppCompat.Widget.SearchView;
using Task = Android.Gms.Tasks.Task;
using Uri = Android.Net.Uri;
using ViewAnimator = Oyadieyie3D.HelperClasses.ViewAnimator;

namespace Oyadieyie3D
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class MainActivity : AppCompatActivity, IServiceConnection
    {
        private RecyclerView mainRecycler;
        private ConstraintLayout emptyRoot;
        private SwipeRefreshLayout swipe_container;
        private FloatingActionButton addPostFab;
        private List<Post> posts { get; set; }
        public PostAdapter postAdapter;
        private RecyclerViewEmptyObserver emptyObserver;
        private User user { get; set; }
        PostEventListener postEventListener = new PostEventListener();
        private ProfileParcelable profileParcelable = new ProfileParcelable();
        private SweetAlertDialog loaderDialog;

        private const string HAS_IMAGE_KEY = "has_image";

        private int nextDrawableId = Resource.Drawable.ic_close;
        private int rotation = 180;
        
        private ImageView postImageView;
        private TextInputLayout commentEt;
        private MaterialButton doneBtn;
        
        public static string fileName;
        private string profile_url;
        private ImageCaptureUtils icu;
        private bool hasImage = false;
        private ProgressBar postProgress;
        private static StorageReference imageRef;
        private Uri uri;
        private RequestOptions requestOptions;

        private BottomSheetBehavior postBottomsheetBehavior;

        private Intent intent;
        private IntentFilter filter;
        private bool isBioEnabled;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            mainRecycler = FindViewById<RecyclerView>(Resource.Id.main_recycler);
            emptyRoot = FindViewById<ConstraintLayout>(Resource.Id.rv_empty_view);
            swipe_container = FindViewById<SwipeRefreshLayout>(Resource.Id.main_refresher);
            addPostFab = FindViewById<FloatingActionButton>(Resource.Id.post_fab);
            var appbar = FindViewById<BottomAppBar>(Resource.Id.bottomAppBar);
            var searchView = appbar.Menu.FindItem(Resource.Id.action_search).ActionView as SearchView;

            var prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            isBioEnabled = prefs.GetBoolean(Constants.BioStatusKey, false);

            searchView.QueryTextChange += (s, e) =>
            {

            };
            appbar.NavigationClick += (s, e) =>
            {

            };
            appbar.MenuItemClick += (s2, e2) =>
            {
                switch (e2.Item.ItemId)
                {
                    case Resource.Id.action_dark_theme:
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                        break;

                    case Resource.Id.action_light_theme:
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                        break;

                    case Resource.Id.action_settings:
                        var intent = new Intent(this, typeof(SettingsActivity));
                        profileParcelable.UserProfile = user;
                        intent.PutExtra(Constants.PROFILE_DATA_EXTRA, profileParcelable);
                        StartActivity(intent);
                        break;
                }
            };
            addPostFab.Click += AddPostFab_Click;

            postEventListener.FetchPost();
            postEventListener.OnPostRetrieved += PostEventListener_OnPostRetrieved;
            GetUserFromFireAsync();
            //CreateBottomSheet();
        }

        protected override void OnStart()
        {
            base.OnStart();
            intent = new Intent(this, typeof(BiometricPromptTimerService));
            filter = new IntentFilter(BiometricPromptTimerService.TimeInfo);
            BindService(intent, this, Bind.None);
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (!isBioEnabled)
                return;

            LocalBroadcastManager.GetInstance(this).RegisterReceiver(new AppBroadcastReceiver((context, intent) =>
            {
                if (intent != null && intent.Action.Equals(BiometricPromptTimerService.TimeInfo))
                {
                    if (!intent.HasExtra("VALUE"))
                        return;

                    switch (intent.GetStringExtra("VALUE"))
                    {
                        case "Stopped":
                        case "InProgress":
                            break;
                        default:
                            ShowBiometricDialog();
                            break;
                    }
                }
            }), filter);
            StartService(intent);
        }

        private void ShowBiometricDialog()
        {
            var bioPromptFragment = new BiometricPromptSheet();
            bioPromptFragment.Cancelable = false;

            var ft = SupportFragmentManager.BeginTransaction();
            ft.Add(bioPromptFragment, "bio_prompt_sheet");
            ft.CommitAllowingStateLoss();
        }

        protected override void OnStop()
        {
            base.OnStop();
            UnbindService(this);
        }

        public void OnServiceConnected(ComponentName name, IBinder service) { }

        public void OnServiceDisconnected(ComponentName name) { }

        private void PostEventListener_OnPostRetrieved(object sender, PostEventListener.PostEventArgs e)
        {
            if (e.Posts == null)
                return;

            posts = e.Posts.OrderByDescending(p => p.PostDate).ToList();
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
                        .AddOnCompleteListener(new OncompleteListener((onComplete) =>
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

        bool isRotate = false;
        private void AddPostFab_Click(object sender, EventArgs e)
        {
            try
            {
                isRotate = ViewAnimator.RotateFab(addPostFab, !isRotate);
                switch (postBottomsheetBehavior.State)
                {
                    case BottomSheetBehavior.StateHidden:
                        postBottomsheetBehavior.State = BottomSheetBehavior.StateExpanded;
                        break;

                    default:
                        postBottomsheetBehavior.State = BottomSheetBehavior.StateHidden;
                        break;
                } 
            }
            catch (Exception)
            {
                ShowToast("No network connection");
            }
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

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if (resultCode == Result.Ok)
                {
                    if (requestCode == Constants.SELECT_PICTURE)
                    {
                        Uri selectedImageURI = data.Data;
                        CropImage(selectedImageURI);
                    }
                    else if (requestCode == Constants.REQUEST_IMAGE_CAPTURE)
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
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public User GetUserFromFireAsync()
        {
            AsyncTask.Execute(new MyRunnable(() => 
            {
                try
                {
                    Looper.Prepare();
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
                    Looper.Loop();
                }
                catch (Exception e)
                {
                    Toast.MakeText(this, e.Message, ToastLength.Short).Show();
                }
            }));
            return user;
        }

        private void ShowToast(string msg)
        {
            Toast.MakeText(this, msg, ToastLength.Short).Show();
        }

        private void CreateBottomSheet()
        {
            var postRoot = FindViewById<NestedScrollView>(Resource.Id.createpost_bottomsheet_root);

            postBottomsheetBehavior = BottomSheetBehavior.From(postRoot);
            postBottomsheetBehavior.State = BottomSheetBehavior.StateHidden;
            postBottomsheetBehavior.AddBottomSheetCallback(new BottomSheetCallback(
                onSlide: (bottomSheet, newState) =>
                {

                }, onStateChanged: (bottomsheet, state) =>
                {
                    isRotate = ViewAnimator.RotateFab(addPostFab, !isRotate);
                }));

            var postToggleBtn = FindViewById<MaterialButtonToggleGroup>(Resource.Id.picture_toggle_group);
            postImageView = FindViewById<ImageView>(Resource.Id.post_imageview);
            commentEt = FindViewById<TextInputLayout>(Resource.Id.comment_edit_text);
            doneBtn = FindViewById<MaterialButton>(Resource.Id.done_btn);
            postProgress = FindViewById<ProgressBar>(Resource.Id.post_progress);

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
        }

        private async void DoneBtn_Click(object sender, EventArgs e)
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
            catch (Exception ex)
            {

            }
        }

        private void EditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            doneBtn.Enabled = commentEt.EditText.Text.Length >= 6 && hasImage;
        }

        private void Icu_OnImageSelected(object sender, ImageCaptureUtils.ImageSelectedEventArgs e)
        {
            StartActivityForResult(Intent.CreateChooser(e.imageIntent, "Select Picture"), Constants.SELECT_PICTURE);
        }

        private void Icu_OnImageCaptured(object sender, ImageCaptureUtils.ImageSelectedEventArgs e)
        {
            StartActivityForResult(e.imageIntent, Constants.REQUEST_IMAGE_CAPTURE);
            fileName = e.fileName;
        }

        private void CropImage(Uri selectedImageURI)
        {
            Uri destinationUri = Uri.FromFile(new File(CacheDir, QueryName(ContentResolver, selectedImageURI)));
            UCrop.Options options = new UCrop.Options();
            options.SetCompressionQuality(Constants.IMAGE_COMPRESSION);
            options.SetToolbarColor(ContextCompat.GetColor(this, Resource.Color.colorPrimary));
            options.SetStatusBarColor(ContextCompat.GetColor(this, Resource.Color.colorPrimaryDark));
            options.SetActiveControlsWidgetColor(ContextCompat.GetColor(this, Resource.Color.colorAccent));

            if (Constants.lockAspectRatio)
                options.WithAspectRatio(Constants.ASPECT_RATIO_X, Constants.ASPECT_RATIO_Y);

            if (Constants.setBitmapMaxWidthHeight)
                options.WithMaxResultSize(Constants.bitmapMaxWidth, Constants.bitmapMaxHeight);

            UCrop.Of(selectedImageURI, destinationUri)
                .WithOptions(options)
                .Start(this, UCrop.RequestCrop);
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
                foreach (var child in path.ListFiles())
                {
                    child.Delete();
                }
            }
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
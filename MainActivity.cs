using Android.App;
using Android.Content;
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
using AndroidX.Core.View;
using AndroidX.Core.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using BumpTech.GlideLib;
using CN.Pedant.SweetAlert;
using Com.Yalantis.Ucrop;
using Firebase.Database;
using Firebase.Storage;
using Google.Android.Material.BottomAppBar;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
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
using Exception = System.Exception;
using SearchView = AndroidX.AppCompat.Widget.SearchView;
using Task = Android.Gms.Tasks.Task;
using Uri = Android.Net.Uri;
using ViewAnimator = Oyadieyie3D.HelperClasses.ViewAnimator;

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
        private List<Post> posts { get; set; }
        public PostAdapter postAdapter;
        private RecyclerViewEmptyObserver emptyObserver;
        PostEventListener postEventListener = new PostEventListener();
        private const string HAS_IMAGE_KEY = "has_image";
        
        private ImageView postImageView;
        private TextInputLayout commentEt;
        private MaterialButton doneBtn;
        
        public static string fileName;
        private ImageCaptureUtils icu;
        private bool hasImage = false;
        private ProgressBar postProgress;
        private static StorageReference imageRef;
        private Uri uri;

        private BottomSheetBehavior postBottomsheetBehavior;
        bool isRotate = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            PreferenceHelper.Init(this);
            AppCompatDelegate.DefaultNightMode = PreferenceHelper.Instance.GetBoolean("theme") ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo;

            SetContentView(Resource.Layout.activity_main);
            mainRecycler = FindViewById<RecyclerView>(Resource.Id.main_recycler);
            emptyRoot = FindViewById<ConstraintLayout>(Resource.Id.rv_empty_view);
            swipe_container = FindViewById<SwipeRefreshLayout>(Resource.Id.main_refresher);
            addPostFab = FindViewById<FloatingActionButton>(Resource.Id.post_fab);
            var appbar = FindViewById<BottomAppBar>(Resource.Id.bottomAppBar);
            var searchView = appbar.Menu.FindItem(Resource.Id.action_search).ActionView as SearchView;

            UCropHelper.Init(this);
            CreateBottomSheet();

            searchView.QueryTextChange += (s, e) =>
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
                        PreferenceHelper.Instance.SetBoolean("theme", true);
                        break;

                    case Resource.Id.action_light_theme:
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                        PreferenceHelper.Instance.SetBoolean("theme", false);
                        break;

                    default:
                        StartActivity(typeof(SettingsActivity));
                        break;
                }
            };
            addPostFab.Click += (s3, e3) =>
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
                catch (Exception e)
                {
                    ShowToast(e.Message);
                }
            };

            //postEventListener.FetchPost();
            //postEventListener.OnPostRetrieved += PostEventListener_OnPostRetrieved;
        }

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

        //public User GetUserFromFireAsync()
        //{
        //    AsyncTask.Execute(new MyRunnable(() => 
        //    {
        //        try
        //        {
        //            Looper.Prepare();
        //            string userId = SessionManager.GetFirebaseAuth().CurrentUser.Uid;
        //            var userRef = SessionManager.GetFireDB().GetReference($"users/{userId}/profile");
        //            userRef.AddValueEventListener(new SingleValueListener(
        //            onDataChange: (snapshot) =>
        //            {
        //                if (!snapshot.Exists())
        //                    return;

        //                var _user = new User
        //                {
        //                    ID = snapshot.Key,
        //                    Username = snapshot.Child(Constants.SNAPSHOT_FNAME) != null ? snapshot.Child(Constants.SNAPSHOT_FNAME).Value.ToString() : "",
        //                    Status = snapshot.Child(Constants.SNAPSHOT_GENDER) != null ? snapshot.Child(Constants.SNAPSHOT_GENDER).Value.ToString() : "",
        //                    ProfileImgUrl = snapshot.Child(Constants.SNAPSHOT_PHOTO_URL) != null ? snapshot.Child(Constants.SNAPSHOT_PHOTO_URL).Value.ToString() : "",
        //                    Email = snapshot.Child(Constants.SNAPSHOT_EMAIL) != null ? snapshot.Child(Constants.SNAPSHOT_EMAIL).Value.ToString() : "",
        //                    Phone = snapshot.Child(Constants.SNAPSHOT_PHONE) != null ? snapshot.Child(Constants.SNAPSHOT_PHONE).Value.ToString() : ""
        //                };
        //                user = _user;
        //            }, onCancelled: (exception) =>
        //            {
        //                Toast.MakeText(Application.Context, exception.Message, ToastLength.Short).Show();
        //            }));
        //            Looper.Loop();
        //        }
        //        catch (Exception e)
        //        {
        //            Toast.MakeText(this, e.Message, ToastLength.Short).Show();
        //        }
        //    }));
        //    return user;
        //}

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

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                switch (resultCode)
                {
                    case Result.Ok:
                        switch (requestCode)
                        {
                            case Constants.SELECT_PICTURE:
                                Uri selectedImageURI = data.Data;
                                UCropHelper.Instance.CropImage(selectedImageURI, this);
                                break;

                            case Constants.REQUEST_IMAGE_CAPTURE:
                                UCropHelper.Instance.CropImage(UCropHelper.Instance.GetCacheImagePath(fileName), this);
                                break;

                            case UCrop.RequestCrop:
                                uri = UCrop.GetOutput(data);
                                Glide.With(this).Load(uri).Into(postImageView);
                                hasImage = true;
                                doneBtn.Enabled = commentEt.EditText.Text.Length >= 6 && hasImage;
                                UCropHelper.Instance.ClearCache();
                                break;

                            case UCrop.ResultError:
                                throw UCrop.GetError(data);

                            default:
                                break;
                        }
                        break;
                    case Result.Canceled:
                        break;

                    case Result.FirstUser:
                        break;

                    default:
                        break;
                }
            }
            catch (Exception)
            {

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
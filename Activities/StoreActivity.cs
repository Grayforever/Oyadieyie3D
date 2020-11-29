using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.View;
using AndroidX.Core.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using BumpTech.GlideLib;
using CN.Pedant.SweetAlert;
using Com.Yalantis.Ucrop;
using Firebase.Storage;
using Google.Android.Material.AppBar;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
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
using Uri = Android.Net.Uri;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class StoreActivity : AppCompatActivity
    {
        private RecyclerView mainRecycler;
        private SwipeRefreshLayout swipe_container;
        private List<Post> posts { get; set; }
        public PostAdapter postAdapter;
        PostEventListener postEventListener = new PostEventListener();
        private const string HAS_IMAGE_KEY = "has_image";

        private ImageView postImageView;
        private TextInputLayout commentEt;
        private MaterialButton doneBtn;

        public static string fileName;
        private ImageCaptureUtils icu;
        private bool hasImage = false;
        private ProgressBar postProgress;
        //private static StorageReference imageRef;
        private Uri uri;
        private BottomSheetBehavior postBottomsheetBehavior;

        private bool isOpen = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.store_layout);

            mainRecycler = FindViewById<RecyclerView>(Resource.Id.main_recycler);
            swipe_container = FindViewById<SwipeRefreshLayout>(Resource.Id.main_refresher);
            var appbar = FindViewById<AppBarLayout>(Resource.Id.store_appbar);
            var toolbar = appbar.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.store_toolbar);

            var searchView = toolbar.Menu.FindItem(Resource.Id.action_search).ActionView as AndroidX.AppCompat.Widget.SearchView;
            searchView.QueryTextChange += (s, e) =>
            {
                
            };
             
            toolbar.MenuItemClick += (s1, e1) =>
            {
                switch (e1.Item.ItemId)
                {
                    case Resource.Id.action_help:
                        //postBottomsheetBehavior.State = isOpen ? BottomSheetBehavior.StateHidden : BottomSheetBehavior.StateExpanded;
                        break;

                    default:
                        break;
                }
            };

            CreateBottomSheet();
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
                    switch (state)
                    {
                        case BottomSheetBehavior.StateExpanded:
                            isOpen = true;
                            break;
                        default:
                            isOpen = false;
                            break;
                    }
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

            commentEt.EditText.TextChanged += EditText_TextChanged;
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

        public override void OnBackPressed()
        {
            if (!isOpen)
                base.OnBackPressed();

            postBottomsheetBehavior.State = BottomSheetBehavior.StateHidden;
            
        }
    }
}
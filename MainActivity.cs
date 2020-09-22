using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.App;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;
using CN.Pedant.SweetAlert;
using Firebase.Storage;
using Google.Android.Material.AppBar;
using Google.Android.Material.FloatingActionButton;
using Karumi.DexterLib;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Adapters;
using Oyadieyie3D.Events;
using Oyadieyie3D.Fragments;
using Oyadieyie3D.HelperClasses;
using Oyadieyie3D.Models;
using Oyadieyie3D.Parcelables;
using Oyadieyie3D.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using SearchView = AndroidX.AppCompat.Widget.SearchView;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Oyadieyie3D
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class MainActivity : AppCompatActivity
    {
        private List<Post> ListOfPost;
        private RecyclerView mainRecycler;
        private ConstraintLayout emptyRoot;
        private PostAdapter postAdapter;
        private CreatePostFragment createPost;
        private RecyclerViewEmptyObserver emptyObserver;
        private GetProfileData getProfile = new GetProfileData();
        private PostEventListener postEventListener;
        private User _user;
        private ProfileParcelable profileParcelable = new ProfileParcelable();
        private static SweetAlertDialog loaderDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            createPost = new CreatePostFragment(this);
            createPost.Cancelable = false;
            createPost.OnPostComplete += CreatePost_OnPostComplete;
            createPost.OnErrorEncounted += CreatePost_OnErrorEncounted;
            GetControls();
            FetchPost();
            getProfile.GetUserFromFire();
            getProfile.OnProfileRetrieved += GetProfile_OnProfileRetrieved;
        }

        private void GetProfile_OnProfileRetrieved(object sender, GetProfileData.ProfileEventArgs e) => _user = e.user;

        private void CreatePost_OnErrorEncounted(object sender, CreatePostFragment.ErrorEventArgs e) => ShowError(e.ErrorMsg);

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

        private void CreatePost_OnPostComplete(object sender, EventArgs e) => ShowSuccess();

        private void ShowSuccess()
        {
            new SweetAlertDialog(this, SweetAlertDialog.SuccessType)
                .SetTitleText("Success")
                .SetConfirmText("OK")
                .SetConfirmClickListener(null)
                .ShowCancelButton(false)
                .Show();
        }

        private void SetUpRecycler()
        {
            postAdapter = new PostAdapter(ListOfPost);
            mainRecycler.SetAdapter(postAdapter);
            emptyObserver = new RecyclerViewEmptyObserver(mainRecycler, emptyRoot);
            postAdapter.RegisterAdapterDataObserver(emptyObserver);
            postAdapter.ItemLongClick += PostAdapter_ItemLongClick;
            postAdapter.ItemClick += PostAdapter_ItemClick;
        }

        private void PostAdapter_ItemClick(object sender, PostAdapter.PostAdapterClickEventArgs e)
        {
            var intent = new Intent(this, typeof(FullscreenImageActivity));

            PostParcelable postParcelable = new PostParcelable();
            postParcelable.PostItem = ListOfPost[e.Position];

            intent.PutExtra("extra_transition_name", ViewCompat.GetTransitionName(e.ImageView));
            intent.PutExtra("extra_post_data", postParcelable);
            intent.PutExtra("parcel_type", 0);
            var options = ActivityOptionsCompat.MakeSceneTransitionAnimation(this, e.ImageView,
                ViewCompat.GetTransitionName(e.ImageView));
            StartActivity(intent, options.ToBundle());
        }

        private void GetControls()
        {
            mainRecycler = FindViewById<RecyclerView>(Resource.Id.main_recycler);
            emptyRoot = FindViewById<ConstraintLayout>(Resource.Id.rv_empty_view);
            var addPostFab = FindViewById<FloatingActionButton>(Resource.Id.post_fab);
            var mainAppBar = FindViewById<AppBarLayout>(Resource.Id.activity_main_appbar);
            var mainToolbar = mainAppBar.FindViewById<Toolbar>(Resource.Id.main_toolbar);
            var itemDecor = new DividerItemDecoration(this, DividerItemDecoration.Horizontal);
            
            SetSupportActionBar(mainToolbar);

            addPostFab.Click += AddPostFab_Click;

            mainRecycler.AddOnScrollListener(new OnscrollListener((r, dx, dy) =>
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
            mainRecycler.AddItemDecoration(itemDecor);
        }

        private void AddPostFab_Click(object sender, EventArgs e)
        {
            var ft = SupportFragmentManager.BeginTransaction();
            ft.Add(createPost, "createPost");
            ft.CommitAllowingStateLoss();
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
            List<Post> searchResult = 
                (from post in ListOfPost
                 where post.PostBody.ToLower().Contains(e.NewText.ToLower())
                 select post).ToList();

            postAdapter = new PostAdapter(searchResult);
            postAdapter.RegisterAdapterDataObserver(emptyObserver);
            mainRecycler.SetAdapter(postAdapter);
            postAdapter.ItemClick += PostAdapter_ItemClick;
            postAdapter.ItemLongClick += PostAdapter_ItemLongClick;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_refresh:
                    break;

                case Resource.Id.action_finder:
                    ShowFinderDialog();
                    break;

                case Resource.Id.action_settings:
                    var intent = new Intent(this, typeof(SettingsActivity));
                    profileParcelable.UserProfile = _user;
                    intent.PutExtra("extra_profile_data", profileParcelable);
                    StartActivity(intent);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void ShowFinderDialog()
        {
            Dexter.WithActivity(this)
                .WithPermissions(Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation)
                .WithListener(new MultiplePermissionsListener((permission, token) =>
                {
                    token.ContinuePermissionRequest();
                }, (report)=> 
                {
                    if (report.AreAllPermissionsGranted())
                    {
                        ShowLoading();

                    }
                    else
                    {
                        
                    }
                })).Check();
        }

        private void ShowLoading()
        {
            loaderDialog = new SweetAlertDialog(this, SweetAlertDialog.ProgressType);
            loaderDialog.SetTitleText("Finding the nearest Oyadieyie...");
            loaderDialog.ShowCancelButton(false);
            loaderDialog.Show();
        }

        private void DismissLoading()
        {
            if (!loaderDialog.IsShowing)
                return;

            loaderDialog.DismissWithAnimation();
        }

        private void PostAdapter_ItemLongClick(object sender, PostAdapter.PostAdapterClickEventArgs e)
        {
            string postID = ListOfPost[e.Position].ID;
            string ownerID = ListOfPost[e.Position].OwnerId;

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
            onClick: async d2 =>
            {
                await SessionManager.GetFireDB().GetReference("posts").Child(postID).RemoveValueAsync();
                StorageReference storageReference = FirebaseStorage.Instance.GetReference("postImages/" + postID);
                storageReference.Delete();
                postAdapter.NotifyDataSetChanged();

                d2.ChangeAlertType(SweetAlertDialog.SuccessType);
                d2.SetTitleText("Deleted");
                d2.SetContentText("");
                d2.ShowCancelButton(false);
                d2.SetConfirmText("OK");
                d2.SetConfirmClickListener(null);
                d2.Show();
            }));
            sweetDialog.Show();
        }

        private void FetchPost()
        {
            postEventListener = new PostEventListener();
            postEventListener.FetchPost();
            postEventListener.OnPostRetrieved += PostEventListener_OnPostRetrieved;
        }

        private void PostEventListener_OnPostRetrieved(object sender, PostEventListener.PostEventArgs e)
        {
            ListOfPost = new List<Post>();
            ListOfPost = e.Posts;

            if (ListOfPost == null)
                return;

            SetUpRecycler();
        }



        internal sealed class GetProfileData
        {
            public event EventHandler<ProfileEventArgs> OnProfileRetrieved;
            public class ProfileEventArgs : EventArgs
            {
                public User user { get; set; }
            }

            public void GetUserFromFire()
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

                        var _user = new User();
                        _user.ID = snapshot.Key;
                        _user.Username = snapshot.Child("fname") != null ? snapshot.Child("fname").Value.ToString() : "";
                        _user.Status = snapshot.Child("gender") != null ? snapshot.Child("gender").Value.ToString() : "";
                        _user.ProfileImgUrl = snapshot.Child("photoUrl") != null ? snapshot.Child("photoUrl").Value.ToString() : "";
                        _user.Email = snapshot.Child("email") != null ? snapshot.Child("email").Value.ToString() : "";
                        _user.Phone = snapshot.Child("phone") != null ? snapshot.Child("phone").Value.ToString() : "";

                        OnProfileRetrieved?.Invoke(this, new ProfileEventArgs { user = _user });
                    }, onCancelled: (exception) =>
                    {
                        
                    }));
                    userRef.KeepSynced(true);
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }
    }
}
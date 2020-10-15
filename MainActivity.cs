using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.App;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using CN.Pedant.SweetAlert;
using Firebase.Storage;
using Google.Android.Material.AppBar;
using Google.Android.Material.FloatingActionButton;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Adapters;
using Oyadieyie3D.Callbacks;
using Oyadieyie3D.Events;
using Oyadieyie3D.Fragments;
using Oyadieyie3D.HelperClasses;
using Oyadieyie3D.Models;
using Oyadieyie3D.Parcelables;
using Oyadieyie3D.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActionMode = Android.Views.ActionMode;
using SearchView = AndroidX.AppCompat.Widget.SearchView;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

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
        private CreatePostFragment createPost;
        private RecyclerViewEmptyObserver emptyObserver;
        private User user { get; set; }
        private List<Post> posts;
        PostEventListener postEventListener = new PostEventListener();
        private ProfileParcelable profileParcelable = new ProfileParcelable();
        private SweetAlertDialog loaderDialog;
        private ActionModeCallback actionModeCallback;
        private ActionMode actionMode;

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
            posts = new List<Post>();
           

            profileParcelable.WriteTOParcelFailed += ProfileParcelable_WriteTOParcelFailed;
            await GetUserFromFireAsync();
            postEventListener.FetchPost();
            postEventListener.OnPostRetrieved += PostEventListener_OnPostRetrieved;
        }

        private void PostEventListener_OnPostRetrieved(object sender, PostEventListener.PostEventArgs e)
        {
            if (e.Posts == null)
                return;

            posts = e.Posts;
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
                    createPost = new CreatePostFragment();
                    createPost.Cancelable = false;
                    var b = new Bundle();
                    b.PutString(Constants.IMG_URL_KEY, user.ProfileImgUrl);
                    createPost.Arguments = b;
                    createPost.OnPostComplete += CreatePost_OnPostComplete;
                    createPost.OnErrorEncounted += CreatePost_OnErrorEncounted;
                    var ft = base.SupportFragmentManager.BeginTransaction();
                    ft.Add(createPost, "createPost");
                    ft.CommitAllowingStateLoss();
                }
                catch (Exception)
                {
                    ShowToast("No network connection");
                }
            });
        }

        private void CreatePost_OnPostComplete(object sender, EventArgs e) => ShowSuccess();

        private void CreatePost_OnErrorEncounted(object sender, CreatePostFragment.ErrorEventArgs e) => ShowError(e.ErrorMsg);


        private void SetUpRecycler()
        {
            postAdapter = new PostAdapter(this, posts);
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
            if (requestCode == 500)
            {
                switch (resultCode)
                {
                    case Result.Ok:
                        ShowSuccess();
                        break;
                    case Result.Canceled:
                        ShowError("slydepay failed");
                        break;
                    case Result.FirstUser:
                        ShowError("You naa cancel the thing");
                        break;
                }
            }
        }

        public async Task<User> GetUserFromFireAsync()
        {
            await Task.Run(() =>
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
    }
}
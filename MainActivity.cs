using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.App;
using AndroidX.Core.View;
using AndroidX.Preference;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using CN.Pedant.SweetAlert;
using Firebase.Storage;
using Google.Android.Material.AppBar;
using Google.Android.Material.FloatingActionButton;
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
        private RecyclerView mainRecycler;
        private ConstraintLayout emptyRoot;
        private SwipeRefreshLayout swipe_container;
        private FloatingActionButton addPostFab;
        private PostAdapter postAdapter;
        private CreatePostFragment createPost;
        private RecyclerViewEmptyObserver emptyObserver;
        private GetProfileData getProfile;
        private PostEventListener postEventListener;
        private User _user;
        private ProfileParcelable profileParcelable = new ProfileParcelable();
        private SweetAlertDialog loaderDialog;
        public static string IMG_URL_KEY = "img_url";
        private ISharedPreferences prefs;

        protected override void OnCreate(Bundle savedInstanceState)
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
            swipe_container.Refreshing = true;
            FetchPost();
            FetchUser();
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            swipe_container.Refresh += (s1, e1) =>
            {
                swipe_container.PostDelayed(() =>
                {
                    swipe_container.Refreshing = true;
                    FetchPost();
                }, 4000);
                
            };



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

            profileParcelable.WriteTOParcelFailed += ProfileParcelable_WriteTOParcelFailed;
        }

        private void ProfileParcelable_WriteTOParcelFailed(object sender, EventArgs e)
        {
            Toast.MakeText(this, "No network connection", ToastLength.Short).Show();
        }

        private void AddPostFab_Click(object sender, EventArgs e)
        {
            addPostFab.Post(() =>
            {
                try
                {
                    createPost = new CreatePostFragment();
                    createPost.Cancelable = false;
                    var b = new Bundle();
                    b.PutString(IMG_URL_KEY, _user.ProfileImgUrl);
                    createPost.Arguments = b;
                    createPost.OnPostComplete += CreatePost_OnPostComplete;
                    createPost.OnErrorEncounted += CreatePost_OnErrorEncounted;
                    var ft = base.SupportFragmentManager.BeginTransaction();
                    ft.Add(createPost, "createPost");
                    ft.CommitAllowingStateLoss();
                }
                catch (Exception)
                {

                    return;
                }
            });
        }

        private void CreatePost_OnPostComplete(object sender, EventArgs e) => ShowSuccess();

        private void CreatePost_OnErrorEncounted(object sender, CreatePostFragment.ErrorEventArgs e) => ShowError(e.ErrorMsg);

        private void FetchPost()
        {
            postEventListener = new PostEventListener();
            postEventListener.FetchPost();
            postEventListener.OnPostRetrieved += PostEventListener_OnPostRetrieved;
        }

        private void PostEventListener_OnPostRetrieved(object sender, PostEventListener.PostEventArgs e)
        {
            var list_of_post = new List<Post>();
            list_of_post = e.Posts;

            if (list_of_post == null)
                return;

            list_of_post = list_of_post.OrderByDescending(x => x.PostDate).ToList();
            SetUpRecycler(list_of_post);
            swipe_container.Refreshing = false;
        }

        private void SetUpRecycler(List<Post> list_of_post)
        {

            postAdapter = new PostAdapter(list_of_post);
            mainRecycler.SetAdapter(postAdapter);
            emptyObserver = new RecyclerViewEmptyObserver(mainRecycler, emptyRoot);
            postAdapter.RegisterAdapterDataObserver(emptyObserver);
            postAdapter.ItemLongClick += (s1, e1) =>
            {
                string postID = list_of_post[e1.Position].ID;
                string ownerID = list_of_post[e1.Position].OwnerId;

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
                postParcelable.PostItem = list_of_post[e2.Position];

                intent.PutExtra("extra_transition_name", ViewCompat.GetTransitionName(e2.ImageView));
                intent.PutExtra("extra_post_data", postParcelable);
                intent.PutExtra("parcel_type", 0);
                var options = ActivityOptionsCompat.MakeSceneTransitionAnimation(this, e2.ImageView,
                    ViewCompat.GetTransitionName(e2.ImageView));
                StartActivity(intent, options.ToBundle());
            };
        }

        private void FetchUser()
        {
            getProfile = new GetProfileData();
            getProfile.GetUserFromFire();
            getProfile.OnProfileRetrieved += GetProfile_OnProfileRetrieved;
        }

        private void GetProfile_OnProfileRetrieved(object sender, GetProfileData.ProfileEventArgs e) => _user = e.user;

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
            //            (from post in list_of_post
            //             where post.PostBody.ToLower().Contains(e.NewText.ToLower())
            //             select post).ToList();

            //    postAdapter = new PostAdapter(searchResult);
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
                    bool isOnline = prefs.GetBoolean("online_switch", false);
                    switch (isOnline)
                    {
                        case false:
                            ShowError("Finder needs your online status to be on. Please turn it on in settings.");
                            break;
                        default:
                            ShowSuccess();
                            break;
                    }
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

        //private void ShowFinderDialog()
        //{
        //    Dexter.WithActivity(this)
        //        .WithPermissions(Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation)
        //        .WithListener(new MultiplePermissionsListener((permission, token) =>
        //        {
        //            token.ContinuePermissionRequest();
        //        }, (report)=> 
        //        {
        //            if (report.AreAllPermissionsGranted())
        //            {
        //                ShowLoading();

        //                locationCallback = new MyLocationCallback(
        //                onLocationresult: locationResult =>
        //                {
        //                    if (locationResult == null)
        //                        return;

        //                    foreach(var location in locationResult.Locations)
        //                    {
        //                        mylocation = location;
        //                        var ll = new LatLng(mylocation.Latitude, mylocation.Longitude);
        //                        Toast.MakeText(this, $"{ll}", ToastLength.Short).Show();
        //                    }
        //                });


        //                fusedLocationClient = LocationServices.GetFusedLocationProviderClient(this);
        //                fusedLocationClient.GetLastLocation()
        //                    .AddOnCompleteListener(this, new OncompleteListener((t)=> 
        //                    {
        //                        if (!t.IsSuccessful)
        //                            return;

        //                        mylocation = t.Result as Location;
        //                    }));

        //                CreateLocationRequest();
        //            }
        //            else
        //            {
                        
        //            }
        //        })).Check();
        //}

        //protected void CreateLocationRequest()
        //{
        //    try
        //    {
        //        locationRequest = LocationRequest.Create();
        //        locationRequest.SetInterval(10000);
        //        locationRequest.SetFastestInterval(50000);
        //        locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);

        //        var builder = new LocationSettingsRequest.Builder()
        //                        .AddLocationRequest(locationRequest);

        //        var client = LocationServices.GetSettingsClient(this);
        //        var task = client.CheckLocationSettings(builder.Build());
        //        task.AddOnCompleteListener(new OncompleteListener((t) =>
        //        {
        //            if (!t.IsSuccessful)
        //                throw t.Exception;
        //        }));
        //    }
        //    catch (Exception e)
        //    {
        //        if(e is ResolvableApiException)
        //        {
        //            try
        //            {
        //                var resolvable = (ResolvableApiException)e;
        //                resolvable.StartResolutionForResult(this, REQUEST_CHECK_SETTINGS);
        //            }
        //            catch (IntentSender.SendIntentException sendEx)
        //            {

        //            }
        //        } 
        //    }
        //}

        //private void StartLocationUpdates()
        //{
        //    fusedLocationClient.RequestLocationUpdates(locationRequest, locationCallback, MainLooper);
        //}

        //private async void StopLocationUpdates()
        //{
        //    await fusedLocationClient.RemoveLocationUpdatesAsync(locationCallback);
        //}

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
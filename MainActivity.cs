using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.App;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;
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
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {
        private List<Post> ListOfPost;
        private RecyclerView mainRecycler;
        private PostAdapter postAdapter;
        private CreatePostFragment createPost = new CreatePostFragment();
        private RecyclerViewEmptyObserver emptyObserver;

        private PostEventListener postEventListener;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            FetchPost();
            SetUpRecycler();
            GetControls();
        }


        private void SetUpRecycler()
        {
            mainRecycler = FindViewById<RecyclerView>(Resource.Id.main_recycler);
            var emptyRoot = FindViewById<ConstraintLayout>(Resource.Id.rv_empty_view);
            postAdapter = new PostAdapter(ListOfPost);
            mainRecycler.SetAdapter(postAdapter);

            emptyObserver = new RecyclerViewEmptyObserver(mainRecycler, emptyRoot);
            postAdapter.RegisterAdapterDataObserver(emptyObserver);

            postAdapter.ItemLongClick += PostAdapter_ItemLongClick;
            postAdapter.LikeClick += PostAdapter_LikeClick;
            postAdapter.ImageClick += PostAdapter_ImageClick;
        }

        private void PostAdapter_ImageClick(object sender, PostAdapter.ImageClickEventArgs e)
        {
            var intent = new Intent(this, typeof(FullscreenImageActivity));

            PostParcelable postParcelable = new PostParcelable();
            postParcelable.PostItem = ListOfPost[e.Position];

            intent.PutExtra("extra_transition_name", ViewCompat.GetTransitionName(e.PostImageView));
            intent.PutExtra("extra_post_data", postParcelable);
            var options = ActivityOptionsCompat.MakeSceneTransitionAnimation(this, e.PostImageView, 
                ViewCompat.GetTransitionName(e.PostImageView));
            StartActivity(intent, options.ToBundle());
        }

        private void GetControls()
        {
            var addPostFab = FindViewById<FloatingActionButton>(Resource.Id.post_fab);
            var mainAppBar = FindViewById<AppBarLayout>(Resource.Id.activity_main_appbar);
            var mainToolbar = mainAppBar.FindViewById<Toolbar>(Resource.Id.main_toolbar);
            
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
                 where post.Author.ToLower().Contains(e.NewText.ToLower())
                    || post.PostBody.ToLower().Contains(e.NewText.ToLower())
                 select post).ToList();

            postAdapter = new PostAdapter(searchResult);
            postAdapter.RegisterAdapterDataObserver(emptyObserver);
            mainRecycler.SetAdapter(postAdapter);

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_refresh:
                    break;

                case Resource.Id.action_settings:
                    
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        

        private void PostAdapter_LikeClick(object sender, PostAdapter.PostAdapterClickEventArgs e)
        {
            Post post = ListOfPost[e.Position];
            //LikeEventListener likeEventListener = new LikeEventListener(post.ID);

            if (!post.Liked)
            {
                //likeEventListener.LikePost();
                post.Liked = true;
            }
            else
            {
                //likeEventListener.UnlikePost();
                post.Liked = false;
            }
        }

        private void PostAdapter_ItemLongClick(object sender, PostAdapter.PostAdapterClickEventArgs e)
        {
            string postID = ListOfPost[e.Position].ID;
            string ownerID = ListOfPost[e.Position].OwnerId;

            if (SessionManager.GetFirebaseAuth().CurrentUser.Uid == ownerID)
            {
                var alert = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
                alert.SetTitle("Edit or Delete Post");
                alert.SetMessage("Are you sure");

                // Edit Post on Firestore
                alert.SetNegativeButton("Edit Post", (o, args) =>
                {
                    //EditPostFragment editPostFragment = new EditPostFragment(ListOfPost[e.Position]);
                    //var trans = SupportFragmentManager.BeginTransaction();
                    //editPostFragment.Show(trans, "edit");

                });

                // Delete Post from Firestore and Storage
                alert.SetPositiveButton("Delete", (o, args) =>
                {
                    //SessionManager.GetFirestore().Collection("posts").Document(postID).Delete();
                    //StorageReference storageReference = FirebaseStorage.Instance.GetReference("postImages/" + postID);
                    //storageReference.Delete();
                });

                alert.Show();
            }
        }

        private void FetchPost()
        {
            //postEventListener = new PostEventListener();
            //postEventListener.FetchPost();
            //postEventListener.OnPostRetrieved += PostEventListener_OnPostRetrieved;
        }

        private void PostEventListener_OnPostRetrieved(object sender, PostEventListener.PostEventArgs e)
        {
            ListOfPost = new List<Post>();
            ListOfPost = e.Posts;

            if (ListOfPost != null)
            {
                ListOfPost = ListOfPost.OrderByDescending(x => x.PostDate).ToList();
            }

            SetUpRecycler();
        }
    }
}
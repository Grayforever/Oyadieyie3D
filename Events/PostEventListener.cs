using Android.App;
using Android.Widget;
using Firebase.Database;
using Oyadieyie3D.HelperClasses;
using Oyadieyie3D.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Oyadieyie3D.Events
{
    public class PostEventListener : Java.Lang.Object, IValueEventListener
    {
        private List<Post> ListOfPost = new List<Post>();
        private DatabaseReference retrievalRef;
        public event EventHandler<PostEventArgs> OnPostRetrieved;

        public class PostEventArgs : EventArgs
        {
            public List<Post> Posts { get; set; }
        }

        public void FetchPost()
        {
            retrievalRef = SessionManager.GetFireDB().GetReference("posts");
            retrievalRef.AddValueEventListener(this);
            retrievalRef.KeepSynced(true);
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            switch (snapshot.Exists())
            {
                case false:
                    break;
                default:
                    ListOfPost.Clear();
                    foreach (var (item, post) in from item in snapshot.Children.ToEnumerable<DataSnapshot>()
                                                 let post = new Post()
                                                 select (item, post))
                    {
                        post.ID = item.Key;
                        post.PostBody = item.Child(Constants.SNAPSHOT_POST_BODY) != null ? item.Child(Constants.SNAPSHOT_POST_BODY).Value.ToString() : "";
                        post.ImageId = item.Child(Constants.SNAPSHOT_POST_IMAGE_ID) != null ? item.Child(Constants.SNAPSHOT_POST_IMAGE_ID).Value.ToString() : "";
                        var userID = item.Child(Constants.SNAPSHOT_POST_OWNER_ID) != null ? item.Child(Constants.SNAPSHOT_POST_OWNER_ID).Value.ToString() : "";
                        post.OwnerId = userID;
                        post.DownloadUrl = item.Child(Constants.SNAPSHOT_POST_DOWNLOAD_URL) != null ? item.Child(Constants.SNAPSHOT_POST_DOWNLOAD_URL).Value.ToString() : "";
                        post.PostDate = item.Child(Constants.SNAPSHOT_POST_POST_DATE) != null ? item.Child(Constants.SNAPSHOT_POST_POST_DATE).Value.ToString() : "";

                        if (item.Child("likes").Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Exists())
                        {
                            post.Liked = true;
                        }
                        else
                        {
                            post.Liked = false;
                        }
                        post.LikeCount = item.Child("likes").ChildrenCount;
                        var ownerRef = SessionManager.GetFireDB().GetReference($"users/{userID}/profile");
                        ownerRef.AddValueEventListener(new SingleValueListener(
                        onDataChange: (s) =>
                        {
                            if (!s.Exists())
                                return;
                            post.Author = s.Child(Constants.SNAPSHOT_FNAME) != null ? s.Child(Constants.SNAPSHOT_FNAME).Value.ToString() : "";
                            post.OwnerImg = s.Child(Constants.SNAPSHOT_PHOTO_URL) != null ? s.Child(Constants.SNAPSHOT_PHOTO_URL).Value.ToString() : "";
                            
                        },
                        onCancelled: (e)=> 
                        {
                            Toast.MakeText(Application.Context, e.Message, ToastLength.Short).Show();
                        }));
                        ListOfPost.Add(post);
                    }
                    OnPostRetrieved?.Invoke(this, new PostEventArgs { Posts = ListOfPost });
                    break;
            }
        }
    }
}
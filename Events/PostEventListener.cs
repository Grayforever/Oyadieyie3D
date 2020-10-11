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
                    //ListOfPost.Clear();
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
                        post.Liked = item.Child("likes").Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Exists() ? true : false;
                        post.LikeCount = item.Child("likes") != null ? item.Child("likes").ChildrenCount : 0;
                        ListOfPost.Add(post);
                    }
                    OnPostRetrieved?.Invoke(this, new PostEventArgs { Posts = ListOfPost });
                    break;
            }
        }
    }
}
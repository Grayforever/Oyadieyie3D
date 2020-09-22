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
        public List<Post> ListOfPost = new List<Post>();
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
            if (!snapshot.Exists())
                return;

            if (ListOfPost.Count > 0)
            {
                ListOfPost.Clear();
            }

            foreach (var (item, post) in from item in snapshot.Children.ToEnumerable<DataSnapshot>()
                                         let post = new Post()
                                         select (item, post))
            {
                post.ID = item.Key;
                post.PostBody = item.Child("post_body") != null ? item.Child("post_body").Value.ToString() : "";
                post.ImageId = item.Child("image_id") != null ? item.Child("image_id").Value.ToString() : "";
                post.OwnerId = item.Child("owner_id") != null ? item.Child("owner_id").Value.ToString() : "";
                post.DownloadUrl = item.Child("download_url") != null ? item.Child("download_url").Value.ToString() : "";
                string datestring = item.Child("post_date") != null ? item.Child("post_date").Value.ToString() : "";
                post.PostDate = DateTime.Parse(datestring);
                ListOfPost.Add(post);
            }
            OnPostRetrieved?.Invoke(this, new PostEventArgs { Posts = ListOfPost });
        }
    }
}
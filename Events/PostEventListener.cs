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
                        post.PostBody = item.Child("post_body") != null ? item.Child("post_body").Value.ToString() : "";
                        post.ImageId = item.Child("image_id") != null ? item.Child("image_id").Value.ToString() : "";
                        var userID = item.Child("owner_id") != null ? item.Child("owner_id").Value.ToString() : "";
                        post.OwnerId = userID;
                        post.DownloadUrl = item.Child("download_url") != null ? item.Child("download_url").Value.ToString() : "";
                        string datestring = item.Child("post_date") != null ? item.Child("post_date").Value.ToString() : "";
                        post.PostDate = DateTime.Parse(datestring);
                        var ownerRef = SessionManager.GetFireDB().GetReference($"users/{userID}/profile");
                        ownerRef.AddValueEventListener(new SingleValueListener(
                        onDataChange: (s) =>
                        {
                            if (!s.Exists())
                                return;
                            post.Author = s.Child("fname") != null ? s.Child("fname").Value.ToString() : "";
                            post.OwnerImg = s.Child("photoUrl") != null ? s.Child("photoUrl").Value.ToString() : "";
                            
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
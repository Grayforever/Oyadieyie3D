using Android.Runtime;
//using Firebase.Firestore;
using Oyadieyie3D.HelperClasses;
using Oyadieyie3D.Models;
using System;
using System.Collections.Generic;

namespace Oyadieyie3D.Events
{
    public class PostEventListener : Java.Lang.Object /*IEventListener*/
    {
        public List<Post> ListOfPost = new List<Post>();
        public event EventHandler<PostEventArgs> OnPostRetrieved;

        public class PostEventArgs : EventArgs
        {
            public List<Post> Posts { get; set; }
        }

        //public void FetchPost()
        //{
        //    SessionManager.GetFirestore().Collection("posts").AddSnapshotListener(this);
        //}

        //public void RemoveListener()
        //{
        //    var listener = SessionManager.GetFirestore().Collection("posts").AddSnapshotListener(this);
        //    listener.Remove();
        //}

        //void OrganizeData(Java.Lang.Object Value)
        //{
        //    var snapshot = (QuerySnapshot)Value;
        //    if (snapshot.IsEmpty)
        //    {
        //        return;
        //    }
        //    if (ListOfPost.Count > 0)
        //    {
        //        ListOfPost.Clear();
        //    }

        //    foreach (DocumentSnapshot item in snapshot.Documents)
        //    {
        //        Post post = new Post();
        //        post.ID = item.Id;

        //        post.PostBody = item.Get("post_body") != null ? item.Get("post_body").ToString() : "";
        //        post.Author = item.Get("author") != null ? item.Get("author").ToString() : "";
        //        post.ImageId = item.Get("image_id") != null ? item.Get("image_id").ToString() : "";
        //        post.OwnerId = item.Get("owner_id") != null ? item.Get("owner_id").ToString() : "";
        //        post.DownloadUrl = item.Get("download_url") != null ? item.Get("download_url").ToString() : "";
        //        string datestring = item.Get("post_date") != null ? item.Get("post_date").ToString() : "";
        //        post.PostDate = DateTime.Parse(datestring);


        //        var data = item.Get("likes") != null ? item.Get("likes") : null;
        //        if (data != null)
        //        {
        //            var dictionaryFromHashMap = new JavaDictionary<string, string>(data.Handle, JniHandleOwnership.DoNotRegister);
        //            string uid = SessionManager.GetFirebaseAuth().CurrentUser.Uid;

        //            post.LikeCount = dictionaryFromHashMap.Count;

        //            if (dictionaryFromHashMap.Contains(uid))
        //            {
        //                post.Liked = true;
        //            }
        //        }

        //        ListOfPost.Add(post);
        //    }

        //    OnPostRetrieved?.Invoke(this, new PostEventArgs { Posts = ListOfPost });
        //}

        //public void OnEvent(Java.Lang.Object obj, FirebaseFirestoreException error)
        //{
        //    OrganizeData(obj);
        //}
    }
}
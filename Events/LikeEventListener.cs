using Java.Util;
using Oyadieyie3D.HelperClasses;
using System;

namespace Oyadieyie3D.Events
{
    public class LikeEventListener
    { 

        public event EventHandler<LikePostEventArgs> OnLikePost;
        private string _postId;
        public class LikePostEventArgs : EventArgs
        {
            public bool isLiked { get; set; }
            public long likeCount { get; set; }
        }

        public LikeEventListener(string postId)
        {
            _postId = postId;
        }

        public void LikePost()
        {
            try
            {
                var likeRef = SessionManager.GetFireDB().GetReference($"posts/{_postId}/likes");
                likeRef.AddValueEventListener(new SingleValueListener((s) =>
                {
                    bool liked = false;
                    if (!s.Exists())
                        return;
                    if (s.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Exists())
                    {
                        likeRef.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).RemoveValueAsync();
                        liked = false;
                    }
                    else
                    {
                        var likeMap = new HashMap();
                        likeMap.Put(SessionManager.GetFirebaseAuth().CurrentUser.Uid, DateTime.UtcNow.ToString());
                        likeRef.SetValueAsync(likeMap);
                        liked = true;
                    }

                    OnLikePost?.Invoke(this, new LikePostEventArgs { isLiked = liked, likeCount = s.ChildrenCount});
    
                }, (e) =>
                {

                }));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
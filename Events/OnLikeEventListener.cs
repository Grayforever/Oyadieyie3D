using Like.Lib;
using System;

namespace Oyadieyie3D.Events
{
    internal sealed class OnLikeEventListener : Java.Lang.Object, IOnLikeListener
    {
        private Action<LikeButton> _onLiked;
        private Action<LikeButton> _onUnLiked;
        public OnLikeEventListener(Action<LikeButton> onLiked, Action<LikeButton> onUnLiked)
        {
            _onLiked = onLiked;
            _onUnLiked = onUnLiked;
        }

        public void Liked(LikeButton likeButton) => _onLiked?.Invoke(likeButton);

        public void UnLiked(LikeButton likeButton) => _onUnLiked?.Invoke(likeButton);
    }
}
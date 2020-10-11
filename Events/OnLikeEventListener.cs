using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Like.Lib;

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
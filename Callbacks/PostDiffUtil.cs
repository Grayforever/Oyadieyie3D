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
using AndroidX.Annotations;
using AndroidX.RecyclerView.Widget;
using Oyadieyie3D.Models;

namespace Oyadieyie3D.Callbacks
{
    public class PostDiffUtil : DiffUtil.Callback
    {
        private List<Post> mOldPostList;
        private List<Post> mNewPostList;
        public PostDiffUtil(List<Post> mOldPostList, List<Post> mNewPostList)
        {
            this.mOldPostList = mOldPostList;
            this.mNewPostList = mNewPostList;
        }
        public override int NewListSize => mNewPostList.Count;

        public override int OldListSize => mOldPostList != null ? mOldPostList.Count : 0;

        public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
        {
            Post oldPost = mOldPostList[oldItemPosition];
            Post newPost = mOldPostList[newItemPosition];

            return oldPost.ID.Equals(newPost.ID);
        }

        public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
        {
            return mOldPostList[oldItemPosition].ID == mNewPostList[newItemPosition].ID;
        }

        [Nullable]
        public override Java.Lang.Object GetChangePayload(int oldItemPosition, int newItemPosition)
        {
            return base.GetChangePayload(oldItemPosition, newItemPosition);
        }
    }
}
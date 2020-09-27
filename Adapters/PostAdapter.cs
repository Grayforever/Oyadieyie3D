using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;
using BumpTech.GlideLib;
using BumpTech.GlideLib.Requests;
using DE.Hdodenhof.CircleImageViewLib;
using Oyadieyie3D.Models;
using System;
using System.Collections.Generic;

namespace Oyadieyie3D.Adapters
{
    public class PostAdapter : RecyclerView.Adapter
    {
        public event EventHandler<PostAdapterClickEventArgs> ItemClick;
        public event EventHandler<PostAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<PostAdapterClickEventArgs> LikeClick;
        List<Post> _items;
        public static Post item;

        public PostAdapter(List<Post> items)
        {
            _items = items;
            NotifyDataSetChanged();
        }
        public override int ItemCount => _items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as PostAdapterViewHolder;
            item = _items[position];
            var date = DateTime.UtcNow;
            vh.usernameTextView.Text = item.Author;
            vh.postBodyTextView.Text = item.PostBody;
            vh.likeCountTextView.Text = item.LikeCount.ToString() + " Likes";
            vh.durationTextView.Text = date.ToString("HH:mm");
            
            GetImage(item.DownloadUrl, vh.postImageView);
            GetImage(item.OwnerImg, vh.profileImageView);
            ViewCompat.SetTransitionName(vh.postImageView, "open_gate");
        }

        private void GetImage(string downloadUrl, ImageView postImageView)
        {
            RequestOptions requestOptions = new RequestOptions();
            requestOptions.Placeholder(Resource.Drawable.img_placeholder);
            requestOptions.SkipMemoryCache(true);

            Glide.With(Application.Context)
                .SetDefaultRequestOptions(requestOptions)
                .Load(downloadUrl)
                .Into(postImageView);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.post_item, parent, false);
            var vh = new PostAdapterViewHolder(itemView, OnClick, OnLongClick, OnLike);

            return vh;
        }

        void OnClick(PostAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(PostAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void OnLike(PostAdapterClickEventArgs args) => LikeClick?.Invoke(this, args);

        public class PostAdapterViewHolder : RecyclerView.ViewHolder
        {
            //public TextView TextView { get; set; }

            public TextView usernameTextView { get; set; }
            public TextView postBodyTextView { get; set; }
            public TextView likeCountTextView { get; set; }
            public ImageView postImageView { get; set; }

            public TextView durationTextView { get; set; }

            public CircleImageView profileImageView { get; set; }

            public PostAdapterViewHolder(View itemView, Action<PostAdapterClickEventArgs> clickListener,
                                Action<PostAdapterClickEventArgs> longClickListener, Action<PostAdapterClickEventArgs> likeClickListener) : base(itemView)
            {
                usernameTextView = (TextView)itemView.FindViewById(Resource.Id.post_name_tv);
                postBodyTextView = (TextView)itemView.FindViewById(Resource.Id.post_caption_tv);
                likeCountTextView = (TextView)itemView.FindViewById(Resource.Id.post_like_count_tv);
                postImageView = (ImageView)itemView.FindViewById(Resource.Id.post_img_iv);
                profileImageView = (CircleImageView)itemView.FindViewById(Resource.Id.post_user_profile);
                durationTextView = (TextView)itemView.FindViewById(Resource.Id.post_time_tv);

                itemView.Click += (sender, e) => clickListener(new PostAdapterClickEventArgs { View = itemView, Position = AdapterPosition, ImageView = postImageView });
                itemView.LongClick += (sender, e) => longClickListener(new PostAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                
            }
        }

        public class PostAdapterClickEventArgs : EventArgs
        {
            public View View { get; set; }
            public int Position { get; set; }

            public ImageView ImageView { get; set; }
        }
    }
}
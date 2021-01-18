using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using DE.Hdodenhof.CircleImageViewLib;
using Oyadieyie3D.Callbacks;
using Oyadieyie3D.Models;
using System;
using System.Collections.Generic;

namespace Oyadieyie3D.Adapters
{
    public class PostAdapter : RecyclerView.Adapter
    {
        public event EventHandler<PostAdapterClickEventArgs> ItemClick;
        public event EventHandler<PostAdapterClickEventArgs> ItemLongClick;

        public static List<Post> _items { get; set; }
        public Post item;
        private RequestOptions requestOptions;

        public PostAdapter(List<Post> items)
        {
            _items = items;
        }

        public void UpdateData(List<Post> newItems)
        {
            var postdiffCallback = new PostDiffUtil(_items, newItems);
            var diffResult = DiffUtil.CalculateDiff(postdiffCallback);
            _items.Clear();
            _items = newItems;
            diffResult.DispatchUpdatesTo(this);
        }

        public override int ItemCount => _items != null ? _items.Count : 0;

        public override long GetItemId(int position) => position;

        public override int GetItemViewType(int position) => position;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as PostAdapterViewHolder;

            item = _items[position];
            ViewCompat.SetTransitionName(vh.postImageView, "open_gate");
            vh.usernameTextView.Text = item.Author;
            vh.postBodyTextView.Text = item.PostBody;
            var ts = DateTime.UtcNow - DateTime.Parse(item.PostDate);
            vh.durationTextView.Text = ts.ToString(@"d\d\ hh\h\ mm\m\ ss\s").TrimStart(' ', 'd', 'h', 'm', 's', '0');
            vh.likeCountTextView.Text = string.Format("{0} like{1}", item.LikeCount, item.LikeCount > 1 || item.LikeCount == 0 ? "s" : string.Empty);
            GetImage(item.DownloadUrl, vh.postImageView);

            //vh.postLikeBtn.SetOnLikeListener(new OnLikeEventListener((liked) =>
            //{
            //    var likeref = SessionManager.GetFireDB().GetReference($"posts/{item.ID}/likes");
            //    likeref.Child(SessionManager.UserId).SetValue("true");
            //}, (unliked) =>
            //{
            //    var unlikeref = SessionManager.GetFireDB().GetReference($"posts/{item.ID}/likes");
            //    unlikeref.AddListenerForSingleValueEvent(new SingleValueListener(
            //        (s)=> 
            //        {
            //            if (!s.Exists())
            //                return;

            //            unlikeref.Child(SessionManager.UserId).RemoveValue();
            //            NotifyDataSetChanged();
            //        }, (e)=> 
            //        {
            //            unlikeref.Child(SessionManager.UserId).RemoveValue();
            //            NotifyDataSetChanged();
            //        }));
            //}));
        }

        private void GetImage(string downloadUrl, ImageView postImageView)
        {
            requestOptions = new RequestOptions();
            requestOptions.Placeholder(Resource.Drawable.img_placeholder);

            Glide.With(Application.Context)
                .SetDefaultRequestOptions(requestOptions)
                .Load(downloadUrl)
                .Into(postImageView);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.post_item, parent, false);
            return new PostAdapterViewHolder(itemView, OnClick, OnLongClick);
        }

        void OnClick(PostAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(PostAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        public class PostAdapterViewHolder : RecyclerView.ViewHolder
        {
            public TextView usernameTextView { get; set; }
            public TextView postBodyTextView { get; set; }
            public TextView likeCountTextView { get; set; }
            public ImageView postImageView { get; set; }
            public TextView durationTextView { get; set; }
            public CircleImageView profileImageView { get; set; }

            public PostAdapterViewHolder(View itemView, Action<PostAdapterClickEventArgs> clickListener,
                Action<PostAdapterClickEventArgs> longClickListener) : base(itemView)
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
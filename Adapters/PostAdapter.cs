using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;
using BumpTech.GlideLib;
using BumpTech.GlideLib.Requests;
using DE.Hdodenhof.CircleImageViewLib;
using Firebase.Database;
using Like.Lib;
using Oyadieyie3D.Callbacks;
using Oyadieyie3D.Events;
using Oyadieyie3D.HelperClasses;
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
        public static Post item;
        public static Context _context;
        private RequestOptions requestOptions;

        public PostAdapter(List<Post> items)
        {
            _items = items;
        }

        public void SetData(List<Post> items)
        {
            var postDiffUtil = new PostDiffUtil(_items, items);
            var diffResult = DiffUtil.CalculateDiff(postDiffUtil);

            _items.Clear();
            _items = items;
            diffResult.DispatchUpdatesTo(this);
        }

        public override int ItemCount => _items != null ? _items.Count : 0;

        public override long GetItemId(int position)
        {
            return position;
        }


        public override int GetItemViewType(int position)
        {
            return position;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as PostAdapterViewHolder;

            item = _items[position];
            vh.usernameTextView.Text = item.Author;
            vh.postBodyTextView.Text = item.PostBody;
            var ts = DateTime.UtcNow - DateTime.Parse(item.PostDate);
            vh.durationTextView.Text = ts.ToString(@"d\d\ hh\h\ mm\m\ ss\s").TrimStart(' ', 'd', 'h', 'm', 's', '0');
            vh.postLikeBtn.SetLiked(item.Liked);
            vh.likeCountTextView.Text = string.Format("{0} like{1}", item.LikeCount, item.LikeCount > 1 || item.LikeCount == 0 ? "s" : string.Empty);
            GetImage(item.DownloadUrl, vh.postImageView);
            ViewCompat.SetTransitionName(vh.postImageView, "open_gate");
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

        public class PostAdapterViewHolder : RecyclerView.ViewHolder, IValueEventListener
        {
            private DatabaseReference unlikeref;

            public TextView usernameTextView { get; set; }
            public TextView postBodyTextView { get; set; }
            public TextView likeCountTextView { get; set; }
            public ImageView postImageView { get; set; }
            public TextView durationTextView { get; set; }
            public CircleImageView profileImageView { get; set; }
            public LikeButton postLikeBtn { get; set; }

            public PostAdapterViewHolder(View itemView, Action<PostAdapterClickEventArgs> clickListener,
                Action<PostAdapterClickEventArgs> longClickListener) : base(itemView)
            {
                usernameTextView = (TextView)itemView.FindViewById(Resource.Id.post_name_tv);
                postBodyTextView = (TextView)itemView.FindViewById(Resource.Id.post_caption_tv);
                likeCountTextView = (TextView)itemView.FindViewById(Resource.Id.post_like_count_tv);
                postImageView = (ImageView)itemView.FindViewById(Resource.Id.post_img_iv);
                profileImageView = (CircleImageView)itemView.FindViewById(Resource.Id.post_user_profile);
                durationTextView = (TextView)itemView.FindViewById(Resource.Id.post_time_tv);
                postLikeBtn = itemView.FindViewById<LikeButton>(Resource.Id.post_like_btn);

                itemView.Click += (sender, e) => clickListener(new PostAdapterClickEventArgs { View = itemView, Position = AdapterPosition, ImageView = postImageView });
                itemView.LongClick += (sender, e) => longClickListener(new PostAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                

                postLikeBtn.SetOnLikeListener(new OnLikeEventListener((liked)=> 
                {
                    var likeref = SessionManager.GetFireDB().GetReference($"posts/{_items[AdapterPosition].ID}/likes");
                    likeref.Child(SessionManager.UserId).SetValue("true");

                },(unliked)=> 
                {
                    unlikeref = SessionManager.GetFireDB().GetReference($"posts/{_items[AdapterPosition].ID}/likes");
                    unlikeref.AddListenerForSingleValueEvent(this);
                } ));
            }

            public void OnCancelled(DatabaseError error)
            {
                unlikeref.Child(SessionManager.UserId).RemoveValue();
            }

            public void OnDataChange(DataSnapshot snapshot)
            {
                if (!snapshot.Exists())
                    return;

                unlikeref.Child(SessionManager.UserId).RemoveValue();
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
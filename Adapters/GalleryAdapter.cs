using Android.Views;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Google.Android.Material.ImageView;
using Oyadieyie3D.Models;
using System;
using System.Collections.Generic;
using R = Oyadieyie3D.Resource;

namespace Oyadieyie3D.Adapters
{
    class GalleryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<GalleryAdapterClickEventArgs> ItemClick;
        public event EventHandler<GalleryAdapterClickEventArgs> ItemLongClick;
        List<ImageMetadata> items;

        public GalleryAdapter(List<ImageMetadata> data)
        {
            items = data;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context).Inflate(R.Layout.gallery_item, parent, false);
            var vh = new GalleryAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];
            var holder = viewHolder as GalleryAdapterViewHolder;
            SetImageFromUrl(holder.ImageView, item.Url);
        }

        private void SetImageFromUrl(ShapeableImageView imageView, string url)
        {
            Glide.With(imageView.Context)
                .Load(url)
                .Into(imageView);
        }

        public override int ItemCount => items.Count;

        void OnClick(GalleryAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(GalleryAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class GalleryAdapterViewHolder : RecyclerView.ViewHolder
    {
        public ShapeableImageView ImageView { get; set; }
        public GalleryAdapterViewHolder(View itemView, Action<GalleryAdapterClickEventArgs> clickListener,
                            Action<GalleryAdapterClickEventArgs> longClickListener) : base(itemView)
        {

            ImageView = itemView.FindViewById<ShapeableImageView>(R.Id.gallery_imageView);
            itemView.Click += (sender, e) => clickListener(new GalleryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new GalleryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class GalleryAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Google.Android.Material.Chip;
using Google.Android.Material.ImageView;
using Oyadieyie3D.Models;
using System;
using System.Collections.Generic;
using R = Oyadieyie3D.Resource;

namespace Oyadieyie3D.Adapters
{
    class SearchAdapter : RecyclerView.Adapter
    {
        public event EventHandler<SearchAdapterClickEventArgs> ItemClick;
        public event EventHandler<SearchAdapterClickEventArgs> ItemLongClick;
        List<ClientResult> _items;

        public SearchAdapter(List<ClientResult> items)
        {
            _items = items;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = R.Layout.search_item;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new SearchAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var holder = viewHolder as SearchAdapterViewHolder;
            var item = _items[position];

            holder.ClientName.Text = item.ClientName;
            holder.ClientLocation.Text = item.Location;
            holder.ClientFavCount.Text = $"{item.FavCount} k";
            holder.ClientRating.Text = item.Rating.ToString();
            holder.ClientOpeningHours.Text = item.OpeningHours;
            holder.ClientDistanceAway.Text = $"{item.DistantFromCustomer} m";

            SetBannerImage(holder.BannerImage, item.BannerUrl);
        }

        private void SetBannerImage(ShapeableImageView bannerImage, string bannerUrl)
        {
            Glide.With(bannerImage.Context)
                .Load(bannerUrl)
                .Into(bannerImage);
        }

        public override int ItemCount => _items.Count;

        void OnClick(SearchAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(SearchAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class SearchAdapterViewHolder : RecyclerView.ViewHolder
    {
        public ShapeableImageView BannerImage { get; set; }
        public TextView ClientName { get; set; }
        public TextView ClientLocation { get; set; }
        public TextView ClientOpeningHours { get; set; }
        public TextView ClientFavCount { get; set; }
        public Chip ClientRating { get; set; }
        public TextView ClientDistanceAway { get; set; }

        public SearchAdapterViewHolder(View itemView, Action<SearchAdapterClickEventArgs> clickListener,
                            Action<SearchAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            BannerImage = itemView.FindViewById<ShapeableImageView>(R.Id.banner_iv);
            ClientName = itemView.FindViewById<TextView>(R.Id.client_name_tv);
            ClientLocation = itemView.FindViewById<TextView>(R.Id.client_location_tv);
            ClientOpeningHours = itemView.FindViewById<TextView>(R.Id.open_label);
            ClientFavCount = itemView.FindViewById<TextView>(R.Id.favs_tv);
            ClientRating = itemView.FindViewById<Chip>(R.Id.rating_chip);
            ClientDistanceAway = itemView.FindViewById<TextView>(R.Id.meters_away_tv);

            itemView.Click += (sender, e) => clickListener(new SearchAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new SearchAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class SearchAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}
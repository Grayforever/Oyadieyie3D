using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using DE.Hdodenhof.CircleImageViewLib;
using Oyadieyie3D.Models;
using System.Collections.Generic;

namespace Oyadieyie3D.Adapters
{
    public class TailorFinderRAdapter : RecyclerView.Adapter
    {

        private Context _context;
        private List<Client> _clientList;

        public TailorFinderRAdapter(Context context, List<Client> clientList)
        {
            _context = context;
            _clientList = clientList;
        }

        public override int ItemCount => _clientList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as TailorFinderViewHolder;
            var client = _clientList[position];

            vh.TNameTv.Text = client.ClientName;
            vh.TDescTv.Text = client.ItemDescription;
            vh.TLocation.Text = client.OpeningHours;
            vh.TRatingBar.Rating = 3.4f;

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.tailor_item, parent, false);
            return new TailorFinderViewHolder(itemView);
        }


        internal sealed class TailorFinderViewHolder : RecyclerView.ViewHolder
        {
            public CircleImageView TImageView { get; set; }
            public TextView TNameTv { get; set; }
            public TextView TDescTv { get; set; }
            public TextView TLocation { get; set; }
            public TextView TPaymentMethodTv { get; set; }
            public RatingBar TRatingBar { get; set; }


            public TailorFinderViewHolder(View itemView) : base(itemView)
            {
                TImageView = itemView.FindViewById<CircleImageView>(Resource.Id.tcard_iview);
                TNameTv = itemView.FindViewById<TextView>(Resource.Id.tcard_tname);
                TDescTv = itemView.FindViewById<TextView>(Resource.Id.tcard_tdesc);
                TLocation = itemView.FindViewById<TextView>(Resource.Id.tcard_location);

                TRatingBar = itemView.FindViewById<RatingBar>(Resource.Id.tcard_tratingbar);
            }
        }
    }
}
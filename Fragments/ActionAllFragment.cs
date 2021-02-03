using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Adapters;
using Oyadieyie3D.Models;
using System.Collections.Generic;

namespace Oyadieyie3D.Fragments
{
    public class ActionAllFragment : AndroidX.Fragment.App.Fragment
    {
        private List<ClientResult> dummyList;

        public static ActionAllFragment Instance { get; private set; }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Instance = this;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.action_all, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var resultRecycler = view.FindViewById<RecyclerView>(Resource.Id.search_recyler);
            resultRecycler.SetLayoutManager(new LinearLayoutManager(Context));

            dummyList = new List<ClientResult>
            {
                new ClientResult{ ClientName = "Eos et", OpeningHours="Mon-Fri", DistantFromCustomer = 5.4, FavCount = 1.2,
                    Location="Consetetur sadipscing sit et ea", Rating = 4.3, BannerUrl = "https://www.gstatic.com/webp/gallery/1.webp"},
                new ClientResult{ ClientName = "Vel augue", OpeningHours="Mon-Fri", DistantFromCustomer = 10, FavCount = 1.5,
                    Location="Takimata kasd eirmod accusam", Rating = 3.2, BannerUrl = "https://www.gstatic.com/webp/gallery/5.webp"},
                new ClientResult { ClientName = "Magna et", OpeningHours = "Mon-Fri", DistantFromCustomer = 10, FavCount = 1.5,
                    Location = "Takimata kasd eirmod accusam", Rating = 3.2, BannerUrl = "https://www.gstatic.com/webp/gallery/4.webp" },
                new ClientResult { ClientName = "Ipsum qui dolor", OpeningHours = "Mon-Fri", DistantFromCustomer = 10, FavCount = 1.5,
                    Location = "Praesent zzril eirmod", Rating = 3.2, BannerUrl = "https://www.gstatic.com/webp/gallery/2.webp" },
                new ClientResult { ClientName = "Vel augue", OpeningHours = "Mon-Fri", DistantFromCustomer = 10, FavCount = 1.5,
                    Location = "Dansoman", Rating = 4.5, BannerUrl = "https://www.gstatic.com/webp/gallery/1.webp" }
            };

            GetData();

            var searchAdapter = new SearchAdapter(dummyList);
            resultRecycler.SetAdapter(searchAdapter);
            searchAdapter.ItemClick += (s1, e1) =>
            {
                e1.View.Post(() =>
                {
                    var intent = new Intent(Activity, typeof(FeaturedDetailsActivity));
                    intent.PutExtra(FeaturedDetailsActivity.BANNER_URL, dummyList[e1.Position].BannerUrl);

                    var options = ActivityOptions.MakeSceneTransitionAnimation(Activity, e1.View, FeaturedDetailsActivity.SHARED_TRANS_NAME);
                    StartActivity(intent, options.ToBundle());
                });
            };
        }

        public List<ClientResult> GetData()
            => dummyList;
    }
}
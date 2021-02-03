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
    public class ActionNearbyFragment : AndroidX.Fragment.App.Fragment
    {
        private List<ClientResult> clientList;
        private SearchAdapter searchAdapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.action_nearby, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var nearbyRecycler = view.FindViewById<RecyclerView>(Resource.Id.nearby_recyler);
            nearbyRecycler.SetLayoutManager(new LinearLayoutManager(Context));

            clientList = ActionAllFragment.Instance.GetData().FindAll(x => x.DistantFromCustomer >= 0.54);

            searchAdapter = new SearchAdapter(clientList);
            nearbyRecycler.SetAdapter(searchAdapter);
            searchAdapter.ItemClick += (s1, e1) =>
            {
                e1.View.Post(() =>
                {
                    var intent = new Intent(Activity, typeof(FeaturedDetailsActivity));
                    intent.PutExtra(FeaturedDetailsActivity.BANNER_URL, clientList[e1.Position].BannerUrl);

                    var options = ActivityOptions.MakeSceneTransitionAnimation(Activity, e1.View, FeaturedDetailsActivity.SHARED_TRANS_NAME);
                    StartActivity(intent, options.ToBundle());
                });
            };
        }
    }
}
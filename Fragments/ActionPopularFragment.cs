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
    public class ActionPopularFragment : AndroidX.Fragment.App.Fragment
    {
        private SearchAdapter searchAdapter;
        private List<ClientResult> clientList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.action_popular, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var popularRecycler = view.FindViewById<RecyclerView>(Resource.Id.popular_recyler);
            popularRecycler.SetLayoutManager(new LinearLayoutManager(Context));

            clientList = ActionAllFragment.Instance.GetData().FindAll(x => x.Rating >= 4.0);

            searchAdapter = new SearchAdapter(clientList);
            popularRecycler.SetAdapter(searchAdapter);
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
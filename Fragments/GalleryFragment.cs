using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.Core.App;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Adapters;
using Oyadieyie3D.Models;
using System.Collections.Generic;

namespace Oyadieyie3D.Fragments
{
    public class GalleryFragment : AndroidX.Fragment.App.Fragment
    {

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.gallery_frag, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var recycler = view.FindViewById<RecyclerView>(Resource.Id.gallery_view_recycler);
            //List<ImageMetadata> list = new List<ImageMetadata>();
            //list.Add(new ImageMetadata { Id = 1, Url = "https://www.gstatic.com/webp/gallery/5.webp" });
            //list.Add(new ImageMetadata { Id = 2, Url = "https://www.gstatic.com/webp/gallery/4.webp" });
            //list.Add(new ImageMetadata { Id = 3, Url = "https://www.gstatic.com/webp/gallery/3.webp" });
            //list.Add(new ImageMetadata { Id = 4, Url = "https://www.gstatic.com/webp/gallery/2.webp" });
            //list.Add(new ImageMetadata { Id = 5, Url = "https://www.gstatic.com/webp/gallery/1.webp" });
            //var adapter = new GalleryAdapter(list);
            //recycler.SetLayoutManager(new GridLayoutManager(Context, 3));
            //recycler.SetAdapter(adapter);

            //adapter.ItemClick += (s1, e1) =>
            //{
            //    var intent = new Intent(Activity, typeof(FullscreenImageActivity));
            //    intent.PutExtra("img_url", list[e1.Position].Url);
            //    intent.PutExtra(HelperClasses.Constants.TRANSITION_NAME, ViewCompat.GetTransitionName(e1.View));

            //    var options = ActivityOptionsCompat.MakeSceneTransitionAnimation(Activity, e1.View,
            //        ViewCompat.GetTransitionName(e1.View));
            //    StartActivity(intent, options.ToBundle());
            //};
        }
    }
}
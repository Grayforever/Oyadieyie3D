using Android.OS;
using Android.Views;
using Google.Android.Material.Button;

namespace Oyadieyie3D.Fragments
{
    public class Book2Fragment : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.book_page2, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var nextBtn = view.FindViewById<MaterialButton>(Resource.Id.book2_nextBtn);
            nextBtn.Click += (s1, e1) =>
            {
                BookingFragment.Instance.ShowPage(2);
            };

            var prevBtn = view.FindViewById<MaterialButton>(Resource.Id.book2_prevBtn);
            prevBtn.Click += (s1, e1) =>
            {
                BookingFragment.Instance.ShowPage(0);
            };
        }
    }
}
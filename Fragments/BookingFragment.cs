using Android.OS;
using Android.Views;
using AndroidX.ViewPager2.Widget;
using KofiGyan.StateProgressBarLib;
using Oyadieyie3D.Adapters;

namespace Oyadieyie3D.Fragments
{
    public class BookingFragment : AndroidX.Fragment.App.Fragment
    {
        private ViewPager2 bookingPager;
        private StateProgressBar stepProgressBar;
        public static BookingFragment Instance { get; set; }

        private PagerAdapter adapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Instance = this;

            adapter = new PagerAdapter(Activity);
            adapter.AddFragment(new Book1Fragment());
            adapter.AddFragment(new Book2Fragment());
            adapter.AddFragment(new Book3Fragment());
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.booking_frag, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            FindViewsWith(view);
        }

        private void FindViewsWith(View view)
        {
            bookingPager = view.FindViewById<ViewPager2>(Resource.Id.booking_pager);
            stepProgressBar = view.FindViewById<StateProgressBar>(Resource.Id.progress_stepper);
            
            bookingPager.Adapter = adapter;
        }

        public void ShowPage(int index)
        {
            bookingPager.SetCurrentItem(index, true);
            switch (index)
            {
                case 1:
                    stepProgressBar.SetCurrentStateNumber(StateProgressBar.StateNumber.Two);
                    break;
                case 2:
                    stepProgressBar.SetCurrentStateNumber(StateProgressBar.StateNumber.Three);
                    break;
                default:
                    stepProgressBar.SetCurrentStateNumber(StateProgressBar.StateNumber.One);
                    break;
            }
            
        }
    }
}
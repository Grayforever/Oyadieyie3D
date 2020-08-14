using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager2.Widget;
using Oyadieyie3D.Adapters;
using Oyadieyie3D.Fragments;
using System;

namespace Oyadieyie3D.Activities
{
    [Activity(Label ="@string/app_name", Theme ="@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, 
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation)]
    public class OnboardingActivity : AppCompatActivity
    {

        private OnboardingFragment onboardingFragment = new OnboardingFragment();
        private SignInFragment signInFragment = new SignInFragment();
        private RegisterFragment regFragment = new RegisterFragment();
        private PartnerFragment partnerFragment = new PartnerFragment();
        private ViewPager2 fragmentsViewPager;
        private bool smoothScroll = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.onboarding_activity);
            fragmentsViewPager = FindViewById<ViewPager2>(Resource.Id.onboarding_viewpager2);
            var fragmentsAdapter = new OnboardingAdapter(SupportFragmentManager, Lifecycle);

            fragmentsViewPager.UserInputEnabled = false;
            fragmentsViewPager.OffscreenPageLimit = 4;
            fragmentsViewPager.Orientation = ViewPager2.OrientationHorizontal;

            fragmentsAdapter.AddFragments(onboardingFragment);
            fragmentsAdapter.AddFragments(signInFragment);
            fragmentsAdapter.AddFragments(regFragment);
            fragmentsAdapter.AddFragments(partnerFragment);
            fragmentsViewPager.Adapter = fragmentsAdapter;

            SetParentFragment();
            SetEvents();
        }

        private void SetParentFragment()
        {
            fragmentsViewPager.SetCurrentItem(0, smoothScroll);
        }

        private void SetEvents()
        {
            onboardingFragment.OnRegisterBtnClick += OnboardingFragment_OnRegisterBtnClick;
            onboardingFragment.OnSignInBtnClick += OnboardingFragment_OnSignInBtnClick;
            regFragment.OnRegNextBtnClick += RegFragment_OnRegNextBtnClick;
        }

        private void RegFragment_OnRegNextBtnClick(object sender, EventArgs e)
        {
            fragmentsViewPager.SetCurrentItem(3, smoothScroll);
        }

        private void OnboardingFragment_OnSignInBtnClick(object sender, EventArgs e)
        {
            fragmentsViewPager.SetCurrentItem(1, smoothScroll);
        }

        private void OnboardingFragment_OnRegisterBtnClick(object sender, EventArgs e)
        {
            fragmentsViewPager.SetCurrentItem(2, smoothScroll);
        }

        public override void OnBackPressed()
        {
            switch (fragmentsViewPager.CurrentItem)
            {
                case 1:
                    fragmentsViewPager.SetCurrentItem(0, false);
                    break;
                case 2:
                    fragmentsViewPager.SetCurrentItem(0, false);
                    break;
                default:
                    base.OnBackPressed();
                    break;
            }
        }
    }
}
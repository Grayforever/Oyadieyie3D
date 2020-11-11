using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Oyadieyie3D.Events;
using Oyadieyie3D.Fragments;
using Ramotion.PaperOnboarding;
using System.Collections.Generic;

namespace Oyadieyie3D.Activities
{
    [Register("id.Oyadieyie3D.Activities.GetPremiumActivity")]
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class GetPremiumActivity : AppCompatActivity
    {
        private const int container = Resource.Id.premium_container;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.get_premium);
            // Create your application here
            InitWalkThrough();
        }

        private void SetStatusBarImmersiveMode()
        {
            Window win = Window;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                win.AddFlags(WindowManagerFlags.TranslucentStatus);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                WindowManagerLayoutParams wmlp = win.Attributes;
                wmlp.Flags &= ~WindowManagerFlags.TranslucentStatus;
                win.SetStatusBarColor(Color.Transparent);
            }
        }

        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            SetStatusBarImmersiveMode();
        }

        private void InitWalkThrough()
        {
            var ft = SupportFragmentManager.BeginTransaction();
            var onboardingFragment = PaperOnboardingFragment.NewInstance(new List<PaperOnboardingPage>
            {
                new PaperOnboardingPage("On top of the world", "Amet stet diam dolor erat lorem amet lorem et molestie", Color.ParseColor("#FFD54F"), Resource.Drawable.hotels, Resource.Drawable.key),
                new PaperOnboardingPage("Get featured","Nihil eos eos ea consequat consetetur et rebum elit no", Color.ParseColor("#4DB6AC"), Resource.Drawable.banks, Resource.Drawable.shopping_cart),
                new PaperOnboardingPage("Get noticed", "Lorem consetetur eum lorem ut accusam illum dolor esse est", Color.ParseColor("#9575CD"), Resource.Drawable.stores, Resource.Drawable.wallet)
            });

            onboardingFragment.SetOnRightOutListener(new OnRightOutListener(
                () =>
                {
                    Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                    var mainFt = SupportFragmentManager.BeginTransaction();
                    var premiumFrag = new PremiumFragment();
                    mainFt.Replace(container, premiumFrag);
                    mainFt.CommitAllowingStateLoss();
                }));

            ft.Add(container, onboardingFragment);
            ft.CommitAllowingStateLoss();
        }
    }
}
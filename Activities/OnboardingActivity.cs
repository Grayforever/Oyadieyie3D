using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Oyadieyie3D.Fragments;

namespace Oyadieyie3D.Activities
{
    [Activity(Label ="@string/app_name", Theme ="@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, 
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class OnboardingActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.onboarding_activity);

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.frag_container, new OnboardingFragment())
                .CommitAllowingStateLoss();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}
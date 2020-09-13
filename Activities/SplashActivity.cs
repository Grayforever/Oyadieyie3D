
using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Oyadieyie3D.HelperClasses;

namespace Oyadieyie3D.Activities
{
    [Activity(MainLauncher = true, Theme = "@style/AppTheme.Splash", Label ="@string/app_name", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout| Android.Content.PM.ConfigChanges.SmallestScreenSize| Android.Content.PM.ConfigChanges.Orientation)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }


        protected override void OnResume()
        {
            base.OnResume();
            switch (SessionManager.GetFirebaseAuth().CurrentUser)
            {
                case null:
                    StartActivity(typeof(OnboardingActivity));
                    break;
                default:
                    StartActivity(typeof(MainActivity));
                    break;
            }
            
        }
    }
}
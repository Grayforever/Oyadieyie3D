using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Auth;
using Oyadieyie3D.Events;
using Oyadieyie3D.HelperClasses;

namespace Oyadieyie3D.Activities
{
    [Activity(MainLauncher = true, Theme = "@style/AppTheme.Splash", Label ="@string/app_name", 
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, 
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout| Android.Content.PM.ConfigChanges.SmallestScreenSize| 
        Android.Content.PM.ConfigChanges.Orientation)]

    [MetaData("android.app.shortcuts", Resource ="@xml/shortcuts")]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            PreferenceHelper.Init(this);
            string firstRun = PreferenceHelper.Instance.GetString("firstRun", "");
            if (firstRun != "" && firstRun != "reg")
            {
                var i = new Intent(this, typeof(MainActivity));
                StartActivity(i);
                Finish();
            }
            else
            {
                PreferenceHelper.Instance.SetString("firstRun", "reg");
                RouteToAppropriatePage(SessionManager.GetFirebaseAuth().CurrentUser);
            }
        }

        private void RouteToAppropriatePage(FirebaseUser currentUser)
        {
            switch (currentUser)
            {
                case null:
                    StartOnboarding();
                    break;
                default:
                    CheckStatus(currentUser.Uid);
                    break;
            }
        }

        private void StartOnboarding()
        {
            var i = new Intent(this, typeof(OnboardingActivity));
            StartActivity(i);
            Finish();
        }

        private void CheckStatus(string uid)
        {
            var statusRef = SessionManager.GetFireDB().GetReference("session");
            statusRef.OrderByKey().EqualTo(uid).AddListenerForSingleValueEvent(new SingleValueListener((s) => 
            {
                if (!s.Child(uid).Exists())
                {
                    var i = new Intent(this, typeof(OnboardingActivity));
                    StartActivity(i);
                    Finish();
                }
                else
                {
                    string stage = s.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Child(Constants.SESION_CHILD) != null ? 
                    s.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Child(Constants.SESION_CHILD).Value.ToString() : "";

                    if (stage.Contains(Constants.REG_STAGE_DONE))
                    {
                        PreferenceHelper.Instance.SetString("firstRun", "regd");

                        var intent = new Intent(this, typeof(MainActivity));                     
                        StartActivity(intent);
                        Finish();
                        OnboardingActivity.Instance.DismissLoader();
                    }
                    else
                    {
                        OnboardingActivity.Instance.GetStage(stage);
                        OnboardingActivity.Instance.DismissLoader();
                    }
                }
                

            }, (e) => 
            {
                Toast.MakeText(this, e.Message, ToastLength.Short).Show();
            }));
        }
    }
}
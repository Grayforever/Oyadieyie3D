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
    [Activity(MainLauncher = true, Theme = "@style/AppTheme.Splash", Label ="@string/app_name", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout| Android.Content.PM.ConfigChanges.SmallestScreenSize| Android.Content.PM.ConfigChanges.Orientation)]
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
                StartActivity(typeof(MainActivity));
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
                    StartActivity(typeof(OnboardingActivity));
                    Finish();
                    break;
                default:
                    CheckStatus(currentUser.Uid);
                    break;
            }
        }

        private void CheckStatus(string uid)
        {
            Toast.MakeText(this, "Getting your last session", ToastLength.Long).Show();
            var statusRef = SessionManager.GetFireDB().GetReference("session");
            statusRef.OrderByKey().EqualTo(uid).AddListenerForSingleValueEvent(new SingleValueListener((s) => 
            {
                if (!s.Child(uid).Exists())
                {
                    StartActivity(typeof(OnboardingActivity));
                    Finish();
                }
                else
                {
                    string stage = s.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Child(Constants.SESION_CHILD) != null ? s.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Child(Constants.SESION_CHILD).Value.ToString() : "";
                    if (stage.Contains(Constants.REG_STAGE_DONE))
                    {
                        PreferenceHelper.Instance.SetString("firstRun", "regd");

                        var intent = new Intent(this, typeof(MainActivity));
                        intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask);
                        StartActivity(intent);
                        OnboardingActivity.DismissLoader();
                    }
                    else
                    {
                        OnboardingActivity.GetStage(stage);
                        OnboardingActivity.DismissLoader();
                    }
                }
                

            }, (e) => 
            {
                Toast.MakeText(this, e.Message, ToastLength.Short).Show();
            }));
        }
    }
}
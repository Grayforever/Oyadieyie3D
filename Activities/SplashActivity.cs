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
    public class SplashActivity : AppCompatActivity
    {
        ISharedPreferences preferences = Application.Context.GetSharedPreferences("userInfo", FileCreationMode.Private);
        ISharedPreferencesEditor editor;

        public static string REG_STATUS = "reg_status";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string firstRun = preferences.GetString("firstRun", "");

            if (firstRun != "" && firstRun != "reg")
            {
                StartActivity(typeof(MainActivity));
                Finish();
            }
            else
            {
                editor = preferences.Edit();
                editor.PutString("firstRun", "reg");
                editor.Commit();
                RouteToAppropriatePage(SessionManager.GetFirebaseAuth().CurrentUser);
            }
        }

        private void RouteToAppropriatePage(FirebaseUser currentUser)
        {
            if (currentUser == null)
            {
                StartActivity(typeof(OnboardingActivity));
                Finish();
            }
            else
            {
                CheckStatus(currentUser.Uid);
            }
        }

        private void CheckStatus(string uid)
        {
            var statusRef = SessionManager.GetFireDB().GetReference($"userSessions/{uid}");
            statusRef.AddListenerForSingleValueEvent(new SingleValueListener((s) => 
            {
                if (!s.Exists())
                    StartActivity(typeof(OnboardingActivity));

                string stage = s.Child("status") != null ? s.Child("status").Value.ToString() : "";
                var stateIntent = new Intent(this, typeof(OnboardingActivity));
                stateIntent.PutExtra(REG_STATUS, stage);
                StartActivity(stateIntent);
            }, (e) => 
            {
                Toast.MakeText(this, e.Message, ToastLength.Short).Show();
            }));
        }
    }
}
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
        ISharedPreferences preferences = Application.Context.GetSharedPreferences(Constants.PREF_NAME, FileCreationMode.Private);
        ISharedPreferencesEditor editor;
        private string _firstRun = "firstRun";

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
                editor.PutString(_firstRun, "reg");
                editor.Commit();
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
            statusRef.OrderByKey().EqualTo(SessionManager.GetFirebaseAuth().CurrentUser.Uid).AddValueEventListener(new SingleValueListener((s) => 
            {
                if (s.Exists() && s.HasChildren)
                {
                    string stage = s.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Child(Constants.SESION_CHILD) != null ? s.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Child(Constants.SESION_CHILD).Value.ToString() : "";
                    var stateIntent = new Intent(this, typeof(OnboardingActivity));
                    stateIntent.PutExtra(Constants.SESION_CHILD, stage);
                    StartActivity(stateIntent);
                    Finish();
                }
                else
                {
                    StartActivity(typeof(OnboardingActivity));
                    Finish();
                }

            }, (e) => 
            {
                Toast.MakeText(this, e.Message, ToastLength.Short).Show();
            }));
        }
    }
}
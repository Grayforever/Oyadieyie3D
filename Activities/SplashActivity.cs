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
            statusRef.OrderByKey().EqualTo(uid).AddValueEventListener(new SingleValueListener((s) => 
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
                        editor = preferences.Edit();
                        editor.PutString("firstRun", "regd");
                        editor.Commit();

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
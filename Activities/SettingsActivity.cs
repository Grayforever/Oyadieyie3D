using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.AppBar;
using Oyadieyie3D.Fragments;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "Settings", Theme = "@style/AppTheme.PreferenceTheme", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SettingsActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.settings_activity);


            var settingsAppbar = FindViewById<AppBarLayout>(Resource.Id.settings_appbar);
            var settingsToolbar = settingsAppbar.FindViewById<Toolbar>(Resource.Id.main_toolbar);
            SetSupportActionBar(settingsToolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.settings_fragment_container, new SettingsFragment())
                .CommitAllowingStateLoss();
            
        }

        public override bool OnSupportNavigateUp()
        {
            Finish();
            return true;

        }
    }
}
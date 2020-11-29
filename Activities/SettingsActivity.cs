using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.App;
using AndroidX.Preference;
using BumpTech.GlideLib;
using BumpTech.GlideLib.Requests;
using DE.Hdodenhof.CircleImageViewLib;
using Google.Android.Material.AppBar;
using Oyadieyie3D.HelperClasses;
using Oyadieyie3D.Parcelables;
using static AndroidX.Fragment.App.FragmentManager;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "Settings", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class SettingsActivity : AppCompatActivity, PreferenceFragmentCompat.IOnPreferenceStartFragmentCallback
    {
        private const string SettingsKey = "Settings";
        private Toolbar toolbar;

        private string tag;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.settings_activity);
            var appBar = FindViewById<AppBarLayout>(Resource.Id.settings_appbar);
            toolbar = appBar.FindViewById<Toolbar>(Resource.Id.main_toolbar);

            if (savedInstanceState == null)
            {
                var fragment = SupportFragmentManager.FindFragmentByTag(SettingsFragment.FRAGMENT_TAG);
                if(fragment == null)
                {
                    fragment = new SettingsFragment();
                }

                var ft = SupportFragmentManager.BeginTransaction();
                ft.Replace(Resource.Id.frag_container_s, fragment, SettingsFragment.FRAGMENT_TAG);
                ft.Commit();

            }
            else
            {
                Title = savedInstanceState.GetCharSequence(SettingsKey);
            }

            SupportFragmentManager.AddOnBackStackChangedListener(new OnBackStackChangedlistener(() => 
            {
                var id = SupportFragmentManager.BackStackEntryCount;
                if (SupportFragmentManager.BackStackEntryCount == 0)
                {
                    SetTitle(Resource.String.settings_title);
                }
                
            }));
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutCharSequence(SettingsKey, Title);
        }

        public override bool OnSupportNavigateUp()
        {
            base.OnBackPressed();
            return true;
        }

        public bool OnPreferenceStartFragment(PreferenceFragmentCompat caller, Preference pref)
        {
            tag = pref.Key;
            var ft = SupportFragmentManager.BeginTransaction();

            Bundle args = pref.Extras;
            var fragment = SupportFragmentManager.FragmentFactory.Instantiate(ClassLoader, pref.Fragment);
            args.PutString(PreferenceFragmentCompat.ArgPreferenceRoot, pref.Key);
            fragment.Arguments = args;
            fragment.SetTargetFragment(caller, 0);

            ft.SetCustomAnimations(Resource.Animation.enter, Resource.Animation.exit, Resource.Animation.pop_enter, Resource.Animation.pop_exit);
            ft.Replace(Resource.Id.frag_container_s, fragment, pref.Key);
            ft.AddToBackStack(null);
            ft.Commit();

            Title = pref.Title;
            return true;
        }

        internal sealed class OnBackStackChangedlistener : Java.Lang.Object, IOnBackStackChangedListener
        {
            private System.Action onBackStackChanged;
            public OnBackStackChangedlistener(System.Action onBackStackChanged)
            {
                this.onBackStackChanged = onBackStackChanged;
            }
            public void OnBackStackChanged()
            {
                onBackStackChanged?.Invoke();
            }
        }

        internal sealed class SettingsFragment : PreferenceFragmentCompat
        {
            public static string FRAGMENT_TAG = "my_preference_fragment";
            public SettingsFragment()
            {

            }

            public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
            {
                SetPreferencesFromResource(Resource.Xml.settings_pref, rootKey);
            }

            public override AndroidX.Fragment.App.Fragment CallbackFragment => this;

            public override void OnNavigateToScreen(PreferenceScreen preferenceScreen)
            {
                base.OnNavigateToScreen(preferenceScreen);
            }
        }
    }
}
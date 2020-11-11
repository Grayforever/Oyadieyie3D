using Android.OS;
using Android.Runtime;
using AndroidX.Preference;
using Oyadieyie3D.HelperClasses;

namespace Oyadieyie3D.Fragments
{
    [Register("id.Oyadieyie3D.Fragments.PrefPrivacyFragment")]
    public class PrefPrivacyFragment : PreferenceFragmentCompat
    {
        private string LablePrefKey = "biometric_lock";
        public static Preference lablePref;
        private bool bioStatus;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            PreferenceHelper.Init(Context);
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.privacy_pref_screen, rootKey);
            lablePref = PreferenceScreen.FindPreference(LablePrefKey);
            bioStatus = PreferenceHelper.Instance.GetBoolean(Constants.BioStatusKey);
            lablePref.Summary = bioStatus != true ? "Disabled" : "Enabled";
        }
    }
}
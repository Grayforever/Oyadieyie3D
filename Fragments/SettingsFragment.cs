using Android.OS;
using AndroidX.Fragment.App;
using AndroidX.Preference;

namespace Oyadieyie3D.Fragments
{
    public class SettingsFragment : PreferenceFragmentCompat
    {
        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.settings_pref, rootKey);
        }


        public override Fragment CallbackFragment => this;

    }
}
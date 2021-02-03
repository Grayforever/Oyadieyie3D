using Android.OS;
using Android.Runtime;
using AndroidX.Preference;
using R = Oyadieyie3D.Resource;

namespace Oyadieyie3D.Fragments
{
    [Register("id.Oyadieyie3D.Fragments.PrefHelpFragment")]
    public class PrefHelpFragment : PreferenceFragmentCompat
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(R.Xml.help_pref_screen, rootKey);
        }
    }
}
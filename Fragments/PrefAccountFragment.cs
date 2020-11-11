using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Preference;

namespace Oyadieyie3D.Fragments
{
    [Register("id.Oyadieyie3D.Fragments.PrefAccountFragment")]
    public class PrefAccountFragment : PreferenceFragmentCompat
    {
        
        public PrefAccountFragment()
        {

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.account_pref_screen, rootKey);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Preference;

namespace Oyadieyie3D.Fragments
{
    public class SettingsFragment : PreferenceFragmentCompat
    {
        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.settings_pref);
            BindPreferenceSummaryToValue(FindPreference(GetString(Resource.String.pref_key_zipcode)));
            BindPreferenceSummaryToValue(FindPreference(GetString(Resource.String.pref_key_unit)));
        }

        private void BindPreferenceSummaryToValue(Preference preference)
        {
            preference.PreferenceChange += Preference_PreferenceChange;
        }

        private void Preference_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            
        }

        public override bool OnPreferenceTreeClick(Preference preference)
        {
            return base.OnPreferenceTreeClick(preference);
        }

        protected override void OnBindPreferences()
        {
            base.OnBindPreferences();
        }
    }
}
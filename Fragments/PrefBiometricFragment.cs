using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.Biometric;
using AndroidX.Core.Content;
using AndroidX.Preference;
using Java.Lang;
using Java.Util.Concurrent;
using Oyadieyie3D.Callbacks;
using Oyadieyie3D.HelperClasses;
using static AndroidX.Preference.Preference;

namespace Oyadieyie3D.Fragments
{
    [Register("id.Oyadieyie3D.Fragments.PrefBiometricFragment")]
    public class PrefBiometricFragment : PreferenceFragmentCompat, IOnPreferenceChangeListener
    {
        public const string BioSwitchKey = "biometric_switch";
        public const string DurationListKey = "duration_list";
        
        private IExecutor executor;
        private BiometricPrompt biometricPrompt;
        private BiometricPrompt.PromptInfo promptInfo;
        private ISharedPreferences prefManager;
        private SwitchPreferenceCompat bioSwitchPreference;
        private ListPreference durationlistPreference;
        private bool isBioEnabled;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            executor = ContextCompat.GetMainExecutor(Context);
                biometricPrompt = new BiometricPrompt(this, executor, GetAuthenticationCallback());
                promptInfo = new BiometricPrompt.PromptInfo.Builder()
                    .SetTitle("Confirm fingerprint")
                    .SetNegativeButtonText("Cancel")
                    .Build();
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.biometric_pref_screen, rootKey);
            prefManager = PreferenceManager.GetDefaultSharedPreferences(Context);
            bioSwitchPreference = (SwitchPreferenceCompat)PreferenceScreen.FindPreference(BioSwitchKey);
            durationlistPreference = (ListPreference)PreferenceScreen.FindPreference(DurationListKey);
            bioSwitchPreference.OnPreferenceChangeListener = this;
            isBioEnabled = prefManager.GetBoolean(BioSwitchKey, false);
            durationlistPreference.Enabled = isBioEnabled;
        }

        public bool OnPreferenceChange(Preference preference, Object newValue)
        {
            bool isBioLockOn = (bool)newValue;
            durationlistPreference.Enabled = isBioLockOn;
            SetPrefValue(isBioLockOn);
            if (isBioLockOn)
            {
                biometricPrompt.Authenticate(promptInfo);
            }

            return true;
        }

        private void SetPrefValue(bool isBioLockOn)
        {
            PrefPrivacyFragment.lablePref.Summary = isBioLockOn != true ? "Disabled" : "Enabled";
            var editor = prefManager.Edit();
            editor.PutBoolean(Constants.BioStatusKey, isBioLockOn);
            editor.Commit();
        }

        private BiometricAuthenticationCallback GetAuthenticationCallback()
        {
            var callback = new BiometricAuthenticationCallback
            {
                Success = (BiometricPrompt.AuthenticationResult result) =>
                {
                    bioSwitchPreference.Checked = true;
                    durationlistPreference.Enabled = true;
                },
                Failed = () =>
                {
                    bioSwitchPreference.Checked = false;
                    durationlistPreference.Enabled = false;
                },
                Error = (int errorCode, ICharSequence msg) =>
                {
                    bioSwitchPreference.Checked = false;
                    durationlistPreference.Enabled = false;
                }
            };
            return callback;
        }
    }
}
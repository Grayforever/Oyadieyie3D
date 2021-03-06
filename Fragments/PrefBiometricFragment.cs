﻿using Android.OS;
using Android.Runtime;
using AndroidX.Biometric;
using AndroidX.Core.Content;
using AndroidX.Preference;
using Java.Lang;
using Java.Util.Concurrent;
using Oyadieyie3D.Callbacks;
using Oyadieyie3D.HelperClasses;
using static AndroidX.Preference.Preference;
using R = Oyadieyie3D.Resource;

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
        private SwitchPreferenceCompat bioSwitchPreference;
        private ListPreference durationlistPreference;
        private bool isBioEnabled;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            PreferenceHelper.Init(Context);
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(R.Xml.biometric_pref_screen, rootKey);
            bioSwitchPreference = (SwitchPreferenceCompat)PreferenceScreen.FindPreference(BioSwitchKey);
            durationlistPreference = (ListPreference)PreferenceScreen.FindPreference(DurationListKey);
            bioSwitchPreference.OnPreferenceChangeListener = this;
            isBioEnabled = PreferenceHelper.Instance.GetBoolean(BioSwitchKey);
            durationlistPreference.Enabled = isBioEnabled;
        }

        public bool OnPreferenceChange(Preference preference, Object newValue)
        {
            bool isBioLockOn = (bool)newValue;
            durationlistPreference.Enabled = isBioLockOn;
            SetPrefValue(isBioLockOn);
            if (isBioLockOn)
            {
                executor = ContextCompat.GetMainExecutor(Context);
                biometricPrompt = new BiometricPrompt(this, executor, GetAuthenticationCallback());
                promptInfo = new BiometricPrompt.PromptInfo.Builder()
                    .SetTitle("Confirm fingerprint")
                    .SetNegativeButtonText("Cancel")
                    .Build();

                biometricPrompt.Authenticate(promptInfo);
            }

            return true;
        }

        private void SetPrefValue(bool isBioLockOn)
        {
            PrefPrivacyFragment.lablePref.Summary = isBioLockOn != true ? "Disabled" : "Enabled";
            PreferenceHelper.Instance.SetBoolean(Constants.BioStatusKey, isBioLockOn);
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
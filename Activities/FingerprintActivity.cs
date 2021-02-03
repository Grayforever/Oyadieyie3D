using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Biometric;
using AndroidX.Core.Content;
using Google.Android.Material.Button;
using Java.Lang;
using Java.Util.Concurrent;
using Oyadieyie3D.Callbacks;
using R = Oyadieyie3D.Resource;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize |
        Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class FingerprintActivity : AppCompatActivity
    {
        private IExecutor executor;
        private BiometricPrompt biometricPrompt;
        private BiometricPrompt.PromptInfo promptInfo;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.biometric_prompt_sheet);
            
            var unlockBtn = FindViewById<MaterialButton>(R.Id.unlock_bio_btn);
            unlockBtn.Click += UnlockBtn_Click;
        }

        private void UnlockBtn_Click(object sender, System.EventArgs e)
        {
            executor = ContextCompat.GetMainExecutor(this);
            biometricPrompt = new BiometricPrompt(this, executor, GetAuthenticationCallback());
            promptInfo = new BiometricPrompt.PromptInfo.Builder()
                .SetNegativeButtonText("Cancel")
                .SetTitle("Unlock")
                .Build();

            biometricPrompt.Authenticate(promptInfo);
        }

        private BiometricAuthenticationCallback GetAuthenticationCallback()
        {
            var callback = new BiometricAuthenticationCallback
            {
                Success = (BiometricPrompt.AuthenticationResult result) =>
                {
                    Finish();
                },
                Failed = () =>
                {
                    Toast.MakeText(this, "Authentication failed", ToastLength.Short).Show();
                },
                Error = (int errorCode, ICharSequence msg) =>
                {
                    switch (errorCode)
                    {
                        case BiometricConstants.ErrorCanceled:
                            break;
                        case BiometricConstants.ErrorHwUnavailable:
                            break;
                        case BiometricConstants.ErrorNoBiometrics:
                            break;
                        case BiometricConstants.ErrorNegativeButton:
                            break;
                    }
                }
            };
            return callback;
        }

        public override void OnBackPressed() => FinishAffinity();
    }
}
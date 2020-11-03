using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Activity;
using AndroidX.Biometric;
using AndroidX.Core.Content;
using AndroidX.Fragment.App;
using Google.Android.Material.Button;
using Java.Lang;
using Java.Util.Concurrent;
using Oyadieyie3D.Callbacks;

namespace Oyadieyie3D.Fragments
{
    public class BiometricPromptSheet : DialogFragment
    {
        private IExecutor executor;
        private BiometricPrompt biometricPrompt;
        private BiometricPrompt.PromptInfo promptInfo;

        public override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);
            SetStyle(StyleNormal, Resource.Style.Theme_MaterialComponents_DayNight_DialogWhenLarge);

            executor = ContextCompat.GetMainExecutor(Activity);
            biometricPrompt = new BiometricPrompt(this, executor, GetAuthenticationCallback());
            promptInfo = new BiometricPrompt.PromptInfo.Builder()
                .SetTitle("Oyadieyie3D Locked")
                .SetDeviceCredentialAllowed(true)
                .Build();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.biometric_prompt_sheet, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var unlockBtn = view.FindViewById<MaterialButton>(Resource.Id.unlock_bio_btn);
            unlockBtn.Click += UnlockBtn_Click;
        }

        private void UnlockBtn_Click(object sender, System.EventArgs e) => biometricPrompt.Authenticate(promptInfo);

        private BiometricAuthenticationCallback GetAuthenticationCallback()
        {
            var callback = new BiometricAuthenticationCallback
            {
                Success = (BiometricPrompt.AuthenticationResult result) =>
                {
                    Dismiss();
                },
                Failed = () =>
                {
                    Toast.MakeText(Activity, "Authentication Failed", ToastLength.Short).Show();
                },
                Error = (int errorCode, ICharSequence msg) =>
                {
                    Toast.MakeText(Activity, "Authentication Error: " + msg, ToastLength.Short).Show();
                }
            };
            return callback;
        }
    }
}
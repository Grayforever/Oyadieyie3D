using Android.Hardware.Biometrics;
using Android.Runtime;
using AndroidX.Biometric;
using Java.Lang;
using System;
using BiometricPrompt = AndroidX.Biometric.BiometricPrompt;

namespace Oyadieyie3D.Callbacks
{
    internal sealed class BiometricAuthenticationCallback : BiometricPrompt.AuthenticationCallback
    {
        public Action<BiometricPrompt.AuthenticationResult> Success;
        public Action Failed;
        public Action<int, ICharSequence> Error;

        public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult result)
        {
            base.OnAuthenticationSucceeded(result);
            Success(result);
        }

        public override void OnAuthenticationFailed()
        {
            base.OnAuthenticationFailed();
            Failed();
        }

        public override void OnAuthenticationError(int errorCode, ICharSequence errString)
        {
            base.OnAuthenticationError(errorCode, errString);
            Error(errorCode, errString);
        }

    }
}
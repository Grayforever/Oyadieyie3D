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
using Firebase;
using Firebase.Auth;
using static Firebase.Auth.PhoneAuthProvider;

namespace Oyadieyie3D.Callbacks
{
    public class PhoneVerificationCallbacks : OnVerificationStateChangedCallbacks
    {
        private Action<PhoneAuthCredential> _onVerificationCompleted;
        private Action<FirebaseException> _onVerificationFailed;
        private Action<string, ForceResendingToken> _onCodeSent;

        public PhoneVerificationCallbacks(Action<PhoneAuthCredential> onVerificationCompleted, Action<FirebaseException> onVerificationFailed,
            Action<string, ForceResendingToken> onCodeSent)
        {
            _onVerificationCompleted = onVerificationCompleted;
            _onVerificationFailed = onVerificationFailed;
            _onCodeSent = onCodeSent;
        }

        public override void OnVerificationCompleted(PhoneAuthCredential p0) => _onVerificationCompleted?.Invoke(p0);

        public override void OnVerificationFailed(FirebaseException p0) => _onVerificationFailed?.Invoke(p0);

        public override void OnCodeSent(string p0, ForceResendingToken p1) => _onCodeSent?.Invoke(p0, p1);
    }
}
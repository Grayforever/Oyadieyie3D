using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using CN.Pedant.SweetAlert;
using Com.Goodiebag.Pinview;
using Firebase;
using Firebase.Auth;
using Google.Android.Material.Button;
using Java.Util.Concurrent;
using Oyadieyie3D.Callbacks;
using Oyadieyie3D.Events;
using Oyadieyie3D.HelperClasses;
using Oyadieyie3D.Utils;
using System;
using static Com.Goodiebag.Pinview.Pinview;

namespace Oyadieyie3D.Fragments
{
    public class GetSmsFragment : AndroidX.Fragment.App.Fragment, IPinViewEventListener
    {
        private Pinview otpView;
        private MaterialButton verifyBtn;
        string phone = "";
        private SweetAlertDialog loaderDialog;
        string verificationId = "";
        private FirebaseAuth auth;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            phone = Arguments.GetString(EnterPhoneFragment.phoneKey);
            SendVerificationCode();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.get_sms_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            otpView = view.FindViewById<Pinview>(Resource.Id.otp_tv);
            verifyBtn = view.FindViewById<MaterialButton>(Resource.Id.verify_otp_btn);
            var numTv = view.FindViewById<TextView>(Resource.Id.get_sms_sub2);
            SetSpan(numTv);
            verifyBtn.Click += VerifyBtn_Click;
            otpView.SetPinViewEventListener(this);
        }

        public void OnDataEntered(Pinview p0, bool p1)
        {
            verifyBtn.Post(() =>
            {
                verifyBtn.Enabled = p0.Value.Length == 6;
            });
        }

        private void SetSpan(TextView numTv)
        {
            string one = "Enter one time password sent on ";
            SpannableString ss = new SpannableString(one + phone);
            
            var cs = new MyClickableSpan((widget) => 
            {
                Toast.MakeText(Context, "clicked", ToastLength.Short).Show();
            }, (ds)=> 
            {
                ds.UnderlineText = false;
                ds.SetTypeface(Typeface.DefaultBold);
            });
            ss.SetSpan(cs, one.Length, one.Length + phone.Length, SpanTypes.ExclusiveExclusive);
            numTv.TextFormatted = ss;
            numTv.MovementMethod = LinkMovementMethod.Instance;
            numTv.SetHighlightColor(Color.Transparent);
        }

        private void VerifyBtn_Click(object sender, EventArgs e)
        {
            verifyBtn.PostDelayed(() =>
            {
                VerifyCode(otpView.Value);
            }, 1000);
        }

        private void VerifyCode(string code)
        {
            try
            {
                ShowLoader();
                PhoneAuthCredential cred = PhoneAuthProvider.GetCredential(verificationId, code);
                auth.SignInWithCredential(cred)
                    .AddOnCompleteListener(new OncompleteListener(
                    onComplete: (t) =>
                    {
                        if (!t.IsSuccessful)
                            throw t.Exception;

                        CheckUserAvailability();
                    }));
            }
            catch (FirebaseNetworkException fne)
            {
                DismissLoader();
                ShowError("Network error", fne.Message);
            }
            catch (FirebaseException fe)
            {
                DismissLoader();
                ShowError("Authentication Error", fe.Message);
            }
            catch(Exception e)
            {
                DismissLoader();
                ShowError("App Error", e.Message);
            }
            
        }

        private void CheckUserAvailability()
        {
            try
            {
                var userRef = SessionManager.UserRef.Child($"{auth.CurrentUser.Uid}");
                userRef.AddValueEventListener(new SingleValueListener(
                        onDataChange: (s) =>
                        {
                            if (!s.Exists())
                            {
                                GotoProfile();
                            }
                            else
                            {
                                string firstname = s.Child("profile").Child("fname") == null ? "" : s.Child("profile").Child("fname").Value.ToString();
                                GotoMain(firstname);
                            }
                        },
                        onCancelled: (e) =>
                        {

                        }));
            }
            catch (Exception e)
            {
                ShowError(e.Source, e.Message);
            }
        }

        private void GotoMain(string name)
        {
            var intent = new Intent(Activity, typeof(MainActivity));
            intent.PutExtra("firstName", name);
            intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask);
            StartActivity(intent);
            DismissLoader();
        }

        private void GotoProfile()
        {
            DismissLoader();
            ParentFragmentManager.BeginTransaction()
                .Replace(Resource.Id.frag_container, new PartnerFragment())
                .CommitAllowingStateLoss();
        }

        private void SendVerificationCode()
        {
            try
            {
                auth = SessionManager.GetFirebaseAuth();
                PhoneAuthProvider.GetInstance(auth)
                    .VerifyPhoneNumber(phone, 30, TimeUnit.Seconds, Activity, new PhoneVerificationCallbacks(
                    onVerificationCompleted: (cred) =>
                    {
                        var code = cred.SmsCode;
                        if (string.IsNullOrEmpty(code))
                            return;

                        otpView.Value = code;
                        VerifyCode(code);
                    }, onVerificationFailed: (e) =>
                    {
                        //DismissLoader();
                        ShowError("Firebase Exception", e.Message);
                    }, onCodeSent: (code, token) =>
                    {
                        verificationId = code;
                    }));
            }
            catch (Exception e)
            {
                ShowError("Oops...", e.Message);
            }
        }

        private void DismissLoader()
        {
            if (!loaderDialog.IsShowing || loaderDialog == null)
                return;

            loaderDialog.DismissWithAnimation();
            loaderDialog = null;
        }

        public void ShowError(string title, string message)
        {
            new SweetAlertDialog(Context, SweetAlertDialog.ErrorType)
                .SetTitleText(title)
                .SetContentText(message)
                .SetConfirmText("OK")
                .ShowCancelButton(false)
                .SetConfirmClickListener(null)
                .Show();
        }

        public void ShowLoader()
        {
            loaderDialog = new SweetAlertDialog(Context, SweetAlertDialog.ProgressType);
            loaderDialog.SetTitleText("Loading");
            loaderDialog.ShowCancelButton(false);
            loaderDialog.Show();
        }
    }
}
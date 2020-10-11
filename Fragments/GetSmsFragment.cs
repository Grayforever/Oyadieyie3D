using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using Com.Goodiebag.Pinview;
using Firebase;
using Firebase.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Java.Util.Concurrent;
using Oyadieyie3D.Activities;
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
        private FloatingActionButton verifiyBtn;
        private MaterialButton resendBtn;
        
        private FirebaseAuth auth;
        private CountDownTimer cdTimer;

        private string verificationId { get; set; }
        private string phone { get; set; }

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
            verifiyBtn = view.FindViewById<FloatingActionButton>(Resource.Id.verify_otp_fab);
            resendBtn = view.FindViewById<MaterialButton>(Resource.Id.resend_code_btn);
            var numTv = view.FindViewById<TextView>(Resource.Id.get_sms_sub2);
            new Spanner(Activity, false).SetSpan(numTv, new string[] { "Enter OTP code sent on ", phone});
            verifiyBtn.Click += VerifyBtn_Click;
            resendBtn.Click += ResendBtn_Click;
            otpView.SetPinViewEventListener(this);

            cdTimer = new CountdownTimer(40000, 1000, () =>
            {
                resendBtn.Enabled = true;
                resendBtn.Text = "resend otp";
            }, (millisUntilFinished) =>
            {
                resendBtn.Text = "Resend OTP in " + millisUntilFinished / 1000;
            }).Start();
        }

        private void ResendBtn_Click(object sender, EventArgs e)
        {
            
        }

        public void OnDataEntered(Pinview p0, bool p1)
        {
            verifiyBtn.Post(() => { verifiyBtn.Enabled = p0.Value.Length == 6; });
        }

        private void VerifyBtn_Click(object sender, EventArgs e)
        {
            resendBtn.PostDelayed(() => { VerifyCode(otpView.Value); }, 1000);
        }

        private void VerifyCode(string code)
        {
            OnboardingActivity.ShowLoader();
            PhoneAuthCredential cred = PhoneAuthProvider.GetCredential(verificationId, code);
            auth.SignInWithCredential(cred)
                .AddOnCompleteListener(new OncompleteListener(
                onComplete: (t) =>
                {
                    try
                    {
                        switch (t.IsSuccessful)
                        {
                            case false:
                                throw t.Exception;

                            default:
                                CheckUserAvailability();
                                break;
                        }
                    }
                    catch (FirebaseAuthInvalidCredentialsException fiace)
                    {
                        OnboardingActivity.DismissLoader();
                        OnboardingActivity.ShowError(fiace.Source, fiace.Message);
                    }
                    catch (FirebaseTooManyRequestsException ftmre)
                    {
                        OnboardingActivity.DismissLoader();
                        OnboardingActivity.ShowError(ftmre.Source, ftmre.Message);
                    }
                    catch (FirebaseAuthInvalidUserException fiue)
                    {
                        OnboardingActivity.DismissLoader();
                        OnboardingActivity.ShowError(fiue.Source, fiue.Message);
                    }
                    catch (FirebaseNetworkException)
                    {
                        OnboardingActivity.DismissLoader();
                        OnboardingActivity.ShowNoNetDialog(false);
                    }
                    catch (Exception e)
                    {
                        OnboardingActivity.DismissLoader();
                        OnboardingActivity.ShowError(e.Source, e.Message);
                    }
                }));
        }

        private void CheckUserAvailability()
        {
            try
            {
                var userRef = SessionManager.UserRef.Child(SessionManager.UserId);
                userRef.AddValueEventListener(new SingleValueListener(
                onDataChange: (s) =>
                {
                    if (!s.Exists())
                    {
                        GotoProfile();
                    }
                    else
                    {
                        var firstname = s.Child("profile").Child("fname") == null ? "" : s.Child("profile").Child("fname").Value.ToString();
                        GotoMain(firstname);
                    }
                },
                onCancelled: (e) =>
                {

                }));
            }
            catch (Exception e)
            {
                OnboardingActivity.ShowError(e.Source, e.Message);
            }
        }

        private void GotoMain(string name)
        {
            SetStatus("done");
            var intent = new Intent(Activity, typeof(MainActivity));
            intent.PutExtra("firstName", name);
            intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask);
            StartActivity(intent);
            OnboardingActivity.DismissLoader();
        }

        public static void SetStatus(string status)
        {
            var statusRef = SessionManager.GetFireDB().GetReference($"userSessions/{SessionManager.UserId}");
            statusRef.Child("status").SetValueAsync(status);
        }

        private void GotoProfile()
        {
            SetStatus("create_profile");
            OnboardingActivity.DismissLoader();
            ParentFragmentManager.BeginTransaction()
                .Replace(Resource.Id.frag_container, new CreateProfileFragment())
                .CommitAllowingStateLoss();
        }

        private void SendVerificationCode()
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
                    try
                    {
                        resendBtn.Enabled = false;
                        cdTimer.Cancel();
                        throw e;
                    }
                    catch (FirebaseNetworkException)                    
                    {
                        OnboardingActivity.ShowNoNetDialog(false);
                    }
                    catch (FirebaseTooManyRequestsException ftmre)
                    {
                        OnboardingActivity.ShowError(ftmre.Source, ftmre.Message);
                    }
                    catch (FirebaseAuthInvalidCredentialsException faice)
                    {
                        OnboardingActivity.ShowError(faice.Source, faice.Message);
                    }
                    catch (FirebaseAuthInvalidUserException fiue)
                    {
                        OnboardingActivity.ShowError(fiue.Source, fiue.Message);
                    }
                    catch (Exception ex)
                    {
                        OnboardingActivity.ShowError(ex.Source, ex.Message);
                    }
                    
                }, onCodeSent: (code, token) =>
                {
                    verificationId = code;
                }));
            
        }


    }
}
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Style;
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
using System;
using static Com.Goodiebag.Pinview.Pinview;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Oyadieyie3D.Fragments
{
    public class GetSmsFragment : AndroidX.Fragment.App.Fragment, IPinViewEventListener
    {
        private Pinview otpView;
        private FloatingActionButton verifiyBtn;
        private MaterialButton resendBtn;
        private TextView numTv;
        private CountDownTimer resendBtnTimer;

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
            numTv = view.FindViewById<TextView>(Resource.Id.get_sms_sub2);
            var editNumTv  = view.FindViewById<TextView>(Resource.Id.get_sms_sub3);
            var toolbarMain = view.FindViewById<Toolbar>(Resource.Id.get_sms_toolbar);

            string smsTxt = "An SMS code was sent to\n";
            var ss = new SpannableString(smsTxt + phone);
            ss.SetSpan(new StyleSpan(TypefaceStyle.Bold), smsTxt.Length, smsTxt.Length + phone.Length, SpanTypes.ExclusiveExclusive);
            numTv.TextFormatted = ss;

            toolbarMain.NavigationClick += ToolbarMain_NavigationClick;

            verifiyBtn.Click += VerifyBtn_Click;
            resendBtn.Click += ResendBtn_Click;
            otpView.SetPinViewEventListener(this);

            editNumTv.Click += (s1, e1) =>
            {
                ChildFragmentManager.PopBackStackImmediate();
            };

            resendBtnTimer = new CountdownTimer(30000, 1000, () =>
            {
                resendBtn.Enabled = true;
                resendBtn.Text = "resend otp";
            }, (millisUntilFinished) =>
            {
                resendBtn.Text = "Resend OTP in " + millisUntilFinished / 1000;
            }).Start();
        }

        private void ToolbarMain_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e) => Activity.OnBackPressed();

        private void ResendBtn_Click(object sender, EventArgs e)
        {
            
        }

        public void OnDataEntered(Pinview p0, bool p1)
        {
            verifiyBtn.Post(() => { verifiyBtn.Enabled = p0.Value.Length == 6; });
        }

        private void VerifyBtn_Click(object sender, EventArgs e)
        {
            resendBtn.Post(() => { VerifyCode(otpView.Value); });
        }

        private void VerifyCode(string code)
        {
            OnboardingActivity.ShowLoader();
            try
            {
                PhoneAuthCredential cred = PhoneAuthProvider.GetCredential(verificationId, code);
                SessionManager.GetFirebaseAuth().SignInWithCredential(cred)
                    .AddOnCompleteListener(new OncompleteListener(
                    onComplete: (t) =>
                    {
                        switch (t.IsSuccessful)
                        {
                            case false:
                                throw t.Exception;

                            default:
                                CheckUserAvailability();
                                break;
                        }

                    }));
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
            
        }

        private void CheckUserAvailability()
        {
            var userRef = SessionManager.UserRef.Child(SessionManager.UserId);
            var statusRef = SessionManager.GetFireDB().GetReference("session");
            statusRef.OrderByKey().EqualTo(SessionManager.GetFirebaseAuth().CurrentUser.Uid).AddValueEventListener(new SingleValueListener((s) =>
            {
                if (s.Exists() && s.HasChildren)
                {
                    string stage = s.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Child(Constants.SESION_CHILD) != null ? s.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Child(Constants.SESION_CHILD).Value.ToString() : "";
                    if (stage.Contains(Constants.REG_STAGE_DONE))
                    {
                        var intent = new Intent(Activity, typeof(MainActivity));
                        intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask);
                        StartActivity(intent);
                        OnboardingActivity.DismissLoader();
                    }
                    else
                    {
                        OnboardingActivity.DismissLoader();
                        OnboardingActivity.GetStage(stage);
                    }
                    
                }
                else
                {
                    GotoProfile();
                }

            }, (e) =>
            {
                OnboardingActivity.DismissLoader();
                OnboardingActivity.ShowError("Database error", e.Message);
            }));
        }

        private void GotoProfile()
        {
            OnboardingActivity.SetStatus(Constants.REG_STAGE_CREATE_PROFILE);
            OnboardingActivity.DismissLoader();
            ParentFragmentManager.BeginTransaction()
                .Replace(Resource.Id.frag_container, new CreateProfileFragment())
                .CommitAllowingStateLoss();
        }

        private void SendVerificationCode()
        {
            PhoneAuthProvider.Instance.VerifyPhoneNumber(phone, 30, TimeUnit.Seconds, Activity, new PhoneVerificationCallbacks(
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
                    finally
                    {
                        resendBtn.Visibility = ViewStates.Invisible;
                        numTv.Text = "You entered an invalid phone number!";
                        numTv.SetTextColor(ColorStateList.ValueOf(Color.Red));
                    }
                    
                }, onCodeSent: (code, token) =>
                {
                    verificationId = code;
                }));
        }
    }
}
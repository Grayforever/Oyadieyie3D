using Android.App;
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
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Oyadieyie3D.Fragments
{
    public class GetSmsFragment : AndroidX.Fragment.App.Fragment
    {
        private Pinview otpView;
        private FloatingActionButton verifiyBtn;
        private MaterialButton resendBtn;
        private TextView numTv;
        private CountDownTimer resendBtnTimer;

        private ISharedPreferences preferences = Application.Context.GetSharedPreferences("userInfo", FileCreationMode.Private);
        private ISharedPreferencesEditor editor;
        private string verificationId { get; set; }
        private string phone { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            phone = Arguments.GetString(Constants.PHONE_KEY);
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

            toolbarMain.NavigationClick += (s1, e1) => Activity.OnBackPressed();
            editNumTv.Click += (s2, e2) => ChildFragmentManager.PopBackStackImmediate();
            resendBtn.Click += (s3, e3) => { };
            verifiyBtn.Click += (s4, e4) => VerifyCode(otpView.Value);

            resendBtnTimer = new CountdownTimer(30000, 1000, 
            onFinsh: () =>
            {
                resendBtn.Enabled = true;
                resendBtn.Text = "resend otp";
            }, onTick: (millisUntilFinished) =>
            {
                resendBtn.Text = "Resend OTP in " + millisUntilFinished / 1000;
            }).Start();
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
                if (!s.Exists() || !s.HasChildren)
                    GotoProfile();

                string stage = s.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Child(Constants.SESION_CHILD) != null ? s.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Child(Constants.SESION_CHILD).Value.ToString() : "";
                if (stage.Contains(Constants.REG_STAGE_DONE))
                {
                    editor = preferences.Edit();
                    editor.PutString("firstRun", "regd");
                    editor.Commit();

                    var intent = new Intent(Activity, typeof(MainActivity));
                    intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask);
                    StartActivity(intent);
                    OnboardingActivity.DismissLoader();
                }
                else
                {
                    OnboardingActivity.GetStage(stage);
                    OnboardingActivity.DismissLoader();
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
                        numTv.SetText(Resource.String.wrong_num_msg);
                        numTv.SetTextColor(ColorStateList.ValueOf(Color.Red));
                    }
                    
                }, onCodeSent: (code, token) =>
                {
                    verificationId = code;
                }));
        }
    }
}
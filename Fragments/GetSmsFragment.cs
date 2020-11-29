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
        //private CountDownTimer resendBtnTimer;
        private string verificationId { get; set; }
        private string phone { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            PreferenceHelper.Init(Context);
            phone = Arguments.GetString(Constants.PHONE_KEY);
            PhoneAuthProvider.Instance.VerifyPhoneNumber(
                phone, 
                30, 
                TimeUnit.Seconds, 
                Activity, 
                VerificationCallback);
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

            //resendBtnTimer = new CountdownTimer(30000, 1000, 
            //onFinsh: () =>
            //{
            //    resendBtn.Enabled = true;
            //    resendBtn.Text = "resend otp";
            //}, onTick: (millisUntilFinished) =>
            //{
            //    resendBtn.Text = "Resend OTP in " + millisUntilFinished / 1000;
            //}).Start();
        }

        private void VerifyCode(string code)
        {
            try
            {
                OnboardingActivity.Instance.ShowLoader();
                PhoneAuthCredential cred = PhoneAuthProvider.GetCredential(verificationId, code);
                SessionManager.GetFirebaseAuth().SignInWithCredential(cred)
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
                            OnboardingActivity.Instance.DismissLoader();
                            OnboardingActivity.Instance.ShowError(fiace.Source, fiace.Message);
                        }
                        catch (FirebaseTooManyRequestsException ftmre)
                        {
                            OnboardingActivity.Instance.DismissLoader();
                            OnboardingActivity.Instance.ShowError(ftmre.Source, ftmre.Message);
                        }
                        catch (FirebaseAuthInvalidUserException fiue)
                        {
                            OnboardingActivity.Instance.DismissLoader();
                            OnboardingActivity.Instance.ShowError(fiue.Source, fiue.Message);
                        }
                        catch (FirebaseNetworkException)
                        {
                            OnboardingActivity.Instance.DismissLoader();
                            OnboardingActivity.Instance.ShowNoNetDialog(false);
                        }
                        catch (Exception e)
                        {
                            OnboardingActivity.Instance.DismissLoader();
                            OnboardingActivity.Instance.ShowError(e.Source, e.Message);
                        }


                    }));
            }
            catch (Exception e)
            {
                OnboardingActivity.Instance.DismissLoader();
                OnboardingActivity.Instance.ShowError(e.Source, e.Message);

            }
            
        }

        private void CheckUserAvailability()
        {
            var statusRef = SessionManager.GetFireDB().GetReference("session");
            statusRef.OrderByKey().EqualTo(SessionManager.GetFirebaseAuth().CurrentUser.Uid).AddListenerForSingleValueEvent(
            new SingleValueListener((s) =>
            {
                if (!s.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Exists())
                {
                    GotoProfile();
                }
                else
                {
                    string stage = s.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Child(Constants.SESION_CHILD) != null ? 
                    s.Child(SessionManager.GetFirebaseAuth().CurrentUser.Uid).Child(Constants.SESION_CHILD).Value.ToString() : "";

                    if (stage.Contains(Constants.REG_STAGE_DONE))
                    {
                        SetUserAndGoHome();
                    }
                    else
                    {
                        OnboardingActivity.Instance.GetStage(stage);
                        OnboardingActivity.Instance.DismissLoader();
                    }
                }   

            }, (e) =>
            {
                OnboardingActivity.Instance.DismissLoader();
                OnboardingActivity.Instance.ShowError("Database error", e.Message);
            }));
        }

        private void SetUserAndGoHome()
        {
            PreferenceHelper.Instance.SetString("firstRun", "regd");

            string userId = SessionManager.GetFirebaseAuth().CurrentUser.Uid;
            var userRef = SessionManager.GetFireDB().GetReference($"users/{userId}/profile");
            userRef.AddValueEventListener(new SingleValueListener(
            onDataChange: (snapshot) =>
            {
                if (!snapshot.Exists())
                    return;


                PreferenceHelper.Instance.SetString(Constants.Username_Key, snapshot.Child(Constants.SNAPSHOT_FNAME) != null ? 
                    snapshot.Child(Constants.SNAPSHOT_FNAME).Value.ToString() : "");

                PreferenceHelper.Instance.SetString(Constants.Status_Key, snapshot.Child(Constants.SNAPSHOT_GENDER) != null ? 
                    snapshot.Child(Constants.SNAPSHOT_GENDER).Value.ToString() : "");

                PreferenceHelper.Instance.SetString(Constants.Profile_Url_Key, snapshot.Child(Constants.SNAPSHOT_PHOTO_URL) != null ? 
                    snapshot.Child(Constants.SNAPSHOT_PHOTO_URL).Value.ToString() : "");

                PreferenceHelper.Instance.SetString(Constants.Email_Key, snapshot.Child(Constants.SNAPSHOT_EMAIL) != null ? 
                    snapshot.Child(Constants.SNAPSHOT_EMAIL).Value.ToString() : "");

                PreferenceHelper.Instance.SetString(Constants.Phone_Key, snapshot.Child(Constants.SNAPSHOT_PHONE) != null ? 
                    snapshot.Child(Constants.SNAPSHOT_PHONE).Value.ToString() : "");

                OnboardingActivity.Instance.DismissLoader();
                var intent = new Intent(Activity, typeof(MainActivity));
                intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask);
                StartActivity(intent);
                

            }, onCancelled: (error) =>
            {
                Toast.MakeText(Context, error.Message, ToastLength.Short).Show();
            }));  
        }

        private void GotoProfile()
        {
            OnboardingActivity.Instance.SetStatus(Constants.REG_STAGE_CREATE_PROFILE);
            OnboardingActivity.Instance.DismissLoader();
            ParentFragmentManager.BeginTransaction()
                .Replace(Resource.Id.frag_container, new CreateProfileFragment())
                .CommitAllowingStateLoss();
        }

        private PhoneVerificationCallbacks VerificationCallback => new PhoneVerificationCallbacks
            (
                onVerificationCompleted: (cred) =>
                {
                    var code = cred.SmsCode;
                    if (string.IsNullOrEmpty(code))
                        return;

                    otpView.Value = code;
                    VerifyCode(code);
                },
                onVerificationFailed: (e) =>
                {
                    try
                    {
                        throw e;
                    }
                    catch (FirebaseNetworkException)
                    {
                        OnboardingActivity.Instance.ShowNoNetDialog(false);
                    }
                    catch (FirebaseTooManyRequestsException ftmre)
                    {
                        OnboardingActivity.Instance.ShowError(ftmre.Source, ftmre.Message);
                    }
                    catch (FirebaseAuthInvalidCredentialsException faice)
                    {
                        OnboardingActivity.Instance.ShowError(faice.Source, faice.Message);
                    }
                    catch (FirebaseAuthInvalidUserException fiue)
                    {
                        OnboardingActivity.Instance.ShowError(fiue.Source, fiue.Message);
                    }
                    catch (Exception ex)
                    {
                        OnboardingActivity.Instance.ShowError(ex.Source, ex.Message);
                    }
                    finally
                    {
                        resendBtn.Visibility = ViewStates.Invisible;
                        numTv.SetText(Resource.String.wrong_num_msg);
                        numTv.SetTextColor(ColorStateList.ValueOf(Color.Red));
                    }

                },
                onCodeSent: (code, token) =>
                {
                    verificationId = code;
                }
            );
    }
}
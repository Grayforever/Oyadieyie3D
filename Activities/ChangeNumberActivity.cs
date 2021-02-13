using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Com.Goodiebag.Pinview;
using Com.Mukesh.CountryPickerLib;
using Firebase.Auth;
using Google.Android.Material.AppBar;
using Google.Android.Material.Button;
using Java.Util;
using Java.Util.Concurrent;
using Oyadieyie3D.Callbacks;
using Oyadieyie3D.Events;
using Oyadieyie3D.HelperClasses;
using System;
using CoordinatorLayout = AndroidX.CoordinatorLayout.Widget.CoordinatorLayout;
using CountryPickerListener = Com.Mukesh.CountryPickerLib.Listeners.CountryPickerListener;
using TextInputEditText = Google.Android.Material.TextField.TextInputEditText;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using R = Oyadieyie3D.Resource;

namespace Oyadieyie3D.Activities
{
    [Register("id.Oyadieyie3D.Activities.ChangeNumberActivity")]
    [Activity(Label = "Change number", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class ChangeNumberActivity : AppCompatActivity
    {
        private bool isNew = false;
        private CountryPicker picker;
        private MaterialButton nextBtn;
        private TextInputEditText newPhoneNumEt;
        private AppCompatAutoCompleteTextView newDialcodeEt;
        private TextInputEditText oldPhoneNumEt;
        private AppCompatAutoCompleteTextView oldDialcodeEt;
        private FirebaseUser user;
        private string verificationId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.change_number);
            InitControls();
        }

        private void InitControls()
        {
            var appbar = FindViewById<AppBarLayout>(R.Id.change_appbar);
            var toolbar = appbar.FindViewById<Toolbar>(R.Id.main_toolbar);
            var coordRoot = FindViewById<CoordinatorLayout>(R.Id.change_root);
            nextBtn = FindViewById<MaterialButton>(R.Id.confirm_change_btn);
            var otpView = FindViewById<Pinview>(R.Id.change_otp_tv);
            newPhoneNumEt = FindViewById<TextInputEditText>(R.Id.phone_number_et_2);
            newDialcodeEt = FindViewById<AppCompatAutoCompleteTextView>(R.Id.dialcode_et_2);
            oldPhoneNumEt = FindViewById<TextInputEditText>(R.Id.phone_number_et);
            oldDialcodeEt = FindViewById<AppCompatAutoCompleteTextView>(R.Id.dialcode_et);
            var confirmOtpBtn = FindViewById<MaterialButton>(R.Id.confirm_otp_btn);
            var logoutBtn = FindViewById<MaterialButton>(R.Id.Log_out_btn);

            otpView.SetPinBackgroundRes(AppCompatDelegate.DefaultNightMode == AppCompatDelegate.ModeNightYes ? R.Color.gray_dark : R.Color.gray_light);

            coordRoot.RequestFocus();
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            user = SessionManager.GetFirebaseAuth().CurrentUser;

            var builder = new CountryPicker.Builder()
               .With(this)
               .Listener(new CountryPickerListener(
                   (c) =>
                   {
                       switch (isNew)
                       {

                           case true:
                               newDialcodeEt.Text = c.DialCode;
                               break;
                           default:
                               oldDialcodeEt.Text = c.DialCode;
                               break;
                       }

                   }));

            picker = builder.Build();
            var country = picker.CountryFromSIM != null ? picker.CountryFromSIM : picker.GetCountryByLocale(Locale.Us);
            newDialcodeEt.Text = country.DialCode;
            oldDialcodeEt.Text = country.DialCode;

            oldDialcodeEt.Click += OldDialcodeEt_Click;
            newDialcodeEt.Click += NewDialcodeEt_Click;

            oldPhoneNumEt.TextChanged += PhoneNumEt_TextChanged;
            newPhoneNumEt.TextChanged += PhoneNumEt_TextChanged;

            otpView.PinViewEvent += (s1, e1) =>
            {

            };
            nextBtn.Click += (s2, e2) =>
            {
                var oldphone = oldDialcodeEt.Text + oldPhoneNumEt.Text;
                var phone = newDialcodeEt.Text + newPhoneNumEt.Text;

                if (!oldphone.Equals(user.PhoneNumber))
                {
                    oldPhoneNumEt.Error = "this is not your current number";
                }
                else
                {
                    var callback = new PhoneVerificationCallbacks(
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
                            catch (Exception)
                            {

                            }

                        }, onCodeSent: (verificationId, token) =>
                        {
                            this.verificationId = verificationId;
                        });

                    PhoneAuthOptions options = PhoneAuthOptions.NewBuilder(SessionManager.GetFirebaseAuth())
                    .SetPhoneNumber(phone)
                    .SetTimeout((Java.Lang.Long)30L, TimeUnit.Seconds)
                    .SetActivity(this)
                    .SetCallbacks(callback)
                    .Build();

                    PhoneAuthProvider.VerifyPhoneNumber(options);

                }
            };
            confirmOtpBtn.Click += (s, e) => VerifyCode(otpView.Value);

            var confirmClick = new SweetConfirmClick(
                    (s) =>
                    {
                        s.DismissWithAnimation();
                        SessionManager.GetFirebaseAuth().SignOut();
                        PreferenceHelper.Instance.ClearPrefs();

                        var i = new Intent(this, typeof(OnboardingActivity));
                        i.AddFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask);
                        StartActivity(i);
                    });

            logoutBtn.Click += (s3, e3) => MainActivity.Instance.ShowWarning(this, "Sign out", "Are you sure you want to sign out?", confirmClick);
        }


        private void VerifyCode(string code)
        {
            try
            {
                var cred = PhoneAuthProvider.GetCredential(verificationId, code);
                user.UpdatePhoneNumber(cred)
                    .AddOnCompleteListener(new OncompleteListener(async (t) =>
                    {
                        switch (t.IsSuccessful)
                        {
                            case false:
                                throw t.Exception;

                            default:
                                var updateRef = SessionManager.GetFireDB().GetReference($"users/{user.Uid}/profile");
                                await updateRef.Child("phone").SetValueAsync(SessionManager.GetFirebaseAuth().CurrentUser.PhoneNumber);
                                break;
                        }
                    }));
            }
            catch (Exception e)
            {
                Toast.MakeText(this, "some error occured: " + e.Message, ToastLength.Long).Show();
            }
        }

        private void PhoneNumEt_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            nextBtn.Enabled = oldPhoneNumEt.Text.Length > 7 && newPhoneNumEt.Text.Length > 7;
        }

        private void NewDialcodeEt_Click(object sender, EventArgs e)
        {
            isNew = true;
            picker.ShowDialog(this);
        }

        private void OldDialcodeEt_Click(object sender, EventArgs e)
        {
            isNew = false;
            picker.ShowDialog(this);
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }
    }
}
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using CN.Pedant.SweetAlert;
using Oyadieyie3D.Events;
using Oyadieyie3D.Fragments;
using Oyadieyie3D.HelperClasses;
using Ramotion.PaperOnboarding;
using System.Collections.Generic;
using R = Oyadieyie3D.Resource;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, 
        WindowSoftInputMode = SoftInput.AdjustResize)]
    public class OnboardingActivity : AppCompatActivity
    {
        public static NoNetBottomSheet noNetBottomSheet = null;
        private static SweetAlertDialog loaderDialog;
        private static SweetAlertDialog infoDialog;

        public static OnboardingActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.onboarding_activity);
            Instance = this;
            GetStage(base.Intent.GetStringExtra(Constants.SESION_CHILD));
        }

        private void SetStatusBarImmersiveMode()
        {
            Window win = Window;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                win.AddFlags(WindowManagerFlags.TranslucentStatus);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                WindowManagerLayoutParams wmlp = win.Attributes;
                wmlp.Flags &= ~WindowManagerFlags.TranslucentStatus;
                win.SetStatusBarColor(Color.Transparent);
            }
        }

        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            SetStatusBarImmersiveMode();
        }

        public void GetStage(string stage)
        {
            switch (stage)
            {
                case Constants.REG_STAGE_CREATE_PROFILE:
                    SetFragment(new CreateProfileFragment());
                    break;
                case Constants.REG_STAGE_SET_PARTNER:
                    SetFragment(new PartnerFragment());
                    break;
                default:
                    SetFragment(InitWalkThrough());
                    break;
            }
        }

        public AndroidX.Fragment.App.Fragment InitWalkThrough()
        {
            var onboardingFragment = PaperOnboardingFragment.NewInstance(new List<PaperOnboardingPage>
            {
                new PaperOnboardingPage(Constants.ONBDTITLE_1, Constants.ONBDESC_1, Color.ParseColor("#EF9A9A"), 
                R.Drawable.tailor, R.Drawable.shopping_cart),

                new PaperOnboardingPage(Constants.ONBDTITLE_2, Constants.ONBDESC_2, Color.ParseColor("#F48FB1"), 
                R.Drawable.tailor_with_client2, R.Drawable.shopping_cart),

                new PaperOnboardingPage(Constants.ONBDTITLE_3, Constants.ONBDESC_3, Color.ParseColor("#CE93D8"), 
                R.Drawable.pay, R.Drawable.wallet)
            });

            onboardingFragment.SetOnRightOutListener(new OnRightOutListener(
                ()=> 
                {
                    SetFragment(new OnboardingFragment());
                    Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                }));

            return onboardingFragment;
        }

        private void SetFragment(AndroidX.Fragment.App.Fragment fragment)
        {
            SupportFragmentManager.BeginTransaction()
                .Replace(R.Id.frag_container, fragment)
                .CommitAllowingStateLoss();
        }

        public void SetStatus(string status)
        {
            DismissLoader();
            var statusRef = SessionManager.GetFireDB().GetReference($"{Constants.REF_USER_SESSION}/{SessionManager.UserId}");
            statusRef.Child("status").SetValueAsync(status);
        }

        public override void OnBackPressed() => base.OnBackPressed();

        public void ShowNoNetDialog(bool flag)
        {
            if (flag)
                return;

            noNetBottomSheet = new NoNetBottomSheet(this);
            noNetBottomSheet.Cancelable = false;
            AndroidX.Fragment.App.FragmentTransaction ft = SupportFragmentManager.BeginTransaction();
            ft.Add(noNetBottomSheet, "no_net");
            ft.CommitAllowingStateLoss();
        }

        public void DismissLoader()
        {
            if (!loaderDialog.IsShowing || loaderDialog == null)
                return;

            loaderDialog.DismissWithAnimation();
        }

        public void ShowError(string title, string message)
        {
            var errorDialog = new SweetAlertDialog(this, SweetAlertDialog.ErrorType);
            errorDialog.SetTitleText(title);
            errorDialog.SetContentText(message);
            errorDialog.SetConfirmText(Constants.DIALOG_OK);
            errorDialog.ShowCancelButton(false);
            errorDialog.SetConfirmClickListener(null);
            errorDialog.Show();
        }

        public void ShowLoader()
        {
            loaderDialog = new SweetAlertDialog(this, SweetAlertDialog.ProgressType);
            loaderDialog.SetTitleText(Constants.DIALOG_LOADING);
            loaderDialog.ShowCancelButton(false);
            loaderDialog.Show();
        }

        public void ShowInfo(string info)
        {
            infoDialog = new SweetAlertDialog(this, SweetAlertDialog.NormalType);
            infoDialog.SetTitleText("Info");
            infoDialog.SetContentText(info);
            infoDialog.SetConfirmText(Constants.DIALOG_OK);
            infoDialog.ShowCancelButton(false);
            infoDialog.SetConfirmClickListener(null);
            infoDialog.Show();
        }

        public void ShowWarning(string warn)
        {
            var warnDialog = new SweetAlertDialog(this, SweetAlertDialog.WarningType);
            warnDialog.SetTitleText("Warning");
            warnDialog.SetContentText(warn);
            warnDialog.SetConfirmText(Constants.DIALOG_OK);
            warnDialog.ShowCancelButton(false);
            warnDialog.SetConfirmClickListener(null);
            warnDialog.Show();
        }

        public void ShowSuccess()
        {
            var successDialog = new SweetAlertDialog(this, SweetAlertDialog.SuccessType);
            successDialog.SetTitleText(Constants.DIALOG_SUCCESS);
            successDialog.SetConfirmText(Constants.DIALOG_OK);
            successDialog.ShowCancelButton(false);
            successDialog.SetConfirmClickListener(null);
            successDialog.Show();
        }

        
    }
}
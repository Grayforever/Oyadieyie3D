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
using Ramotion.PaperOnboarding.Listeners;
using System.Collections.Generic;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class OnboardingActivity : AppCompatActivity
    {
        public static NoNetBottomSheet noNetBottomSheet = null;
        private static AppCompatActivity _context;
        private static SweetAlertDialog loaderDialog;
        private static SweetAlertDialog infoDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.onboarding_activity);
            _context = this;
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

        public static void GetStage(string stage)
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

        private static AndroidX.Fragment.App.Fragment InitWalkThrough()
        {
            var onboardingFragment = PaperOnboardingFragment.NewInstance(new List<PaperOnboardingPage>
            {
                new PaperOnboardingPage(Constants.ONBDTITLE_1, Constants.ONBDESC_1, Color.ParseColor("#7C4DFF"), Resource.Drawable.hotels, Resource.Drawable.key),
                new PaperOnboardingPage(Constants.ONBDTITLE_2, Constants.ONBDESC_2, Color.ParseColor("#FF4081"), Resource.Drawable.banks, Resource.Drawable.shopping_cart),
                new PaperOnboardingPage(Constants.ONBDTITLE_3, Constants.ONBDESC_3, Color.ParseColor("#FF6E40"), Resource.Drawable.stores, Resource.Drawable.wallet)
            });
            onboardingFragment.SetOnRightOutListener(new OnRightOutListener(
                ()=> 
                {
                    SetFragment(new OnboardingFragment());
                    _context.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    _context.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                }));
            return onboardingFragment;
        }

        private static void SetFragment(AndroidX.Fragment.App.Fragment fragment)
        {
            _context.SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.frag_container, fragment)
                .CommitAllowingStateLoss();
        }

        public static void SetStatus(string status)
        {
            DismissLoader();
            var statusRef = SessionManager.GetFireDB().GetReference($"{Constants.REF_USER_SESSION}/{SessionManager.UserId}");
            statusRef.Child("status").SetValueAsync(status);
        }

        public override void OnBackPressed() => base.OnBackPressed();

        public static void ShowNoNetDialog(bool val)
        {
            if (val != true)
            {
                noNetBottomSheet = new NoNetBottomSheet(_context);
                noNetBottomSheet.Cancelable = false;
                AndroidX.Fragment.App.FragmentTransaction ft = _context.SupportFragmentManager.BeginTransaction();
                ft.Add(noNetBottomSheet, "no_net");
                ft.CommitAllowingStateLoss();
            }
            else
            {
                return;
            }
        }

        public static void DismissLoader()
        {
            if (!loaderDialog.IsShowing || loaderDialog == null)
                return;

            loaderDialog.DismissWithAnimation();
        }

        public static void ShowError(string title, string message)
        {
            var errorDialog = new SweetAlertDialog(_context, SweetAlertDialog.ErrorType);
            errorDialog.SetTitleText(title);
            errorDialog.SetContentText(message);
            errorDialog.SetConfirmText(Constants.DIALOG_OK);
            errorDialog.ShowCancelButton(false);
            errorDialog.SetConfirmClickListener(null);
            errorDialog.Show();
        }

        public static void ShowLoader()
        {
            loaderDialog = new SweetAlertDialog(_context, SweetAlertDialog.ProgressType);
            loaderDialog.SetTitleText(Constants.DIALOG_LOADING);
            loaderDialog.ShowCancelButton(false);
            loaderDialog.Show();
        }

        public static void ShowInfo(string info)
        {
            infoDialog = new SweetAlertDialog(_context, SweetAlertDialog.NormalType);
            infoDialog.SetTitleText("Info");
            infoDialog.SetContentText(info);
            infoDialog.SetConfirmText(Constants.DIALOG_OK);
            infoDialog.ShowCancelButton(false);
            infoDialog.SetConfirmClickListener(null);
            infoDialog.Show();
        }

        public static void ShowWarning(string warn)
        {
            var warnDialog = new SweetAlertDialog(_context, SweetAlertDialog.WarningType);
            warnDialog.SetTitleText("Warning");
            warnDialog.SetContentText(warn);
            warnDialog.SetConfirmText(Constants.DIALOG_OK);
            warnDialog.ShowCancelButton(false);
            warnDialog.SetConfirmClickListener(null);
            warnDialog.Show();
        }

        public static void ShowSuccess()
        {
            var successDialog = new SweetAlertDialog(_context, SweetAlertDialog.SuccessType);
            successDialog.SetTitleText(Constants.DIALOG_SUCCESS);
            successDialog.SetConfirmText(Constants.DIALOG_OK);
            successDialog.ShowCancelButton(false);
            successDialog.SetConfirmClickListener(null);
            successDialog.Show();
        }

        
    }
}
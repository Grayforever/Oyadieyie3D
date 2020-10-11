using Android.App;
using Android.Graphics;
using Android.OS;
using AndroidX.AppCompat.App;
using CN.Pedant.SweetAlert;
using Oyadieyie3D.Fragments;
using Ramotion.PaperOnboarding;
using Ramotion.PaperOnboarding.Listeners;
using System.Collections.Generic;

namespace Oyadieyie3D.Activities
{
    [Activity(Label ="@string/app_name", Theme ="@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, 
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class OnboardingActivity : AppCompatActivity, IPaperOnboardingOnRightOutListener
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
            //GetStage(base.Intent.GetStringExtra(SplashActivity.REG_STATUS));

            PaperOnboardingPage scr1 = new PaperOnboardingPage("Hotels", "All hotels and hostels are sorted by hospitality rating", Color.ParseColor("#678FB4"), Resource.Drawable.ic_whatsapp, Resource.Drawable.ic_whatsapp);
            PaperOnboardingPage scr2 = new PaperOnboardingPage("Banks", "We carefully verify all banks before add them into the app", Color.ParseColor("#65B0B4"), Resource.Drawable.ic_whatsapp, Resource.Drawable.ic_whatsapp);
            PaperOnboardingPage scr3 = new PaperOnboardingPage("Stores", "All local stores are categorized for your convenience", Color.ParseColor("#9B90BC"), Resource.Drawable.ic_whatsapp, Resource.Drawable.ic_whatsapp);

            List<PaperOnboardingPage> elements = new List<PaperOnboardingPage>();
            elements.Add(scr1);
            elements.Add(scr2);
            elements.Add(scr3);

            var onboardingFragment = PaperOnboardingFragment.NewInstance(elements);
            onboardingFragment.SetOnRightOutListener(this);

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.frag_container, onboardingFragment)
                .CommitAllowingStateLoss();
        }

        public void OnRightOut()
        {
            GetStage(base.Intent.GetStringExtra(SplashActivity.REG_STATUS));
        }

        private void GetStage(string stage)
        {
            switch (stage)
            {
                case "create_profile":
                    SetFragment(new CreateProfileFragment());
                    break;
                case "set_partner":
                    SetFragment(new PartnerFragment());
                    break;
                default:
                    SetFragment(new OnboardingFragment());
                    break;
            }
        }

        private void SetFragment(AndroidX.Fragment.App.Fragment fragment)
        {
            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.frag_container, fragment)
                .CommitAllowingStateLoss();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

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
            if (!loaderDialog.IsShowing)
                return;

            loaderDialog.DismissWithAnimation();
        }

        public static void ShowError(string title, string message)
        {
            var errorDialog = new SweetAlertDialog(_context, SweetAlertDialog.ErrorType);
            errorDialog.SetTitleText(title);
            errorDialog.SetContentText(message);
            errorDialog.SetConfirmText("OK");
            errorDialog.ShowCancelButton(false);
            errorDialog.SetConfirmClickListener(null);
            errorDialog.Show();
        }

        public static void ShowLoader()
        {
            loaderDialog = new SweetAlertDialog(_context, SweetAlertDialog.ProgressType);
            loaderDialog.SetTitleText("Loading");
            loaderDialog.ShowCancelButton(false);
            loaderDialog.Show();
        }

        public static void ShowInfo(string info)
        {
            infoDialog = new SweetAlertDialog(_context, SweetAlertDialog.NormalType);
            infoDialog.SetTitleText("Info");
            infoDialog.SetContentText(info);
            infoDialog.SetConfirmText("OK");
            infoDialog.ShowCancelButton(false);
            infoDialog.SetConfirmClickListener(null);
            infoDialog.Show();
        }

        public static void ShowWarning(string warn)
        {
            var warnDialog = new SweetAlertDialog(_context, SweetAlertDialog.WarningType);
            warnDialog.SetTitleText("Warning");
            warnDialog.SetContentText(warn);
            warnDialog.SetConfirmText("OK");
            warnDialog.ShowCancelButton(false);
            warnDialog.SetConfirmClickListener(null);
            warnDialog.Show();
        }

        public static void ShowSuccess()
        {
            var successDialog = new SweetAlertDialog(_context, SweetAlertDialog.SuccessType);
            successDialog.SetTitleText("Success");
            successDialog.SetConfirmText("OK");
            successDialog.ShowCancelButton(false);
            successDialog.SetConfirmClickListener(null);
            successDialog.Show();
        }

        
    }
}
using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using Google.Android.Material.Button;
using System;

namespace Oyadieyie3D.Fragments
{
    public class OnboardingFragment : Fragment
    {
        public event EventHandler OnRegisterBtnClick;
        public event EventHandler OnSignInBtnClick;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.onboarding_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var registerBtn = view.FindViewById<MaterialButton>(Resource.Id.onboard_register_btn);
            var signInBtn = view.FindViewById<MaterialButton>(Resource.Id.onboard_signin_btn);

            registerBtn.Click += RegisterBtn_Click;
            signInBtn.Click += SignInBtn_Click;
        }

        private void SignInBtn_Click(object sender, System.EventArgs e)
        {
            OnSignInBtnClick?.Invoke(this, new EventArgs());
        }

        private void RegisterBtn_Click(object sender, System.EventArgs e)
        {
            OnRegisterBtnClick?.Invoke(this, new EventArgs());
        }
    }
}
using Android.OS;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Button;
using Mukesh;
using Oyadieyie3D.Utils;
using System;

namespace Oyadieyie3D.Fragments
{
    public class GetSmsFragment : AndroidX.Fragment.App.Fragment, IOnOtpCompletionListener
    {
        private OtpView otpView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.get_sms_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            otpView = view.FindViewById<OtpView>(Resource.Id.otp_view);
            var verifyBtn = view.FindViewById<MaterialButton>(Resource.Id.verify_otp_btn);
            var numTv = view.FindViewById<TextView>(Resource.Id.get_sms_sub2);
            SetSpan(numTv);
            verifyBtn.Click += VerifyBtn_Click;
            otpView.SetOtpCompletionListener(this);
        }

        private void SetSpan(TextView numTv)
        {
            SpannableString ss = new SpannableString($"Enter one time password sent on {0203870541}");
            var cs = new MyClickableSpan(((widget) => 
            { 
                
            }));
            

        }

        private void VerifyBtn_Click(object sender, System.EventArgs e)
        {
            ParentFragmentManager.BeginTransaction()
                .Replace(Resource.Id.frag_container, new PartnerFragment())
                .CommitAllowingStateLoss();
        }

        public void OnOtpCompleted(string otp)
        {

        }
    }
}
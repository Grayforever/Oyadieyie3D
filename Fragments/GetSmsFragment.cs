using Android.OS;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Com.Goodiebag.Pinview;
using Google.Android.Material.Button;
using Oyadieyie3D.Utils;
using System;
using static Com.Goodiebag.Pinview.Pinview;

namespace Oyadieyie3D.Fragments
{
    public class GetSmsFragment : AndroidX.Fragment.App.Fragment, IPinViewEventListener
    {
        private Pinview otpView;
        private MaterialButton verifyBtn;

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
            otpView = view.FindViewById<Pinview>(Resource.Id.otp_tv);
            verifyBtn = view.FindViewById<MaterialButton>(Resource.Id.verify_otp_btn);
            var numTv = view.FindViewById<TextView>(Resource.Id.get_sms_sub2);
            SetSpan(numTv);
            verifyBtn.Click += VerifyBtn_Click;
            otpView.SetPinViewEventListener(this);
        }

        public void OnDataEntered(Pinview p0, bool p1)
        {
            verifyBtn.Enabled = p0.Value.Length == 6;
        }

        private void SetSpan(TextView numTv)
        {
            string one = "Enter one time password sent on ";
            string two = "0203870543";
            SpannableString ss = new SpannableString(one + two);
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
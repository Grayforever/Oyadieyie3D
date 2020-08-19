using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using Google.Android.Material.TextField;
using System;

namespace Oyadieyie3D.Fragments
{
    public class OnboardingFragment : Fragment
    {
        private TextInputLayout phoneEt;
        private const string TAG = "phone_et";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.get_started_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            phoneEt = view.FindViewById<TextInputLayout>(Resource.Id.phone_et);
            var gsPhoneEt = view.FindViewById<TextInputEditText>(Resource.Id.gs_phone_et);
            gsPhoneEt.Click += GsPhoneEt_Click;
        }

        private void GsPhoneEt_Click(object sender, EventArgs e)
        {
            ParentFragmentManager.BeginTransaction()
                .AddSharedElement(phoneEt, "phone_et")
                .AddToBackStack(TAG)
                .Replace(Resource.Id.frag_container, new EnterPhoneFragment())
                .CommitAllowingStateLoss();
        }
    }
}
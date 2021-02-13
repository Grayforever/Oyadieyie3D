using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Transitions;
using Com.Mukesh.CountryPickerLib;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Java.Util;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Events;
using Constants = Oyadieyie3D.HelperClasses.Constants;
using R = Oyadieyie3D.Resource;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Oyadieyie3D.Fragments
{
    public class EnterPhoneFragment : AndroidX.Fragment.App.Fragment
    {
        private GetSmsFragment smsFragment = new GetSmsFragment();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SharedElementEnterTransition = TransitionInflater.From(Context).InflateTransition(Android.Resource.Transition.Move);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(R.Layout.enter_phone_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var toolbarMain = view.FindViewById<Toolbar>(R.Id.enter_phone_toolbar);
            var countryLinear = view.FindViewById<ConstraintLayout>(R.Id.cpicker_lin);
            var phoneEt = view.FindViewById<TextInputLayout>(R.Id.enter_phone_et);
            var nextBtn = view.FindViewById<MaterialButton>(R.Id.enter_cont_btn);
            var countryFlagIv = view.FindViewById<ImageView>(R.Id.cflag_iv);
            var dialcodeTv = view.FindViewById<TextView>(R.Id.dialcode_tv);

            var builder = new CountryPicker.Builder()
                .With(Context)
                .Listener(new CountryPickerListener(
                onSelectCountry: (c) =>
                {
                    countryFlagIv.SetImageResource(c.Flag);
                    dialcodeTv.Text = c.DialCode;
                    phoneEt.RequestFocus();
                }));

            var picker = builder.Build();

            //get country from sim card else default to US
            var country = picker.CountryFromSIM != null ? picker.CountryFromSIM : picker.GetCountryByLocale(Locale.Us);
            countryFlagIv.SetImageResource(country.Flag);
            dialcodeTv.Text = country.DialCode;
            phoneEt.RequestFocus();

            phoneEt.EditText.TextChanged += (s1, e1) => nextBtn.Enabled = phoneEt.EditText.Text.Length >= 8;
            countryLinear.Click += (s2, e2) => picker.ShowDialog(OnboardingActivity.Instance);
            toolbarMain.NavigationClick += (s2, e2) => Activity.OnBackPressed();

            nextBtn.Click += (s3, e3) => nextBtn.Post(() =>
            {
                var extras = new Bundle();
                var phone = dialcodeTv.Text + phoneEt.EditText.Text;
                extras.PutString(Constants.PHONE_KEY, phone);
                smsFragment.Arguments = extras;

                ParentFragmentManager.BeginTransaction()
                    .AddToBackStack(null)
                    .Replace(R.Id.frag_container, smsFragment)
                    .CommitAllowingStateLoss();
            });

        }
    }
}
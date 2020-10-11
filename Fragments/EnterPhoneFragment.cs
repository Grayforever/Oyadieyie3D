using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Transitions;
using CN.Pedant.SweetAlert;
using Com.Mukesh.CountryPickerLib;
using Google.Android.Material.AppBar;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Oyadieyie3D.Events;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Oyadieyie3D.Fragments
{
    public class EnterPhoneFragment : AndroidX.Fragment.App.Fragment
    {
        private CountryPicker.Builder builder;
        private CountryPicker picker;
        private TextInputLayout phoneEt;
        private MaterialButton nextBtn;
        private ImageView countryFlagIv;
        private TextView dialcodeTv;
        public static string phoneKey = "phoneNumber";
        private GetSmsFragment smsFragment = new GetSmsFragment();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SharedElementEnterTransition = TransitionInflater.From(Context).InflateTransition(Android.Resource.Transition.Move);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.enter_phone_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var appbarMain = view.FindViewById<AppBarLayout>(Resource.Id.enter_phone_appbar);
            var toolbarMain = appbarMain.FindViewById<Toolbar>(Resource.Id.main_toolbar);
            var countryLinear = view.FindViewById<ConstraintLayout>(Resource.Id.cpicker_lin);
            phoneEt = view.FindViewById<TextInputLayout>(Resource.Id.enter_phone_et);
            nextBtn = view.FindViewById<MaterialButton>(Resource.Id.enter_cont_btn);
            countryFlagIv = view.FindViewById<ImageView>(Resource.Id.cflag_iv);
            dialcodeTv = view.FindViewById<TextView>(Resource.Id.dialcode_tv);

            ((AppCompatActivity)Activity).SetSupportActionBar(toolbarMain);
            ((AppCompatActivity)Activity).SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbarMain.Title = "Enter phone";
            toolbarMain.NavigationClick += ToolbarMain_NavigationClick;

            
            builder = new CountryPicker.Builder().With(Context).Listener(new CountryPickerListener((c) =>
            {
                countryFlagIv.SetImageResource(c.Flag);
                dialcodeTv.Text = c.DialCode;
                phoneEt.RequestFocus();
            }));
            picker = builder.Build();

            var country = picker.CountryFromSIM;
            countryFlagIv.SetImageResource(country.Flag);
            dialcodeTv.Text = country.DialCode;
            phoneEt.RequestFocus();
            phoneEt.EditText.TextChanged += EditText_TextChanged;

            countryLinear.Click += CountryLinear_Click;

            nextBtn.Click += NextBtn_Click;
        }

        private void ToolbarMain_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e) => Activity.OnBackPressed();

        private void NextBtn_Click(object sender, System.EventArgs e)
        {
            nextBtn.Post(() =>
            {
                var extras = new Bundle();
                var phone = dialcodeTv.Text + phoneEt.EditText.Text;
                extras.PutString(phoneKey, phone);
                smsFragment.Arguments = extras;

                ParentFragmentManager.BeginTransaction()
                    .AddToBackStack(null)
                    .Replace(Resource.Id.frag_container, smsFragment)
                    .CommitAllowingStateLoss();
            });
        }

        
        private void EditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e) => CheckIfEmpty();

        private void CheckIfEmpty()
        {
            bool v = phoneEt.EditText.Text.Length >= 8;
            nextBtn.Enabled = v;
        }

        private void CountryLinear_Click(object sender, System.EventArgs e) => picker.ShowDialog(Activity);

        
    }
}
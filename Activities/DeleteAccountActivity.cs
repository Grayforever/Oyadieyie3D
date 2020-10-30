using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Com.Mukesh.CountryPickerLib;
using Com.Mukesh.CountryPickerLib.Listeners;
using Google.Android.Material.AppBar;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Java.Util;

namespace Oyadieyie3D.Activities
{
    [Register("id.Oyadieyie3D.Activities.DeleteAccountActivity")]
    [Activity(Label = "Delete Account", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class DeleteAccountActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.delete_account);
            InitWidgets();
        }

        private void InitWidgets()
        {
            var deleteBtn = FindViewById<MaterialButton>(Resource.Id.confirm_delete_btn);
            var changeNumBtn = FindViewById<MaterialButton>(Resource.Id.change_num_btn);
            var dialcodeEt = FindViewById<AppCompatAutoCompleteTextView>(Resource.Id.dialcode_et);
            var phoneNumEt = FindViewById<TextInputEditText>(Resource.Id.phone_number_et);
            var appbar = FindViewById<AppBarLayout>(Resource.Id.delete_appbar);
            var toolbar = appbar.FindViewById<Toolbar>(Resource.Id.main_toolbar);
            var coordRoot = FindViewById<CoordinatorLayout>(Resource.Id.delete_root);
            coordRoot.RequestFocus();

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.RequestFocus();

            var builder = new CountryPicker.Builder()
                .With(this)
                .Listener(new CountryPickerListener(
                actionOnSelectCountry: (c) =>
                {
                    dialcodeEt.Text = c.DialCode;
                    phoneNumEt.RequestFocus();
                }));

            var picker = builder.Build();
            var country = picker.CountryFromSIM != null ? picker.CountryFromSIM : picker.GetCountryByLocale(Locale.Us);
            dialcodeEt.Text = country.DialCode;

            dialcodeEt.Click += (s2, e2) =>
            {
                picker.ShowDialog(this);
            };

            phoneNumEt.TextChanged += (s1, e1) =>
            {
                deleteBtn.Enabled = phoneNumEt.Text.Length >= 8;
            };

            changeNumBtn.Click += (s3, e3) =>
            {
                var intent = new Intent(this, typeof(ChangeNumberActivity));
                StartActivity(intent);
            };
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }
    }
}
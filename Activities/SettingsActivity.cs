using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.App;
using AndroidX.Preference;
using BumpTech.GlideLib;
using BumpTech.GlideLib.Requests;
using DE.Hdodenhof.CircleImageViewLib;
using Google.Android.Material.AppBar;
using Oyadieyie3D.Fragments;
using Oyadieyie3D.Parcelables;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class SettingsActivity : AppCompatActivity, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        private ConstraintLayout profileConstraint;
        private CircleImageView profileIv;
        private Bundle extras;
        private ProfileParcelable parcelable;
        private TextView username_tv;
        private TextView status_tv;
        private string imgUrl;
        private string username;
        private string phone;
        private string email;
        private string status;
        private string[] userPro;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.settings_activity);
            extras = Intent.Extras;
            parcelable = (ProfileParcelable)extras.GetParcelable("extra_profile_data");

            username_tv = FindViewById<TextView>(Resource.Id.set_prof_name_tv);
            status_tv = FindViewById<TextView>(Resource.Id.set_prof_extras_tv);
            var appBar = FindViewById<AppBarLayout>(Resource.Id.settings_appbar);
            var toolbar = appBar.FindViewById<Toolbar>(Resource.Id.main_toolbar);
            profileConstraint = FindViewById<ConstraintLayout>(Resource.Id.profile_const);
            profileIv = FindViewById<CircleImageView>(Resource.Id.set_prof_iv);
            
            toolbar.Title = "Settings";
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.NavigationClick += Toolbar_NavigationClick;
            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.frag_container_s, new SettingsFragment())
                .CommitAllowingStateLoss();

            var prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            prefs.RegisterOnSharedPreferenceChangeListener(this);
            var isOnline = prefs.GetBoolean("online_switch", false);
            SetOnlineStatus(isOnline);

            profileConstraint.Click += ProfileConstraint_Click;

            RequestOptions requestOptions = new RequestOptions();
            requestOptions.Placeholder(Resource.Drawable.user);

            imgUrl = parcelable.UserProfile.ProfileImgUrl;
            username = parcelable.UserProfile.Username;
            phone = parcelable.UserProfile.Phone;
            email = parcelable.UserProfile.Email;
            status = parcelable.UserProfile.Status;
            userPro = new string[] { imgUrl, username, phone, status };

            Glide.With(this)
                .SetDefaultRequestOptions(requestOptions)
                .Load(imgUrl)
                .Into(profileIv);

            username_tv.Text = username;
            status_tv.Text = status;
        }

        private void SetOnlineStatus(bool isOnline)
        {
            Toast.MakeText(this, isOnline.ToString(), ToastLength.Short).Show();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.help_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Help me", ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }

        private void ProfileConstraint_Click(object sender, System.EventArgs e)
        {
            var intent = new Intent(this, typeof(ProfileActivity));
            intent.PutStringArrayListExtra("extra_details", userPro);
            ActivityOptionsCompat op = ActivityOptionsCompat.MakeSceneTransitionAnimation(this, profileIv, "profile_holder");
            StartActivity(intent, op.ToBundle());
        }

        private void Toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e) => OnBackPressed();

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            
        }
    }
}
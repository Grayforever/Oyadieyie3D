using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Google.Android.Material.AppBar;
using Oyadieyie3D.Fragments;
using AndroidX.AppCompat.Widget;
using AndroidX.ConstraintLayout.Widget;
using Android.Content;
using AndroidX.Core.App;
using DE.Hdodenhof.CircleImageViewLib;
using BumpTech.GlideLib.Requests;
using BumpTech.GlideLib;
using Oyadieyie3D.Parcelables;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class SettingsActivity : AppCompatActivity
    {
        private ConstraintLayout profileConstraint;
        private CircleImageView profileIv;
        private Bundle extras;
        private ProfileParcelable parcelable;
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
        }

        private void ProfileConstraint_Click(object sender, System.EventArgs e)
        {
            var intent = new Intent(this, typeof(ProfileActivity));
            intent.PutStringArrayListExtra("extra_details", userPro);
            ActivityOptionsCompat op = ActivityOptionsCompat.MakeSceneTransitionAnimation(this, profileIv, "profile_holder");
            StartActivity(intent, op.ToBundle());
        }

        private void Toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e) => OnBackPressed();
    }
}
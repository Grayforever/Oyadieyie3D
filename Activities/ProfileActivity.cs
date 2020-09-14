using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using BumpTech.GlideLib;
using DE.Hdodenhof.CircleImageViewLib;
using Google.Android.Material.FloatingActionButton;
using Oyadieyie3D.Fragments;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class ProfileActivity : AppCompatActivity
    {
        private FloatingActionButton camFab;
        private CircleImageView profileImageView;
        private ProfileChooserFragment profileChooserFrag;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profile_activity);
            var toolbar = FindViewById<Toolbar>(Resource.Id.profile_toolbar);
            camFab = FindViewById<FloatingActionButton>(Resource.Id.cam_fab);
            profileImageView = FindViewById<CircleImageView>(Resource.Id.prof_prof_iv);
            toolbar.Title = "Profile";
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.NavigationClick += Toolbar_NavigationClick;
            camFab.Click += CamFab_Click;
            profileChooserFrag = new ProfileChooserFragment();
            profileChooserFrag.OnCropComplete += ProfileChooserFrag_OnCropComplete;
        }

        private void ProfileChooserFrag_OnCropComplete(object sender, ProfileChooserFragment.CropCompleteEventArgs e)
        {
            Glide.With(this).Load(e.imageUri).Into(profileImageView);
        }

        private void CamFab_Click(object sender, System.EventArgs e)
        {
            var ft = SupportFragmentManager.BeginTransaction();
            ft.Add(profileChooserFrag, "profile_chooser");
            ft.CommitAllowingStateLoss();
        }

        private void Toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e) => OnBackPressed();

        public override void OnBackPressed()
        { 
            camFab.Post(() =>
            {
                camFab.Hide();
            });
            base.OnBackPressed();
        }

        public override void OnEnterAnimationComplete()
        {
            base.OnEnterAnimationComplete();
            camFab.PostDelayed(() =>
            {
                camFab.Show();
            }, 1000);
        }
    }
}
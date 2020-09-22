using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.App;
using AndroidX.Core.View;
using BumpTech.GlideLib;
using BumpTech.GlideLib.Requests;
using CN.Pedant.SweetAlert;
using DE.Hdodenhof.CircleImageViewLib;
using Firebase.Storage;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Oyadieyie3D.Events;
using Oyadieyie3D.Fragments;
using Oyadieyie3D.HelperClasses;
using Oyadieyie3D.Models;
using Oyadieyie3D.Parcelables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = SoftInput.StateAlwaysHidden)]
    public class ProfileActivity : AppCompatActivity, View.IOnTouchListener
    {
        private FloatingActionButton camFab;
        private CircleImageView profileImageView;
        private TextInputLayout usernameEt;
        private TextInputLayout phoneEt;
        private TextInputLayout statusEt;
        private ProfileChooserFragment profileChooserFrag;
        private SweetAlertDialog loaderDialog;
        private User user;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profile_activity);
            var bundle = Intent.Extras;
            await LazyCreateUserAsync(bundle.GetStringArrayList("extra_details"));
            var toolbar = FindViewById<Toolbar>(Resource.Id.profile_toolbar);
            camFab = FindViewById<FloatingActionButton>(Resource.Id.cam_fab);
            profileImageView = FindViewById<CircleImageView>(Resource.Id.prof_prof_iv);
            usernameEt = FindViewById<TextInputLayout>(Resource.Id.prof_fname_et);
            phoneEt = FindViewById<TextInputLayout>(Resource.Id.prof_phone_et);
            statusEt = FindViewById<TextInputLayout>(Resource.Id.prof_about_et);
            toolbar.Title = "Profile";
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.NavigationClick += Toolbar_NavigationClick;
            camFab.Click += CamFab_Click;
            profileChooserFrag = new ProfileChooserFragment();
            profileChooserFrag.OnCropComplete += ProfileChooserFrag_OnCropComplete;

            RequestOptions requestOptions = new RequestOptions();
            requestOptions.Placeholder(Resource.Drawable.user);
            
            profileImageView.Click += ProfileImageView_Click;
 
            phoneEt.EditText.Text = user.Phone;
            usernameEt.EditText.Text = user.Username;
            statusEt.EditText.Text = user.Status;

            phoneEt.SetOnTouchListener(this);
            usernameEt.SetOnTouchListener(this);
            statusEt.SetOnTouchListener(this);

            Glide.With(this)
                .SetDefaultRequestOptions(requestOptions)
                .Load(user.ProfileImgUrl)
                .Into(profileImageView);
        }

        private async Task<User> LazyCreateUserAsync(IList<string> res)
        {
            await Task.Run(() =>
            {
                user = new User
                {
                    ProfileImgUrl = res[0],
                    ID = "",
                    Username = res[1],
                    Email = "",
                    Phone = res[2],
                    Status = res[3]
                };
            });
            return user;
        }

        private void ProfileImageView_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(FullscreenImageActivity));
            var pp = new ProfileParcelable();
            pp.UserProfile = user;
            intent.PutExtra("extra_transition_name", ViewCompat.GetTransitionName(profileImageView));
            intent.PutExtra("extra_post_data", pp);
            intent.PutExtra("parcel_type", 1);
            var options = ActivityOptionsCompat.MakeSceneTransitionAnimation(this, profileImageView,
                ViewCompat.GetTransitionName(profileImageView));
            StartActivity(intent, options.ToBundle());
        }

        private async void ProfileChooserFrag_OnCropComplete(object sender, ProfileChooserFragment.CropCompleteEventArgs e)
        {
            try
            {
                ShowLoading();
                var userId = SessionManager.GetFirebaseAuth().CurrentUser.Uid;
                StorageReference storageReference = FirebaseStorage.Instance.GetReference("userProfile/" + userId);

                var stream = new System.IO.MemoryStream();
                var bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, e.imageUri);
                await bitmap.CompressAsync(Bitmap.CompressFormat.Webp, 70, stream);
                var imgArray = stream.ToArray();
                storageReference.PutBytes(imgArray)
                    .AddOnCompleteListener(new OncompleteListener((t) =>
                    {
                        if (!t.IsSuccessful)
                            throw t.Exception;

                        Glide.With(this).Load(e.imageUri).Into(profileImageView);
                        DismissLoading();
                    }));
                
            }
            catch (Exception)
            {

            }
        }

        private void ShowLoading()
        {
            loaderDialog = new SweetAlertDialog(this, SweetAlertDialog.ProgressType);
            loaderDialog.SetTitleText("Loading");
            loaderDialog.ShowCancelButton(false);
            loaderDialog.Show();
        }

        private void DismissLoading()
        {
            if (!loaderDialog.IsShowing)
                return;

            loaderDialog.Dismiss();
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

        private void ShowEditSheet(int which)
        {
            var profileEdit = new ProfileEditSheet(which);
            var ft = SupportFragmentManager.BeginTransaction();
            ft.Add(profileEdit, "edit_text");
            ft.CommitAllowingStateLoss();
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (v.Id)
            {
                case Resource.Id.prof_phone_et:
                    ShowEditSheet(0);
                    break;
                case Resource.Id.prof_fname_et:
                    ShowEditSheet(1);
                    break;
                case Resource.Id.prof_about_et:
                    ShowEditSheet(2);
                    break;
            }

            return false;
        }
    }
}
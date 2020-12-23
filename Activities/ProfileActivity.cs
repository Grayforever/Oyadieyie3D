using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.App;
using AndroidX.Core.View;
using BumpTech.GlideLib;
using BumpTech.GlideLib.Requests;
using CN.Pedant.SweetAlert;
using DE.Hdodenhof.CircleImageViewLib;
using Firebase.Database;
using Firebase.Storage;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Oyadieyie3D.Adapters;
using Oyadieyie3D.Events;
using Oyadieyie3D.Fragments;
using Oyadieyie3D.HelperClasses;
using System;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = SoftInput.StateAlwaysHidden)]
    public class ProfileActivity : AppCompatActivity, View.IOnClickListener
    {
        private FloatingActionButton camFab;
        private CircleImageView profileImageView;
        private TextInputEditText usernameEditText;
        private TextInputEditText phoneEditText;
        private AppCompatAutoCompleteTextView statusEditText;
        private DatabaseReference statusUpdateRef;
        private string img_url;
        private ProfileChooserFragment profileChooserFrag;
        private SweetAlertDialog loaderDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profile_activity);

            var toolbar = FindViewById<Toolbar>(Resource.Id.profile_toolbar);
            camFab = FindViewById<FloatingActionButton>(Resource.Id.cam_fab);
            profileImageView = FindViewById<CircleImageView>(Resource.Id.prof_prof_iv);
            var usernameEt = FindViewById<TextInputLayout>(Resource.Id.prof_fname_et);
            var phoneEt = FindViewById<TextInputLayout>(Resource.Id.prof_phone_et);
            var statusEt = FindViewById<TextInputLayout>(Resource.Id.prof_about_et);
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


            phoneEditText = phoneEt.FindViewById<TextInputEditText>(Resource.Id.phone_edittext);
            usernameEditText = usernameEt.FindViewById<TextInputEditText>(Resource.Id.name_edittext);
            statusEditText = statusEt.FindViewById<AppCompatAutoCompleteTextView>(Resource.Id.status_autocomplete);

            statusUpdateRef = SessionManager.GetFireDB().GetReference($"users/{SessionManager.UserId}/profile");
            img_url = PreferenceHelper.Instance.GetString("profile_url", "");
            phoneEditText.Text = PreferenceHelper.Instance.GetString("phone", "");
            usernameEditText.Text = PreferenceHelper.Instance.GetString("username", "");
            statusEditText.Text = PreferenceHelper.Instance.GetString("status", "");
            statusEditText.Adapter = ArrayAdapterClass.CreateArrayAdapter(this, new string[] { "Available", "Away", "Leave a message", "Busy", "Closed"});
            statusEditText.ItemClick += (s1, e1) =>
            {
                var status = e1.Parent.GetItemAtPosition(e1.Position).ToString();
                
                statusUpdateRef.Child("gender").SetValue(status).AddOnCompleteListener(new OncompleteListener(
                (t) =>
                {
                    if (t.IsSuccessful)
                    {
                        PreferenceHelper.Instance.SetString("status", status);
                    }
                    else
                    {
                        Toast.MakeText(this, t.Exception.Message, ToastLength.Short).Show();
                    }  
                }));
                
            };


            phoneEditText.SetOnClickListener(this);
            usernameEditText.SetOnClickListener(this);

            Glide.With(this)
                .SetDefaultRequestOptions(requestOptions)
                .Load(img_url)
                .Into(profileImageView);
        }

        private void ProfileImageView_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(FullscreenImageActivity));
            intent.PutExtra("img_url", img_url);
            intent.PutExtra(Constants.TRANSITION_NAME, ViewCompat.GetTransitionName(profileImageView));

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

        public override void OnBackPressed() => base.OnBackPressed();

        public override void OnEnterAnimationComplete()
        {
            base.OnEnterAnimationComplete();
            camFab.PostDelayed(() =>
            {
                camFab.Show();
            }, 2000);
        }

        private void ShowEditSheet(int which)
        {
            
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.phone_edittext:
                    MainActivity.Instance.ShowWarning(this, "Change number", "Do you wish to change your number?", ConfirmYes());
                    break;
                case Resource.Id.name_edittext:
                    ShowEditSheet(0);
                    break;
            }
        }

        private SweetConfirmClick ConfirmYes()
        {
            var confirmClick = new SweetConfirmClick(
                    (s) =>
                    {
                        s.DismissWithAnimation();
                        var i = new Intent(this, typeof(ChangeNumberActivity));
                        StartActivity(i);
                    });
            return confirmClick;
        }
    }
}
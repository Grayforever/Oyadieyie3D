using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using BumpTech.GlideLib;
using IGreenWood.LoupeLib;
using Oyadieyie3D.Parcelables;
using Oyadieyie3D.HelperClasses;
using System;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class FullscreenImageActivity : AppCompatActivity
    {
        private ImageView imageView;
        private Toolbar fullToolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.fullscreen_imageviewer);
            imageView = (ImageView)FindViewById(Resource.Id.image);
            fullToolbar = (Toolbar)FindViewById(Resource.Id.fullscreen_toolbar);
            var container = FindViewById<FrameLayout>(Resource.Id.container);
            SetSupportActionBar(fullToolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var loupe = new Loupe(imageView, container);
            loupe.OnViewTranslateListener = new OnViewTranslateListener((v1) =>
            {
                SupportFinishAfterTransition();
            }, null, null, null);

            try
            {
                Bundle extras = Intent.Extras;
                int type = extras.GetInt(Constants.PARCEL_TYPE);
                switch (type)
                {
                    case 0:
                        PostParcelable postParcel = (PostParcelable)extras.GetParcelable(Constants.POST_DATA_EXTRA);
                        Glide.With(this).Load(postParcel.PostItem.DownloadUrl).Into(imageView);
                        break;
                    default:
                        ProfileParcelable profileParcel = (ProfileParcelable)extras.GetParcelable(Constants.PROFILE_DATA_EXTRA);
                        Glide.With(this).Load(profileParcel.UserProfile.ProfileImgUrl).Into(imageView);
                        break;
                }
                string imageTransitionName = extras.GetString(Constants.TRANSITION_NAME);
                imageView.TransitionName = imageTransitionName;
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Short).Show();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.fullscreen_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_whatsapp:
                    var intent = new Intent();
                    intent.PutExtra(Intent.ExtraText, "Intent hghghg");
                    intent.SetType("text/plain");
                    intent.SetPackage("com.whatsapp");
                    StartActivity(intent);
                    break;

                case Resource.Id.action_call:
                    break;

            }
            return base.OnOptionsItemSelected(item);
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return base.OnSupportNavigateUp();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        internal sealed class OnViewTranslateListener : Java.Lang.Object, Loupe.IOnViewTranslateListener
        {
            Action<ImageView> _onDismiss;
            Action<ImageView> _onRestore;
            Action<ImageView> _onStart;
            Action<ImageView, float> _onViewTranslate;
            public OnViewTranslateListener(Action<ImageView> onDismiss, Action<ImageView> onRestore, 
                Action<ImageView> onStart, Action<ImageView, float> onViewTranslate)
            {
                _onDismiss = onDismiss;
                _onRestore = onRestore;
                _onStart = onStart;
                _onViewTranslate = onViewTranslate;
            }
            public void OnDismiss(ImageView view)
            {
                _onDismiss?.Invoke(view);
            }

            public void OnRestore(ImageView view)
            {
                _onRestore?.Invoke(view);
            }

            public void OnStart(ImageView view)
            {
                _onStart?.Invoke(view);
            }

            public void OnViewTranslate(ImageView view, float amount)
            {
                _onViewTranslate?.Invoke(view, amount);
            }
        }
    }
}
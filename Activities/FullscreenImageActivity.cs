using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using BumpTech.GlideLib;
using IGreenWood.LoupeLib;
using Oyadieyie3D.Parcelables;
using System;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class FullscreenImageActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.fullscreen_imageviewer);
            ImageView imageView = (ImageView)FindViewById(Resource.Id.image);
            var loupe = new Loupe(imageView);
            loupe.UseDismissAnimation = false;
            loupe.OnViewTranslateListener = new OnViewTranslateListener
                (
                    onDismiss: (v1) =>
                    {
                        SupportFinishAfterTransition();
                    }, onRestore: (v2) =>
                    {

                    }, onStart: (v3) =>
                    {

                    }, onViewTranslate: (v4, f) =>
                    {

                    }
                );

            try
            {
                Bundle extras = Intent.Extras;
                int type = extras.GetInt("parcel_type");
                switch (type)
                {
                    case 0:
                        PostParcelable postParcel = (PostParcelable)extras.GetParcelable("extra_post_data");
                        Glide.With(this).Load(postParcel.PostItem.DownloadUrl).Into(imageView);
                        break;
                    default:
                        ProfileParcelable profileParcel = (ProfileParcelable)extras.GetParcelable("extra_post_data");
                        Glide.With(this).Load(profileParcel.UserProfile.ProfileImgUrl).Into(imageView);
                        break;
                }
                string imageTransitionName = extras.GetString("extra_transition_name");
                imageView.TransitionName = imageTransitionName;

            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Short).Show();
            }
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
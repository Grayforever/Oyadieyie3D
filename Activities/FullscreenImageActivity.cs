using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Bumptech.Glide;
using Bumptech.Glide.Load;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Bumptech.Glide.Request.Target;
using IGreenWood.LoupeLib;
using Oyadieyie3D.HelperClasses;
using System;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class FullscreenImageActivity : AppCompatActivity
    {
        private ImageView imageView;
        private FrameLayout container;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.fullscreen_imageviewer);
            PostponeEnterTransition();
            imageView = FindViewById<ImageView>(Resource.Id.image);
            container = FindViewById<FrameLayout>(Resource.Id.container);

            try
            {
                Bundle extras = Intent.Extras;
                string img_url = extras.GetString("img_url");
                string imageTransitionName = extras.GetString(Constants.TRANSITION_NAME);
                imageView.TransitionName = imageTransitionName;
                Glide.With(this).Load(img_url).Listener(RequestListener).Into(imageView);
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Short).Show();
            }
        }

        private GlideRequestListener RequestListener => new GlideRequestListener()
        {
            LoadFailed = (p0, p1, p2, p3) => { },
            ResourceReady = (p0, p1, p2, p3, p4) =>
            {
                StartPostponedEnterTransition();
                var loupe = new Loupe(imageView, container);
                loupe.UseFlingToDismissGesture = false;
                loupe.OnViewTranslateListener = new OnViewTranslateListener
                {
                    Dismiss = (ImageView v) => SupportFinishAfterTransition()
                };
            }
        };

        internal sealed class OnViewTranslateListener : Java.Lang.Object, Loupe.IOnViewTranslateListener
        {
            public Action<ImageView> Dismiss;

            public void OnDismiss(ImageView view)
            {
                Dismiss(view);
            }

            public void OnRestore(ImageView view)
            {

            }

            public void OnStart(ImageView view)
            {

            }

            public void OnViewTranslate(ImageView view, float amount)
            {

            }
        }

        internal sealed class GlideRequestListener : Java.Lang.Object, IRequestListener
        {
            public Action<GlideException, Java.Lang.Object, ITarget, bool> LoadFailed;
            public Action<Java.Lang.Object, Java.Lang.Object, ITarget, DataSource, bool> ResourceReady;

            public bool OnLoadFailed(GlideException p0, Java.Lang.Object p1, ITarget p2, bool p3)
            {
                LoadFailed(p0, p1, p2, p3);
                return false;
            }

            public bool OnResourceReady(Java.Lang.Object p0, Java.Lang.Object p1, ITarget p2, DataSource p3, bool p4)
            {
                ResourceReady(p0, p1, p2, p3, p4);
                return false;
            }
        }
    }
}
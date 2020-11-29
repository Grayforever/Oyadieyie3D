using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using BumpTech.GlideLib;
using BumpTech.GlideLib.Load;
using BumpTech.GlideLib.Load.Engines;
using BumpTech.GlideLib.Requests;
using BumpTech.GlideLib.Requests.Target;
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
            LoadFailed = () => { },
            ResourceReady = () =>
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
            public Action LoadFailed;
            public Action ResourceReady;

            public bool OnLoadFailed(GlideException e, Java.Lang.Object model, ITarget target, bool isFirstResource)
            {
                LoadFailed();
                return false;
            }

            public bool OnResourceReady(Java.Lang.Object resource, Java.Lang.Object model, ITarget target, DataSource dataSource, bool isFirstResource)
            {
                ResourceReady();
                return false;
            }
        }
    }
}
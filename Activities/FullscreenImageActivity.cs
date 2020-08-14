using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using IGreenWood.LoupeLib;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class FullscreenImageActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.fullscreen_imageviewer);
            
            Bundle extras = Intent.Extras;
            ImageView imageView = (ImageView)FindViewById(Resource.Id.image);
            string imageTransitionName = extras.GetString("extra_transition_name");
            imageView.TransitionName = imageTransitionName;

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
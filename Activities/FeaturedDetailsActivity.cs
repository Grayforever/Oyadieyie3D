using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Transitions;
using Android.Views;
using AndroidX.Annotations;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.Palette.Graphics;
using AndroidX.ViewPager2.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Resource.Drawable;
using Bumptech.Glide.Request.Target;
using Bumptech.Glide.Request.Transition;
using Google.Android.Material.AppBar;
using Google.Android.Material.ImageView;
using Google.Android.Material.Tabs;
using Oyadieyie3D.Adapters;
using System;
using static Android.Transitions.Transition;
using static AndroidX.Palette.Graphics.Palette;
using static Google.Android.Material.Tabs.TabLayoutMediator;
using R = Oyadieyie3D.Resource;

namespace Oyadieyie3D.Activities
{
    [Activity(Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class FeaturedDetailsActivity : AppCompatActivity
    {
        private string url;
        private CollapsingToolbarLayout collapsingToolbar;
        private Toolbar toolbar;
        private ViewPager2 viewPager;
        private ShapeableImageView imageView;
        internal static string BANNER_URL = "bannerUrl";
        internal static string SHARED_TRANS_NAME = "shared";
        private TabLayout tabLayout;
        private string[] tabTitle = { "Details", "Gallery", "Book" };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.featured_details);
            url = Intent.GetStringExtra(BANNER_URL);

            collapsingToolbar = FindViewById<CollapsingToolbarLayout>(R.Id.mycoll);
            collapsingToolbar.Animate();

            toolbar = FindViewById<Toolbar>(R.Id.featured_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            imageView = FindViewById<ShapeableImageView>(R.Id.collimg);
            ViewCompat.SetTransitionName(imageView, SHARED_TRANS_NAME);

            LoadImage();
            SetUpPager();
            SetupTabLayout();
        }

        private void SetupTabLayout()
        {
            tabLayout = FindViewById<TabLayout>(R.Id.featured_tablayout);
            new TabLayoutMediator(tabLayout, viewPager, new TabConfigStrategy 
            {
                ConfigureTab = (tab, position) =>
                {
                    tab.SetText(tabTitle[position]);
                    viewPager.SetCurrentItem(tab.Position, true);
                }
            }).Attach();
        }

        private void SetUpPager()
        {
            var pagerAdapter = new FeaturedPagerAdapter(this);
            viewPager = FindViewById<ViewPager2>(R.Id.featured_viewpager);

            pagerAdapter.AddFragment(new DetailsFragment());
            pagerAdapter.AddFragment(new GalleryFragment());
            pagerAdapter.AddFragment(new BookingFragment());
            viewPager.Adapter = pagerAdapter;
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }

        private void LoadImage()
        {
            Glide.With(this)
                .AsBitmap()
                .Load(url)
                .Transition(DrawableTransitionOptions.WithCrossFade())
                .Into(new CustomTargetView
                {
                    LoadCleared = (d) => { },
                    ResourceReady = (b, t) =>
                    {
                        imageView.SetImageBitmap(b);
                        var palette = Palette.From(b).Generate(new PaletteAsyncListener
                        {
                            Generated = (p) =>
                            {
                                var vibrant = p.VibrantSwatch;
                                if (vibrant != null)
                                {
                                    collapsingToolbar.SetContentScrimColor(vibrant.Rgb);
                                }
                            }
                        });
                    }
                });
        }

        [RequiresApi(Api = 21)]
        public bool AddTransitionListener()
        {
            var transition = Window.SharedElementEnterTransition;
            if (transition != null)
            {
                transition.AddListener(GetTransitionListener());
                return true;
            }
            return false;
        }

        private TransitionListener GetTransitionListener()
        {
            var l = new TransitionListener();
            l.TransitionCancel = (t) =>
            {
                t.RemoveListener(l);
            };

            l.TransitionEnd = (t) =>
            {
                LoadImage();
                t.RemoveListener(l);
            };

            return l;
        }

        internal sealed class CustomTargetView : CustomTarget
        {
            public Action<Drawable> LoadCleared;
            public Action<Bitmap, ITransition> ResourceReady;
            public override void OnLoadCleared(Drawable p0)
            {
                LoadCleared(p0);
            }

            public override void OnResourceReady(Java.Lang.Object resource, ITransition transition)
            {
                ResourceReady(resource.JavaCast<Bitmap>(), transition);
            }
        }

        internal sealed class PaletteAsyncListener : Java.Lang.Object, IPaletteAsyncListener
        {
            public Action<Palette> Generated;
            public void OnGenerated(Palette palette)
            {
                Generated(palette);
            }
        }

        internal sealed class TransitionListener : Java.Lang.Object, ITransitionListener
        {
            public Action<Transition> TransitionCancel;
            public Action<Transition> TransitionEnd;
            public Action<Transition> TransitionPause;
            public Action<Transition> TransitionResume;
            public Action<Transition> TransitionStart;
            public void OnTransitionCancel(Transition transition)
            {
                TransitionCancel(transition);
            }

            public void OnTransitionEnd(Transition transition)
            {
                TransitionEnd(transition);
            }

            public void OnTransitionPause(Transition transition)
            {
                TransitionPause(transition);
            }

            public void OnTransitionResume(Transition transition)
            {
                TransitionResume(transition);
            }

            public void OnTransitionStart(Transition transition)
            {
                TransitionStart(transition);
            }
        }

        private class TabConfigStrategy : Java.Lang.Object, ITabConfigurationStrategy
        {
            public Action<TabLayout.Tab, int> ConfigureTab;
            public void OnConfigureTab(TabLayout.Tab p0, int p1)
            {
                ConfigureTab(p0, p1);
            }
        }

        private class DetailsFragment : AndroidX.Fragment.App.Fragment
        {
            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
            }

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                return inflater.Inflate(R.Layout.details_frag, container, false);
            }

            public override void OnViewCreated(View view, Bundle savedInstanceState)
            {
                base.OnViewCreated(view, savedInstanceState);
            }
        }

        private class GalleryFragment : AndroidX.Fragment.App.Fragment
        {
            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
            }

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                return inflater.Inflate(R.Layout.gallery_frag, container, false);
            }

            public override void OnViewCreated(View view, Bundle savedInstanceState)
            {
                base.OnViewCreated(view, savedInstanceState);
            }
        }

        private class BookingFragment : AndroidX.Fragment.App.Fragment
        {
            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
            }

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                return inflater.Inflate(R.Layout.booking_frag, container, false);
            }

            public override void OnViewCreated(View view, Bundle savedInstanceState)
            {
                base.OnViewCreated(view, savedInstanceState);
            }
        }
    }
}
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using AndroidX.Palette.Graphics;
using AndroidX.ViewPager2.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request.Target;
using Bumptech.Glide.Request.Transition;
using FlavioFaria.KenBurnsViewLib;
using Google.Android.Material.AppBar;
using Google.Android.Material.Tabs;
using Oyadieyie3D.Adapters;
using Oyadieyie3D.Fragments;
using Oyadieyie3D.HelperClasses;
using System;
using static AndroidX.Palette.Graphics.Palette;
using static Google.Android.Material.Tabs.TabLayoutMediator;
using R = Oyadieyie3D.Resource;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Oyadieyie3D.Activities
{
    [Activity(Theme = "@style/AppTheme.CustomFeaturedTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | 
        Android.Content.PM.ConfigChanges.SmallestScreenSize |
        Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class FeaturedDetailsActivity : AppCompatActivity, IOnApplyWindowInsetsListener
    {
        private string url;
        private DetailsFragment details;
        private GalleryFragment gallery;
        private BookingFragment booking;
        private CollapsingToolbarLayout collapsingToolbar;
        private AppBarLayout appBarLayout;
        private Toolbar toolbar;
        private ViewPager2 viewPager;

        public FrameLayout containerView;

        private KenBurnsView imageView;
        internal static string BANNER_URL = "bannerUrl";
        internal static string SHARED_TRANS_NAME = "shared";
        private TabLayout tabLayout;
        private string[] tabTitle = { "Details", "Gallery", "Book", "Reviews" };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.featured_details);
            url = Intent.GetStringExtra(BANNER_URL);
            details = new DetailsFragment();
            gallery = new GalleryFragment();
            booking = new BookingFragment();
            FindViews();
            SetUpViews();
        }

        private void SetUpViews()
        {
            SetSupportActionBar(toolbar);
            if (SupportActionBar != null)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);
                SupportActionBar.SetDisplayShowTitleEnabled(false);
            }

            collapsingToolbar.TitleEnabled = false;
            collapsingToolbar.LayoutParameters.Height = (Utils.CommonUtils.IsLand(this) ?
                Utils.CommonUtils.GetDisplayDimen(this).Y :
                Utils.CommonUtils.GetDisplayDimen(this).X) * 9 / 16 +
                Utils.CommonUtils.GetStatusBarHeightPixel(this);
            collapsingToolbar.RequestLayout();

            ActionBarResponsive();

            appBarLayout.AddOnOffsetChangedListener(new OffsetChangedListener
            {
                OffsetChanged = (s, o) =>
                {

                },
                StateChanged = (a, s) =>
                {

                }
            });

            var pagerAdapter = new PagerAdapter(this);
            pagerAdapter.AddFragment(details);
            pagerAdapter.AddFragment(gallery);
            pagerAdapter.AddFragment(booking);
            pagerAdapter.AddFragment(new AndroidX.Fragment.App.Fragment());

            viewPager.Adapter = pagerAdapter;

            new TabLayoutMediator(tabLayout, viewPager, new TabConfigStrategy
            {
                ConfigureTab = (tab, position) =>
                {
                    tab.SetText(tabTitle[position]);
                    viewPager.SetCurrentItem(tab.Position, true);
                }
            }).Attach();

            _ = Glide.With(this)
               .AsBitmap()
               .Load(url)
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
                                   collapsingToolbar.SetStatusBarScrimColor(vibrant.Rgb);
                               }
                           }
                       });
                   }
               });

            ViewCompat.SetOnApplyWindowInsetsListener(containerView, this);
        }

        private void FindViews()
        {
            imageView = FindViewById<KenBurnsView>(R.Id.collimg);
            toolbar = FindViewById<Toolbar>(R.Id.featured_toolbar);
            collapsingToolbar = FindViewById<CollapsingToolbarLayout>(R.Id.mycoll);
            appBarLayout = FindViewById<AppBarLayout>(R.Id.featured_appbar);
            tabLayout = FindViewById<TabLayout>(R.Id.featured_tablayout);
            viewPager = FindViewById<ViewPager2>(R.Id.featured_viewpager);
            containerView = FindViewById<FrameLayout>(R.Id.iv_container);
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (imageView != null)
            {
                imageView.Pause();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (imageView != null)
            {
                imageView.Resume();
            }
        }

        private void ActionBarResponsive()
        {
            int actionBarHeight = Utils.CommonUtils.GetActionBarHeightPixel(this);
            int tabHeight = Utils.CommonUtils.GetTabHeight(this);
            if (actionBarHeight > 0)
            {
                toolbar.LayoutParameters.Height = actionBarHeight + tabHeight;
                toolbar.RequestLayout();
            }
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }

        public WindowInsetsCompat OnApplyWindowInsets(View v, WindowInsetsCompat insets)
        {
            return insets.ConsumeSystemWindowInsets();
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

        internal sealed class OffsetChangedListener : Java.Lang.Object, AppBarLayout.IOnOffsetChangedListener
        {
            public enum State
            {
                Expanded,
                Collapsed,
                Idle
            }

            private State currentState = State.Idle;
            public Action<AppBarLayout, State> StateChanged;
            public Action<State, float> OffsetChanged;

            public void OnOffsetChanged(AppBarLayout appBarLayout, int verticalOffset)
            {

                if (verticalOffset == 0)
                {
                    if (currentState != State.Expanded)
                    {
                        StateChanged(appBarLayout, State.Expanded);
                    }
                    currentState = State.Expanded;
                }
                else if (Math.Abs(verticalOffset) >= appBarLayout.TotalScrollRange)
                {
                    if (currentState != State.Idle)
                    {
                        StateChanged(appBarLayout, State.Idle);
                    }
                    currentState = State.Collapsed;
                }
                OffsetChanged(currentState, Math.Abs(verticalOffset / (float)appBarLayout.TotalScrollRange));
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
        
    }
}
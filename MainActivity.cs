using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager2.Widget;
using CN.Pedant.SweetAlert;
using DE.Hdodenhof.CircleImageViewLib;
using Google.Android.Material.AppBar;
using Google.Android.Material.BottomAppBar;
using Google.Android.Material.Button;
using Google.Android.Material.Tabs;
using Google.Android.Material.TextField;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Adapters;
using Oyadieyie3D.Events;
using Oyadieyie3D.Fragments;
using Oyadieyie3D.HelperClasses;
using Oyadieyie3D.Models;
using System.Collections.Generic;
using static CN.Pedant.SweetAlert.SweetAlertDialog;
using R = Oyadieyie3D.Resource;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Oyadieyie3D
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", 
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | 
        Android.Content.PM.ConfigChanges.SmallestScreenSize |
        Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class MainActivity : AppCompatActivity
    {
        private AppBarLayout appBarLayout;
        private Toolbar toolbar;
        private TabLayout tabLayout;
        private ViewPager2 viewPager;
        private TextInputEditText searchBox;
        private ImageView filterBtn;
        private PagerAdapter pagerAdapter;

        private string[] tabTitle = { "All", "Popular", "Nearby", "Recent" };
        private ActionAllFragment all;
        private ActionPopularFragment popular;
        private ActionNearbyFragment nearby;
        private ActionRecentFragment recent;
        private BottomAppBar appbar;
        private CircleImageView profileImageView;
        private string profileImgUrl;
        private string phone;
        private string username;
        private PreferenceHelper prefInstance = PreferenceHelper.Instance;
        public static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            PreferenceHelper.Init(this);
            AppCompatDelegate.DefaultNightMode = prefInstance.GetBoolean("theme") ?
                AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo;

            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.activity_main);
            Instance = this;

            all = new ActionAllFragment();
            popular = new ActionPopularFragment();
            nearby = new ActionNearbyFragment();
            recent = new ActionRecentFragment();

            FindViews();
            SetUpViews();

            profileImgUrl = prefInstance.GetString(Constants.Profile_Url_Key, "");
            username = prefInstance.GetString(Constants.Username_Key, "");
            phone = prefInstance.GetString(Constants.Phone_Key, "");
            prefInstance.SetImageResource(profileImageView, profileImgUrl, PlaceholderType.Profile);
        }

        private void FindViews()
        {
            appbar = FindViewById<BottomAppBar>(R.Id.bottomAppBar);
            profileImageView = appbar.FindViewById<CircleImageView>(R.Id.profile_iv);
            appBarLayout = FindViewById<AppBarLayout>(R.Id.action_appbar);
            toolbar = FindViewById<Toolbar>(R.Id.action_toolbar);
            tabLayout = FindViewById<TabLayout>(R.Id.action_tablayout);
            viewPager = FindViewById<ViewPager2>(R.Id.action_viewpager);
            searchBox = FindViewById<TextInputEditText>(R.Id.actionSearchField);
            filterBtn = FindViewById<ImageView>(R.Id.filterBtn);
        }

        private void SetUpViews()
        {
            //appbar
            appbar.MenuItemClick += (s2, e2) =>
            {
                switch (e2.Item.ItemId)
                {
                    case R.Id.action_market:
                        var marketIntent = new Intent(this, typeof(StoreActivity));
                        StartActivity(marketIntent);
                        break;

                    case R.Id.action_dark_theme:
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                        PreferenceHelper.Instance.SetBoolean("theme", true);
                        break;

                    case R.Id.action_light_theme:
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                        PreferenceHelper.Instance.SetBoolean("theme", false);
                        break;

                    default:
                        var intent = new Intent(this, typeof(SettingsActivity));
                        StartActivity(intent);
                        break;
                }
            };
            profileImageView.Click += (s4, e4)
                => ShowProfileDialog();

            //pager
            pagerAdapter = new PagerAdapter(this);
            pagerAdapter.AddFragment(all);
            pagerAdapter.AddFragment(popular);
            pagerAdapter.AddFragment(nearby);
            pagerAdapter.AddFragment(recent);
            viewPager.Adapter = pagerAdapter;
            new TabLayoutMediator(tabLayout, viewPager, new TabConfigStrategy
            {
                ConfigureTab = (tab, position) =>
                {
                    tab.SetText(tabTitle[position]);
                    viewPager.SetCurrentItem(tab.Position, true);
                }
            }).Attach();

            filterBtn.Click += (s1, e1) =>
            {
                ShowFilterSheet();
            };
        }

        private void ShowProfileDialog()
        {
            var view = LayoutInflater.From(this).Inflate(R.Layout.profile_popup_window, null);
            var dialog = new AndroidX.AppCompat.App.AlertDialog.Builder(this)
                .SetView(view)
                .Create();

            dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
            dialog.Show();

            var image_view = view.FindViewById<CircleImageView>(R.Id.popup_prof_iv);
            var username = view.FindViewById<TextView>(R.Id.popup_user_tv);
            var phone = view.FindViewById<TextView>(R.Id.popup_phone_tv);
            var manage = view.FindViewById<MaterialButton>(R.Id.popup_manage_btn);
            var logout = view.FindViewById<ImageButton>(R.Id.dialog_logout_btn);

            prefInstance.SetImageResource(image_view, profileImgUrl, PlaceholderType.Profile);

            username.Text = this.username;
            phone.Text = this.phone;
            manage.Click += (s, e) =>
            {
                manage.Post(() =>
                {
                    var intent = new Intent(this, typeof(ProfileActivity));
                    StartActivity(intent);
                    dialog.Dismiss();
                });
            };
            logout.Click += (s1, e1) =>
            {
                var confirmClick = new SweetConfirmClick(
                    (s) =>
                    {
                        s.DismissWithAnimation();
                        SessionManager.GetFirebaseAuth().SignOut();
                        PreferenceHelper.Instance.ClearPrefs();

                        var i = new Intent(this, typeof(OnboardingActivity));
                        i.AddFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask);
                        StartActivity(i);
                    });

                ShowWarning(this, "Sign out", "Are you sure you want to sign-out?", confirmClick);
            };
        }

        public void ShowWarning(Context context, string title, string warn, IOnSweetClickListener listener)
        {
            var warnDialog = new SweetAlertDialog(context, WarningType);
            warnDialog.SetTitleText(title);
            warnDialog.SetContentText(warn);
            warnDialog.SetConfirmText("Yes");
            warnDialog.SetCancelText("No");
            warnDialog.SetConfirmClickListener(listener);
            warnDialog.Show();
        }


        private void ShowFilterSheet()
        {
            var view = LayoutInflater.From(this).Inflate(R.Layout.filter_sheet, null);
            var dialog = new AndroidX.AppCompat.App.AlertDialog.Builder(this)
                .SetView(view)
                .Create();

            dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
            dialog.Show();     

            var expandableListView = FindViewById<ExpandableListView>(R.Id.filter_listExpander);
            var listDetail = ExpandableListDataPump.GetData();
            List<string> expandableListTitle = new List<string>(listDetail.Keys);
            var listAdapter = new CustomExpandableListAdapter(this, expandableListTitle, listDetail);

            expandableListView.SetAdapter(listAdapter);
        }
    }
}

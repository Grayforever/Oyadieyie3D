using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Google.Android.Material.AppBar;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Oyadieyie3D.Adapters;
using Oyadieyie3D.Events;
using Oyadieyie3D.Models;
using Oyadieyie3D.Utils;
using System.Collections.Generic;
using R = Oyadieyie3D.Resource;
using Uri = Android.Net.Uri;

namespace Oyadieyie3D.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme",
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout |
        Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation,
        WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class StoreActivity : AppCompatActivity
    {
        private RecyclerView mainRecycler;
        private SwipeRefreshLayout swipe_container;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.store_layout);

            mainRecycler = FindViewById<RecyclerView>(R.Id.main_recycler);
            swipe_container = FindViewById<SwipeRefreshLayout>(R.Id.main_refresher);
            var appbar = FindViewById<AppBarLayout>(R.Id.store_appbar);
            var toolbar = appbar.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(R.Id.store_toolbar);

            var searchView = toolbar.Menu.FindItem(R.Id.action_search).ActionView as AndroidX.AppCompat.Widget.SearchView;
            searchView.QueryTextChange += (s, e) =>
            {

            };

            toolbar.MenuItemClick += (s1, e1) =>
            {
                switch (e1.Item.ItemId)
                {
                    case R.Id.action_help:
                        //postBottomsheetBehavior.State = isOpen ? BottomSheetBehavior.StateHidden : BottomSheetBehavior.StateExpanded;
                        break;

                    default:
                        break;
                }
            };
        }


    }
}
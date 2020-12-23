using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Annotations;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content.Resources;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;
using Oyadieyie3D.Cards;
using Oyadieyie3D.Events;
using Oyadieyie3D.Models;
using Oyadieyie3D.Utils;
using Ramotion.CardSliderLib;
using System;
using System.Collections.Generic;
using System.Linq;
using R = Oyadieyie3D.Resource;

namespace Oyadieyie3D.Activities
{
    [Register("id.Oyadieyie3D.Activities.FinderActivity")]
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public sealed class FinderActivity : AppCompatActivity
    {
        private int[] pics;
        private string[] names;
        private string[] description;
        private int[] maps;
        private string[] title;
        private double[] rating;
        private string[] hours;

        private const string MyTag = "RamotionCardSlider";

        private readonly int[,] _dotCoords = new int[5, 2];

        private CardSliderLayoutManager _layoutManger;
        private RecyclerView _recyclerView;
        private ImageSwitcher _mapSwitcher;
        private TextSwitcher _ratingSwitcher, _featuredSwitcher, _clockSwitcher, _descriptionsSwitcher;
        private View _greenDot;

        private TextView _name1TextView, _name2TextView;
        private int _nameOffset1, _nameOffset2;
        private long _nameAnimDuration;
        private int _currentPosition;

        private DecodeBitmapTask _decodeMapBitmapTask;
        private DecodeBitmapTask.IListener _mapLoadListener;

        private SliderAdapter MySliderAdapter;
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           
        }
    }
}
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
            SetContentView(R.Layout.finder_layout);
            SetUpFeatured();

            InitRecyclerView();
            InitCountryText();
            InitSwitchers();
            InitGreenDot();
        }

        private void SetUpFeatured()
        {
            var client1 = new Client.Builder()
                .SetClientName("Gray Labs")
                .SetRating(4.4)
                .SetOpeningHours("Mon-Thu 7:00am-8:00pm")
                .SetMapUrl(R.Drawable.map_beijing)
                .SetImageUrl(R.Drawable.p1)
                .SetItemTitle("Sit duis aliquyam esse dolores")
                .SetItemDescription("Sed amet ut dolor stet ut dolore nonumy invidunt consequat")
                .Build();

            var client2 = new Client.Builder()
                .SetClientName("Laddy")
                .SetRating(4.4)
                .SetOpeningHours("Mon-Thu 7:00am-8:00pm")
                .SetMapUrl(R.Drawable.map_paris)
                .SetImageUrl(R.Drawable.p2)
                .SetItemTitle("Sit duis aliquyam esse dolores")
                .SetItemDescription("Sed amet ut dolor stet ut dolore nonumy invidunt consequat")
                .Build();

            var client3 = new Client.Builder()
                .SetClientName("Jeffery")
                .SetRating(4.4)
                .SetOpeningHours("Mon-Thu 7:00am-8:00pm")
                .SetMapUrl(R.Drawable.map_seoul)
                .SetImageUrl(R.Drawable.p3)
                .SetItemTitle("Sit duis aliquyam esse dolores")
                .SetItemDescription("Sed amet ut dolor stet ut dolore nonumy invidunt consequat")
                .Build();

            var client4 = new Client.Builder()
                .SetClientName("Jenny")
                .SetRating(4.4)
                .SetOpeningHours("Mon-Thu 7:00am-8:00pm")
                .SetMapUrl(R.Drawable.map_london)
                .SetImageUrl(R.Drawable.p4)
                .SetItemTitle("Sit duis aliquyam esse dolores")
                .SetItemDescription("Sed amet ut dolor stet ut dolore nonumy invidunt consequat")
                .Build();

            var client5 = new Client.Builder()
                .SetClientName("Ea nonumy eos et dolor diam et et consequat ipsum")
                .SetRating(4.4)
                .SetOpeningHours("Mon-Thu 7:00am-8:00pm")
                .SetMapUrl(R.Drawable.map_greece)
                .SetImageUrl(R.Drawable.p5)
                .SetItemTitle("Sit duis aliquyam esse dolores")
                .SetItemDescription("Sed amet ut dolor stet ut dolore nonumy invidunt consequat")
                .Build();

            var premiumClients = new List<Client>();
            premiumClients.Add(client1);
            premiumClients.Add(client2);
            premiumClients.Add(client3);
            premiumClients.Add(client4);
            premiumClients.Add(client5);

            pics = premiumClients.Select(client => client.ItemImgUrl).ToArray();
            names = premiumClients.Select(client => client.ClientName.ToUpper()).ToArray();
            description = premiumClients.Select(client => client.ItemDescription).ToArray();
            maps = premiumClients.Select(client => client.MapImgUrl).ToArray();
            title = premiumClients.Select(client => client.ItemTitle).ToArray();
            rating = premiumClients.Select(client => client.Rating).ToArray();
            hours = premiumClients.Select(client => client.OpeningHours).ToArray();
            MySliderAdapter = new SliderAdapter(pics, premiumClients.Count, OnCardClickListener);
        }

        private void InitRecyclerView()
        {
            _recyclerView = FindViewById<RecyclerView>(R.Id.finder_recycler);
            _recyclerView.SetAdapter(MySliderAdapter);
            _recyclerView.HasFixedSize = true;
            _recyclerView.AddOnScrollListener(
                new MyRvOnScrollListener(
                    null,
                    (rv, newState) =>
                    {
                        if (newState == RecyclerView.ScrollStateIdle)
                            OnActiveCardChange();
                    }
                )
            );

            _layoutManger = (CardSliderLayoutManager)_recyclerView.GetLayoutManager();

            new CardSnapHelper().AttachToRecyclerView(_recyclerView);
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (IsFinishing)
                _decodeMapBitmapTask?.Cancel(true);
        }

        private void InitSwitchers()
        {
            _ratingSwitcher = FindViewById<TextSwitcher>(R.Id.ts_temperature);
            _ratingSwitcher.SetFactory(new TextViewFactory(this, R.Style.AppTheme_RatingTextView, true));
            _ratingSwitcher.SetCurrentText(rating[0].ToString());

            _featuredSwitcher = FindViewById<TextSwitcher>(R.Id.ts_place);
            _featuredSwitcher.SetFactory(new TextViewFactory(this, R.Style.AppTheme_NameTextView, false));
            _featuredSwitcher.SetCurrentText(title[0]);

            _clockSwitcher = FindViewById<TextSwitcher>(R.Id.ts_clock);
            _clockSwitcher.SetFactory(new TextViewFactory(this, R.Style.AppTheme_ClockTextView, false));
            _clockSwitcher.SetCurrentText(hours[0]);

            _descriptionsSwitcher = FindViewById<TextSwitcher>(R.Id.ts_description);
            _descriptionsSwitcher.SetInAnimation(this, Android.Resource.Animation.FadeIn);
            _descriptionsSwitcher.SetOutAnimation(this, Android.Resource.Animation.FadeOut);
            _descriptionsSwitcher.SetFactory(new TextViewFactory(this, R.Style.AppTheme_DescriptionTextView, false));
            _descriptionsSwitcher.SetCurrentText(description[0]);

            _mapSwitcher = FindViewById<ImageSwitcher>(R.Id.ts_map);
            _mapSwitcher.SetInAnimation(this, R.Animation.fade_in);
            _mapSwitcher.SetOutAnimation(this, R.Animation.fade_out);
            _mapSwitcher.SetFactory(new ImageViewFactory(this));
            _mapSwitcher.SetImageResource(maps[0]);

            _mapLoadListener = new DecodeBitmapTask.Listener(
                bmp =>
                {
                    (_mapSwitcher.NextView as ImageView)?.SetImageBitmap(bmp);
                    _mapSwitcher.ShowNext();
                }
            );
        }

        private void InitCountryText()
        {
            _nameAnimDuration = Resources.GetInteger(R.Integer.labels_animation_duration);
            _nameOffset1 = Resources.GetDimensionPixelSize(R.Dimension.left_offset);
            _nameOffset2 = Resources.GetDimensionPixelSize(R.Dimension.card_width);
            _name1TextView = FindViewById<TextView>(R.Id.tv_country_1);
            _name2TextView = FindViewById<TextView>(R.Id.tv_country_2);

            _name1TextView.SetX(_nameOffset1);
            _name2TextView.SetX(_nameOffset2);
            _name1TextView.Text = names[0];
            _name2TextView.Alpha = 0f;

            var typeface = ResourcesCompat.GetFont(this, R.Font.raleway_bold);
            _name1TextView.Typeface = typeface;
            _name2TextView.Typeface = typeface;
        }

        private void InitGreenDot()
        {
            try
            {
                var l = new MyVtoOnGlobalLayoutListener();
                l.GlobalLayoutEvent += (s, e) =>
                {
                    _mapSwitcher.ViewTreeObserver.RemoveOnGlobalLayoutListener(l);

                    var viewLeft = _mapSwitcher.Left;
                    var viewTop = _mapSwitcher.Top + _mapSwitcher.Height / 3;

                    const int border = 100;
                    var xRange = Math.Max(1, _mapSwitcher.Width - border * 2);
                    var yRange = Math.Max(1, _mapSwitcher.Height / 3 * 2 - border * 2);

                    var rnd = new System.Random();

                    for (int i = 0, cnt = _dotCoords.GetLength(0); i < cnt; i++)
                    {
                        _dotCoords[i, 0] = viewLeft + border + rnd.Next(xRange);
                        _dotCoords[i, 1] = viewTop + border + rnd.Next(yRange);
                    }

                    _greenDot = FindViewById<View>(R.Id.green_dot);
                    _greenDot.SetX(_dotCoords[0, 0]);
                    _greenDot.SetY(_dotCoords[0, 1]);
                };
                _mapSwitcher.ViewTreeObserver.AddOnGlobalLayoutListener(l);
            }
            catch (Exception exc) when (exc is IndexOutOfRangeException)
            {
                LogW(exc.Message);
            }
        }
        private void SetCountryText(string text, bool left2Right)
        {
            TextView invisibleText;
            TextView visibleText;

            if (_name1TextView.Alpha > _name2TextView.Alpha)
            {
                visibleText = _name1TextView;
                invisibleText = _name2TextView;
            }
            else
            {
                visibleText = _name2TextView;
                invisibleText = _name1TextView;
            }

            int vOffset;
            if (left2Right)
            {
                invisibleText.SetX(0);
                vOffset = _nameOffset2;
            }
            else
            {
                invisibleText.SetX(_nameOffset2);
                vOffset = 0;
            }

            invisibleText.Text = text;

            var iAlpha = ObjectAnimator.OfFloat(invisibleText, "alpha", 1f);
            var vAlpha = ObjectAnimator.OfFloat(visibleText, "alpha", 0f);
            var iX = ObjectAnimator.OfFloat(invisibleText, "x", _nameOffset1);
            var vX = ObjectAnimator.OfFloat(visibleText, "x", vOffset);

            var animSet = new AnimatorSet();
            animSet.PlayTogether(iAlpha, vAlpha, iX, vX);
            animSet.SetDuration(_nameAnimDuration);
            animSet.Start();
        }

        private void OnActiveCardChange()
        {
            var pos = _layoutManger.ActiveCardPosition;
            if (pos == RecyclerView.NoPosition || pos == _currentPosition)
                return;

            OnActiveCardChange(pos);
        }

        private void OnActiveCardChange(int pos)
        {
            var animH = new[] { R.Animation.slide_in_right, R.Animation.slide_out_left };
            var animV = new[] { R.Animation.slide_in_top, R.Animation.slide_out_bottom };

            var left2Right = pos < _currentPosition;
            if (left2Right)
            {
                animH[0] = R.Animation.slide_in_left;
                animH[1] = R.Animation.slide_out_right;

                animV[0] = R.Animation.slide_in_bottom;
                animV[1] = R.Animation.slide_out_top;
            }

            SetCountryText(names[pos % names.Length], left2Right);

            _ratingSwitcher.SetInAnimation(this, animH[0]);
            _ratingSwitcher.SetOutAnimation(this, animH[1]);
            _ratingSwitcher.SetText(rating[pos % rating.ToString().Length].ToString());

            _featuredSwitcher.SetInAnimation(this, animV[0]);
            _featuredSwitcher.SetOutAnimation(this, animV[1]);
            _featuredSwitcher.SetText(title[pos % title.Length]);

            _clockSwitcher.SetInAnimation(this, animV[0]);
            _clockSwitcher.SetOutAnimation(this, animV[1]);
            _clockSwitcher.SetText(hours[pos % hours.Length]);

            _descriptionsSwitcher.SetText(description[pos % description.Length]);

            ShowMap(maps[pos % maps.Length]);

            ViewCompat.Animate(_greenDot)
                .TranslationX(_dotCoords[pos % _dotCoords.GetLength(0), 0])
                .TranslationY(_dotCoords[pos % _dotCoords.GetLength(0), 1])
                .Start();

            _currentPosition = pos;
        }

        private void ShowMap([DrawableRes] int resId)
        {
            _decodeMapBitmapTask?.Cancel(true);

            var w = _mapSwitcher.Width;
            var h = _mapSwitcher.Height;

            _decodeMapBitmapTask = new DecodeBitmapTask(Resources, resId, w, h, _mapLoadListener);
            _decodeMapBitmapTask.Execute();
        }

        private sealed class TextViewFactory : Java.Lang.Object, ViewSwitcher.IViewFactory
        {
            private readonly Context _ctx;
            [StyleRes] private readonly int _styleId;
            private readonly bool _center;

            public TextViewFactory(Context ctx, [StyleRes] int styleId, bool center)
            {
                _ctx = ctx;
                _styleId = styleId;
                _center = center;
            }

            public View MakeView()
            {
                var textView = new TextView(_ctx);

                if (_center)
                    textView.Gravity = GravityFlags.Center;

                if (Build.VERSION.SdkInt < BuildVersionCodes.M)
#pragma warning disable 618
                    textView.SetTextAppearance(_ctx, _styleId);
#pragma warning restore 618
                else
                    textView.SetTextAppearance(_styleId);

                return textView;
            }
        }

        private sealed class ImageViewFactory : Java.Lang.Object, ViewSwitcher.IViewFactory
        {
            private readonly Context _ctx;

            public ImageViewFactory(Context ctx)
            {
                _ctx = ctx;
            }

            public View MakeView()
            {
                var imageView = new ImageView(_ctx);
                imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
                imageView.LayoutParameters = new FrameLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent
                );
                return imageView;
            }
        }

        private View.IOnClickListener OnCardClickListener => new MyViewOnClickListener(
            v =>
            {
                var lm = (CardSliderLayoutManager)_recyclerView.GetLayoutManager();

                if (lm.IsSmoothScrolling)
                    return;

                var activeCardPosition = lm.ActiveCardPosition;
                if (activeCardPosition == RecyclerView.NoPosition)
                    return;

                var clickedPosition = _recyclerView.GetChildAdapterPosition(v);
                if (clickedPosition == activeCardPosition)
                {
                    //var intent = new Intent(this, typeof(DetailsActivity));
                    //intent.PutExtra(DetailsActivity.BundleImageId, _pics[activeCardPosition % _pics.Length]);

                    //if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                    //{
                    //    StartActivity(intent);
                    //}
                    //else
                    //{
                    //    var cardView = (CardView)v;
                    //    var sharedView = cardView.GetChildAt(cardView.ChildCount - 1);
                    //    var options = ActivityOptions.MakeSceneTransitionAnimation(this, sharedView, "shared");
                    //    StartActivity(intent, options.ToBundle());
                    //}
                }
                else if (clickedPosition > activeCardPosition)
                {
                    _recyclerView.SmoothScrollToPosition(clickedPosition);
                    OnActiveCardChange(clickedPosition);
                }
            }
        );

        private static void LogD(string msg) => Log.Debug(MyTag, msg);

        private static void LogW(string msg) => Log.Warn(MyTag, msg);
    }
}
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Annotations;
using AndroidX.AppCompat.App;
using AndroidX.CardView.Widget;
using AndroidX.Core.Content.Resources;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using BumpTech.GlideLib;
using BumpTech.GlideLib.Requests;
using CN.Pedant.SweetAlert;
using DE.Hdodenhof.CircleImageViewLib;
using Firebase.Database;
using Google.Android.Material.BottomAppBar;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.Chip;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Cards;
using Oyadieyie3D.Events;
using Oyadieyie3D.HelperClasses;
using Oyadieyie3D.Models;
using Oyadieyie3D.Utils;
using Ramotion.CardSliderLib;
using System.Collections.Generic;
using System.Linq;
using static CN.Pedant.SweetAlert.SweetAlertDialog;
using Exception = System.Exception;
using R = Oyadieyie3D.Resource;

namespace Oyadieyie3D
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | 
        Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class MainActivity : AppCompatActivity
    {
        private int[] pics;
        private string[] names;
        private string[] description;
        private int[] maps;
        private string[] title;
        private double[] rating;
        private string[] hours;

        private CardSliderLayoutManager _layoutManger;
        private RecyclerView _recyclerView;
        private ImageSwitcher _mapSwitcher;
        private TextSwitcher _ratingSwitcher, _featuredSwitcher, _clockSwitcher, _descriptionsSwitcher;

        private TextView _name1TextView, _name2TextView;
        private int _nameOffset1, _nameOffset2;
        private long _nameAnimDuration;
        private int _currentPosition;

        private DecodeBitmapTask _decodeMapBitmapTask;
        private DecodeBitmapTask.IListener _mapLoadListener;

        private SliderAdapter MySliderAdapter;
        private CircleImageView profileImageView;
        private string profileImgUrl;
        private string phone;
        private RequestOptions op;
        private string username;
        private string status;
        private string email;
        private List<Client> premiumClients;
        private BottomSheetBehavior searchSheet;
        private AnimatedVectorDrawable animatable;

        public static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            PreferenceHelper.Init(this);
            AppCompatDelegate.DefaultNightMode = PreferenceHelper.Instance.GetBoolean("theme") ?
                AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo;

            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.activity_main);
            Instance = this;

            var searchFab = FindViewById<FloatingActionButton>(R.Id.post_fab);
            var appbar = FindViewById<BottomAppBar>(R.Id.bottomAppBar);
            var searchRoot = FindViewById<LinearLayout>(R.Id.search_root);
            var swipeRoot = FindViewById<SwipeRefreshLayout>(R.Id.finder_swipe_root);
            profileImageView = appbar.FindViewById<CircleImageView>(R.Id.profile_iv);

            swipeRoot.Refreshing = true;
            searchSheet = SetSearchSheet(searchRoot);

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
            searchFab.Click += (s3, e3) =>
            {
                try
                {
                    animatable = searchFab.Drawable as AnimatedVectorDrawable;
                    switch (searchSheet.State)
                    {
                        case BottomSheetBehavior.StateHidden:
                            searchSheet.State = BottomSheetBehavior.StateExpanded;
                            animatable.Start();
                            break;

                        default:
                            searchSheet.State = BottomSheetBehavior.StateHidden;
                            animatable.Reset();
                            break;
                    }



                }
                catch (Exception)
                {
                    //
                }
            };
            profileImageView.Click += (s4, e4) => ShowProfileDialog();

            profileImgUrl = PreferenceHelper.Instance.GetString(Constants.Profile_Url_Key, "");
            username = PreferenceHelper.Instance.GetString(Constants.Username_Key, "");
            status = PreferenceHelper.Instance.GetString(Constants.Status_Key, "");
            email = PreferenceHelper.Instance.GetString(Constants.Email_Key, "");
            phone = PreferenceHelper.Instance.GetString(Constants.Phone_Key, "");

            op = new RequestOptions();
            op.Placeholder(R.Drawable.ic_account_circle);
            Glide.With(this).SetDefaultRequestOptions(op).Load(profileImgUrl).Into(profileImageView);

            FetchClients();
        }

        public override void OnBackPressed()
        {
            var currentState = searchSheet.State;
            if (currentState == BottomSheetBehavior.StateExpanded)
            {
                currentState = BottomSheetBehavior.StateHidden;
                searchSheet.State = currentState;
                animatable.Reset();
            }
            else
            {
                base.OnBackPressed();
            }
            
        }

        private BottomSheetBehavior SetSearchSheet(LinearLayout searchRoot)
        {
            var searchSheet = BottomSheetBehavior.From(searchRoot);
            searchSheet.State = BottomSheetBehavior.StateHidden;
            var searchbox = FindViewById<TextInputLayout>(R.Id.search_box_tl);
            var filterchip = FindViewById<ChipGroup>(R.Id.cat_chip_group);
            var resultRecycler = FindViewById<RecyclerView>(R.Id.search_recyler);

            return searchSheet;
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (IsFinishing)
                _decodeMapBitmapTask?.Cancel(true);
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

            Glide.With(this).SetDefaultRequestOptions(op).Load(profileImgUrl).Into(image_view);
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
                    (s)=> 
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

        private void FetchClients()
        {
            var clientsDbSnapshot = SessionManager.GetFireDB().GetReference("");
            clientsDbSnapshot.AddListenerForSingleValueEvent(new SingleValueListener(
                (s) => 
                {
                    if (!s.Exists())
                        return;

                    var snapshot = s.Children.ToEnumerable<DataSnapshot>();

                    if (snapshot.Count() == 0)
                        return;

                    foreach(var child in snapshot)
                    {
                        var client = new Client.Builder()
                            .SetClientName(child.Child("client_name") != null ? child.Child("client_name").Value.ToString() : "")
                            .SetImageUrl(0)
                            .SetRating(4.3)
                            .SetOpeningHours(child.Child("client_name") != null ? child.Child("client_name").Value.ToString() : "")
                            .SetItemTitle(child.Child("client_name") != null ? child.Child("client_name").Value.ToString() : "")
                            .SetItemDescription(child.Child("client_name") != null ? child.Child("client_name").Value.ToString() : "")
                            .SetMapUrl(0)
                            .Build();

                        premiumClients.Add(client);

                        pics = premiumClients.Select(client => client.ItemImgUrl).ToArray();
                        names = premiumClients.Select(client => client.ClientName.ToUpper()).ToArray();
                        description = premiumClients.Select(client => client.ItemDescription).ToArray();
                        maps = premiumClients.Select(client => client.MapImgUrl).ToArray();
                        title = premiumClients.Select(client => client.ItemTitle).ToArray();
                        rating = premiumClients.Select(client => client.Rating).ToArray();
                        hours = premiumClients.Select(client => client.OpeningHours).ToArray();
                        MySliderAdapter = new SliderAdapter(pics, premiumClients.Count, OnCardClickListener);
                    }

                    InitRecyclerView();
                    InitCountryText();
                    InitSwitchers();

                }, (e) => 
                {
                    Toast.MakeText(this, e.Message, ToastLength.Short).Show();
                }));
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

        private void SetClientName(string text, bool left2Right)
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

            SetClientName(names[pos % names.Length], left2Right);

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

            _currentPosition = pos;
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
                    var intent = new Intent(this, typeof(FeaturedDetailsActivity));
                    intent.PutExtra(FeaturedDetailsActivity.BundleImageId, pics[activeCardPosition % pics.Length]);

                    if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                    {
                        StartActivity(intent);
                    }
                    else
                    {
                        var cardView = (CardView)v;
                        var sharedView = cardView.GetChildAt(cardView.ChildCount - 1);
                        var options = ActivityOptions.MakeSceneTransitionAnimation(this, sharedView, "shared");
                        StartActivity(intent, options.ToBundle());
                    }
                }
                else if (clickedPosition > activeCardPosition)
                {
                    _recyclerView.SmoothScrollToPosition(clickedPosition);
                    OnActiveCardChange(clickedPosition);
                }
            }
        );

        private static void LogW(string msg) => Log.Warn(Constants.MyTag, msg);

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

    }
}

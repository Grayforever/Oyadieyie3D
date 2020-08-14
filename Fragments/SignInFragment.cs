using Android.Content.Res;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.AppBar;

namespace Oyadieyie3D.Fragments
{
    public class SignInFragment : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.sign_in_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var appbar = view.FindViewById<AppBarLayout>(Resource.Id.sign_in_appbar);
            var toolbar = appbar.FindViewById<Toolbar>(Resource.Id.main_toolbar);
            toolbar.Title = "Sign in";
            toolbar.SetTitleTextColor(ColorStateList.ValueOf(Android.Graphics.Color.White));
        }
    }
}
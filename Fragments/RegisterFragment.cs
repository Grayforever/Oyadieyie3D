using Android.Content.Res;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.Fragment.App;
using Google.Android.Material.AppBar;
using Google.Android.Material.Button;
using System;

namespace Oyadieyie3D.Fragments
{
    public class RegisterFragment : Fragment
    {
        public event EventHandler OnRegNextBtnClick;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.register_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var appbar = view.FindViewById<AppBarLayout>(Resource.Id.register_appbar);
            var toolbar = appbar.FindViewById<Toolbar>(Resource.Id.main_toolbar);
            toolbar.Title = "Register";

            var continueBtn = view.FindViewById<MaterialButton>(Resource.Id.reg_nxt_btn);
            continueBtn.Click += ContinueBtn_Click;
        }

        private void ContinueBtn_Click(object sender, System.EventArgs e)
        {
            OnRegNextBtnClick?.Invoke(this, new EventArgs());
        }
    }
}
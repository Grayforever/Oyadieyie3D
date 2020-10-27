using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Google.Android.Material.Button;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Events;
using Oyadieyie3D.HelperClasses;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;

namespace Oyadieyie3D.Fragments
{
    public class PartnerFragment : Fragment
    {
        ISharedPreferences preferences = Application.Context.GetSharedPreferences("userInfo", FileCreationMode.Private);
        ISharedPreferencesEditor editor;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.account_type_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var buttonToggleGrp = view.FindViewById<MaterialButtonToggleGroup>(Resource.Id.acc_type_opt_tglgrp);
            var nextBtn = view.FindViewById<MaterialButton>(Resource.Id.acc_type_nxt_btn);
            buttonToggleGrp.AddOnButtonCheckedListener(new ButtonCheckedListener((group, id, isChecked) => 
            {
                
            }));

            nextBtn.Click += (s1, e1) =>
            {
                editor = preferences.Edit();
                editor.PutString("firstRun", "regd");
                editor.Commit();
                
                OnboardingActivity.SetStatus(Constants.REG_STAGE_DONE);
                var i = new Intent(Context, typeof(MainActivity));
                i.SetFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask);
                StartActivity(i);
            };
        }
    }
}
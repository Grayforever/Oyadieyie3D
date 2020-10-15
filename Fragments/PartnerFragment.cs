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

        private bool isDesigner = false;
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
            buttonToggleGrp.AddOnButtonCheckedListener(new ButtonCheckedListener((group, id, isChecked) => 
            {
                switch (id)
                {
                    case Resource.Id.client_btn:
                        isDesigner = false;
                        break;

                    case Resource.Id.designer_btn:
                        isDesigner = true;
                        break;
                    default:
                        break;
                }
            }));

            var nextBtn = view.FindViewById<MaterialButton>(Resource.Id.acc_type_nxt_btn);
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
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using Google.Android.Material.Button;
using Oyadieyie3D.Events;

namespace Oyadieyie3D.Fragments
{
    public class PartnerFragment : Fragment
    {

        private bool isDesigner = false;

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
                //SetFragment();
                var i = new Intent(Context, typeof(MainActivity));
                StartActivity(i);
            };
        }

        private void SetFragment()
        {
            
            switch (isDesigner)
            {
                case true:
                    FragmentTransaction ft1 = ChildFragmentManager.BeginTransaction();
                    ft1.Add(new ClientFragment(), "clientFragment");
                    ft1.CommitAllowingStateLoss();
                    break;

                case false:
                    FragmentTransaction ft2 = ChildFragmentManager.BeginTransaction();
                    ft2.Add(new DesignerFragment(), "designerFragment");
                    ft2.CommitAllowingStateLoss();
                    break;
            }
        }
    }
}
﻿using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Java.IO;
using Java.Lang;
using System.Threading.Tasks;
using Process = Java.Lang.Process;
using R = Oyadieyie3D.Resource;

namespace Oyadieyie3D.Fragments
{
    public class NoNetBottomSheet : BottomSheetDialogFragment
    {
        private TextView noNetHeader;
        private TextView noNetSub;
        private ProgressBar progress;
        private MaterialButton btnRetry;
        private MaterialButton btnOpenSettings;
        private Runtime runtime;
        private Process mIpAddrProcess;

        private int retryText1 = R.String.no_net_retry1; 
        private int headerText1 = R.String.no_net_header1;
        private int subTitle1 = R.String.no_net_sub1;

        private int retryText2 = R.String.no_net_retry2;
        private int headerText2 = R.String.no_net_header2;
        private int subTitle2 = R.String.no_net_sub2;
        private FragmentActivity _context;
        private bool isPinging = false;

        public NoNetBottomSheet(FragmentActivity context)
        {
            _context = context;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle(StyleNormal, R.Style.Widget_MaterialComponents_BottomSheet_Modal);
            
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(R.Layout.no_net_btmsht, container, false);
            runtime = Runtime.GetRuntime();
            return view; 
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            //base.OnViewCreated(view, savedInstanceState);
            noNetHeader = view.FindViewById<TextView>(R.Id.no_net_hdr);
            noNetSub = view.FindViewById<TextView>(R.Id.no_net_sbttl);
            progress = view.FindViewById<ProgressBar>(R.Id.no_net_prgs);
            btnRetry = view.FindViewById<MaterialButton>(R.Id.no_net_btn);
            btnOpenSettings = view.FindViewById<MaterialButton>(R.Id.no_net_btn_opn_stn);
            btnRetry.Click += (s1, e1) => SetLoading(true);
            btnOpenSettings.Click += (s2, e2) => SetLoading(false);
        }

        private async void SetLoading(bool val)
        {
            if (val)
            {
                isPinging = true;
                UpdateUi();
                await PingGoogleServer();
            }
            else
            {
                isPinging = false;
                UpdateUi();
                Intent intentOpenSettings = new Intent(Settings.ActionDataRoamingSettings);
                _context.StartActivityForResult(intentOpenSettings, 0);
                
            }
        }

        private void UpdateUi()
        {
            if (!isPinging)
            {
                progress.Visibility = ViewStates.Invisible;
                btnRetry.Enabled = true;
                btnRetry.SetText(retryText1);
                noNetHeader.SetText(headerText1);
                noNetSub.SetText(subTitle1);
            }
            else
            {
                progress.Visibility = ViewStates.Visible;
                btnRetry.Enabled = false;
                btnRetry.SetText(retryText2);
                noNetHeader.SetText(headerText2);
                noNetSub.SetText(subTitle2);
            }
        }

        private async Task<bool> PingGoogleServer()
        {   
            try
            {
                mIpAddrProcess = runtime.Exec("/system/bin/ping -c 1 8.8.8.8");
                int mExitValue = await mIpAddrProcess.WaitForAsync();
                if (mExitValue == 0)
                {
                    Dismiss();
                    return true;
                }
                else
                {
                    progress.Visibility = ViewStates.Invisible;
                    btnRetry.Enabled = true;
                    btnRetry.SetText(retryText1);
                    noNetHeader.SetText(headerText1);
                    noNetSub.SetText(subTitle1);
                    return false;
                }
            }
            catch (InterruptedException)
            {

            }
            catch (IOException)
            {
               
            }
            return false;
        }
    }
}
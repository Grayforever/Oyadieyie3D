using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.LocalBroadcastManager.Content;
using Oyadieyie3D.BroadcastReceivers;
using Oyadieyie3D.Fragments;
using static Android.App.Application;

namespace Oyadieyie3D.HelperClasses
{
    //[Application(Name = "Oyadieyie3D.HelperClasses.AppLifecycleObserver")]
    public class AppLifecycleObserver : Application, IActivityLifecycleCallbacks, IServiceConnection
    {
        private int activityReferences = 0;
        private bool isActivityChangingConfigurations = false;
        private bool isFingerprintOn;
        private Intent intent;
        private IntentFilter filter;

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            PreferenceHelper.Init(this);
            isFingerprintOn = PreferenceHelper.Instance.GetBoolean(Constants.BioStatusKey);
            if (isFingerprintOn)
            {
                intent = new Intent(this, typeof(BiometricPromptTimerService));
                filter = new IntentFilter(BiometricPromptTimerService.TimeInfo);
                BindService(intent, this, Bind.None);
            }
            else
            {
                return;
            }
        }
        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            if (isFingerprintOn)
            {
                ShowBiometricDialog();
            }
        }

        public void OnActivityDestroyed(Activity activity)
        {
            UnbindService(this);
        }

        public void OnActivityPaused(Activity activity)
        {
            
        }

        public void OnActivityResumed(Activity activity)
        {
            
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            
        }

        public void OnActivityStarted(Activity activity)
        {
            if (++activityReferences == 1 && !isActivityChangingConfigurations)
            {
                
            }
        }

        public void OnActivityStopped(Activity activity)
        {
            isActivityChangingConfigurations = activity.IsChangingConfigurations;
            if (--activityReferences == 0 && !isActivityChangingConfigurations)
            {
                StartService();
            }
        }

        public void OnServiceConnected(ComponentName name, IBinder service) { }

        public void OnServiceDisconnected(ComponentName name) { }

        private void StartService()
        {
            LocalBroadcastManager.GetInstance(this).RegisterReceiver(new AppBroadcastReceiver((context, intent) =>
            {
                if (intent != null && intent.Action.Equals(BiometricPromptTimerService.TimeInfo))
                {
                    if (!intent.HasExtra("VALUE"))
                        return;

                    switch (intent.GetStringExtra("VALUE"))
                    {
                        case "Stopped":
                        case "InProgress":
                            break;
                        default:
                            ShowBiometricDialog();
                            break;
                    }
                }
            }), filter);
            StartService(intent);
        }

        private void ShowBiometricDialog()
        {
            var bioPromptFragment = new BiometricPromptSheet();
            bioPromptFragment.Cancelable = false;
            //bioPromptFragment.Show(FragmentManager, "bio");
        }
    }
}
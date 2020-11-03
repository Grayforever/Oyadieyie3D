using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.LocalBroadcastManager.Content;
using AndroidX.Preference;
using Java.Lang;
using Oyadieyie3D.Events;
using Oyadieyie3D.HelperClasses;
using System;

namespace Oyadieyie3D.BroadcastReceivers
{

    [Register("id.Oyadieyie3D.BroadcastReceivers.BiometricPromptTimerService")]
    public class BiometricPromptTimerService : Service
    {
        public static string TimerStatusKey = "VALUE";
        private CountdownTimer timer;
        private ISharedPreferences prefs;
        private int duration;
        public static string TimeInfo = "time_info";

        private IBinder localBinder;
        
        public override void OnCreate()
        {
            base.OnCreate();
            localBinder = new LocalBinder(this);
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            string strDuration = prefs.GetString(Constants.LockDuration, GetString(Resource.String.default_lock));
            duration = int.Parse(strDuration); 
        }
        public override IBinder OnBind(Intent intent) => localBinder;

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (intent == null)
                return StartCommandResult.Sticky;

            AsyncTask.Execute(new Runnable(()=> 
            {
                Looper.Prepare();
                timer = new CountdownTimer(duration, 1000,
                () =>
                {
                    var timeInfoIntent = new Intent(TimeInfo);
                    timeInfoIntent.PutExtra(TimerStatusKey, "Completed");
                    LocalBroadcastManager.GetInstance(this).SendBroadcast(timeInfoIntent);
                }, (millisUntilFinished) =>
                {
                    var secondsLeft = millisUntilFinished / 1000;
                    var timeInfoIntent = new Intent(TimeInfo);
                    timeInfoIntent.PutExtra(TimerStatusKey, "InProgress");
                    LocalBroadcastManager.GetInstance(this).SendBroadcast(timeInfoIntent);
                });
                timer.Start();
                Looper.Loop();
            }));
            
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if(timer != null)
                timer.Cancel();

            var timeInfoIntent = new Intent(TimeInfo);
            timeInfoIntent.PutExtra(TimerStatusKey, "Stopped");
            LocalBroadcastManager.GetInstance(this).SendBroadcast(timeInfoIntent);
        }


        public class LocalBinder : Binder
        {
            public LocalBinder(BiometricPromptTimerService service)
            {
                PromptTimerService = service;
            }
            BiometricPromptTimerService PromptTimerService { get; }
        }


        public class Runnable : Java.Lang.Object, IRunnable
        {
            private Action run;
            public Runnable(Action run)
            {
                this.run = run;
            }
            public void Run()
            {
                run.Invoke();
            }
        }
    }
}
using Android.App;
using Android.Content;
using Android.Runtime;
using AndroidX.Lifecycle;
using Java.Interop;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Events;
using Oyadieyie3D.HelperClasses;
using System;

namespace Oyadieyie3D.Callbacks
{
    [Application]
    public class AppLifecycleObserver : Application, ILifecycleObserver
    {
        private CountdownTimer timer;

        private enum TimerStatus { NotAvailable, Complete, InProgress}
        private TimerStatus status = TimerStatus.NotAvailable;

        public AppLifecycleObserver(IntPtr javaReference, JniHandleOwnership transfer) 
            : base(javaReference, transfer) { }

        public override void OnCreate()
        {
            base.OnCreate();
            ProcessLifecycleOwner.Get().Lifecycle.AddObserver(this);
            PreferenceHelper.Init(this);
            status = TimerStatus.NotAvailable;
        }

        [Lifecycle.Event.OnStop]
        [Export]
        public void OnAppBackgrounded() => CreateTimer();

        [Lifecycle.Event.OnStart]
        [Export]
        public void OnAppForegrounded()
        {
            switch (status)
            {
                case TimerStatus.Complete:
                    timer.Cancel();
                    timer = null;
                    var intent = new Intent(this, typeof(FingerprintActivity));
                    StartActivity(intent);
                    break;

                case TimerStatus.InProgress:
                    timer.Cancel();
                    timer = null;
                    break;

                case TimerStatus.NotAvailable:
                    break;
            }
        }

        [Lifecycle.Event.OnDestroy]
        [Export]
        public void OnAppDestroyed()
        {
            if (timer != null)
                timer.Cancel();
        }

        private void CreateTimer()
        {
            bool isFingerprintEnabled = PreferenceHelper.Instance.GetBoolean(Constants.BioStatusKey);

            if (isFingerprintEnabled)
            {
                string strDuration = PreferenceHelper.Instance.GetString(Constants.LockDuration, GetString(Resource.String.default_lock));
                int duration = int.Parse(strDuration);
                timer = new CountdownTimer(duration, 1000,
                onFinsh: () =>
                {
                    status = TimerStatus.Complete;
                }, onTick: (millisUntilFinished) =>
                {
                    status = TimerStatus.InProgress;
                });
                timer.Start();
            }
        }
    }
}
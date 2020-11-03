using Android.Content;
using System;

namespace Oyadieyie3D.BroadcastReceivers
{
    public class AppBroadcastReceiver : BroadcastReceiver
    {
        private Action<Context, Intent> onReceive;
        public AppBroadcastReceiver(Action<Context, Intent> onReceive)
        {
            this.onReceive = onReceive;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            onReceive.Invoke(context, intent);
        }
    }
}
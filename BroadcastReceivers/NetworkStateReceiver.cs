using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Net;
using Oyadieyie3D.Activities;

namespace Oyadieyie3D.BroadcastReceivers
{
    internal sealed class NetworkStateReceiver : BroadcastReceiver
    {

        public NetworkStateReceiver()
        {
            
        }
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent == null || intent.Extras == null)
                return;

            var connManager = context.GetSystemService(Context.ConnectivityService) as ConnectivityManager;
            connManager.RegisterDefaultNetworkCallback(new DefaultNetCallback(
                (net, capa)=> 
                {
                    var connected = capa.HasCapability(NetCapability.Internet);
                    OnboardingActivity.Instance.ShowNoNetDialog(connected);
                }, (net)=> 
                {
                    OnboardingActivity.Instance.ShowNoNetDialog(false);
                }));
        }


        internal sealed class DefaultNetCallback : ConnectivityManager.NetworkCallback
        {
            private Action<Network, NetworkCapabilities> _onCapaChanged;
            private Action<Network> _onLost;
            public DefaultNetCallback(Action<Network, NetworkCapabilities> onCapaChanged, Action<Network> onLost)
            {
                _onCapaChanged = onCapaChanged;
                _onLost = onLost;
            }
            public override void OnCapabilitiesChanged(Network network, NetworkCapabilities networkCapabilities)
            {
                _onCapaChanged?.Invoke(network, networkCapabilities);
            }

            public override void OnLost(Network network)
            {
                _onLost?.Invoke(network);
            }
        }
    }
}
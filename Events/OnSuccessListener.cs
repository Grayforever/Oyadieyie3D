using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Oyadieyie3D.Events
{
    internal sealed class OnSuccessListener : Java.Lang.Object, Android.Gms.Tasks.IOnSuccessListener, Android.Gms.Tasks.IOnFailureListener
    {
        private Action<Java.Lang.Exception> _onFailure;
        private Action<Java.Lang.Object> _onSuccess;

        public OnSuccessListener(Action<Java.Lang.Object> onSuccess, Action<Java.Lang.Exception> onFailure)
        {
            _onSuccess = onSuccess;
            _onFailure = onFailure;
        }
        public void OnFailure(Java.Lang.Exception e)
        {
            _onFailure?.Invoke(e);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            _onSuccess?.Invoke(result);
        }
    }
}
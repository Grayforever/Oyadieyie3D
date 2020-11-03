using System;
using Android.Views;
using Object = Java.Lang.Object;

namespace Oyadieyie3D.Events
{
    internal sealed class MyViewOnClickListener : Object, View.IOnClickListener
    {
        private readonly Action<View> _onClick;

        public MyViewOnClickListener(Action<View> onClick)
        {
            _onClick = onClick;
        }

        public void OnClick(View v) => _onClick?.Invoke(v);
    }
}
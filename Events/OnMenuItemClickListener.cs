using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;

namespace Oyadieyie3D.Events
{
    internal sealed class OnMenuItemClickListener : Java.Lang.Object, AndroidX.AppCompat.Widget.Toolbar.IOnMenuItemClickListener
    {
        private Action<IMenuItem> _onMenuItemClick;
        private bool _isClick;
        public OnMenuItemClickListener(Action<IMenuItem> onMenuItemClick, bool isClick)
        {
            _onMenuItemClick = onMenuItemClick;
            _isClick = isClick;
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            _onMenuItemClick?.Invoke(item);
            return _isClick;
        }
    }
}
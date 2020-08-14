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
using Google.Android.Material.Button;

namespace Oyadieyie3D.Events
{
    public sealed class ButtonCheckedListener : Java.Lang.Object, MaterialButtonToggleGroup.IOnButtonCheckedListener
    {
        private Action<MaterialButtonToggleGroup, int, bool> _onbuttonChecked;

        public ButtonCheckedListener(Action<MaterialButtonToggleGroup, int, bool> onbuttonChecked)
        {
            _onbuttonChecked = onbuttonChecked;
        }
        public void OnButtonChecked(MaterialButtonToggleGroup p0, int p1, bool p2)
        {
            _onbuttonChecked?.Invoke(p0, p1, p2);
        }
    }
}
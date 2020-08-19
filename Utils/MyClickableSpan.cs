using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text.Style;
using Android.Views;
using Android.Widget;

namespace Oyadieyie3D.Utils
{
    internal sealed class MyClickableSpan : ClickableSpan
    {
        private Action<View> _onClick;
        public MyClickableSpan(Action<View> onClick)
        {
            _onClick = onClick;
        }
        public override void OnClick(View widget)
        {
            _onClick?.Invoke(widget);
        }
    }
}
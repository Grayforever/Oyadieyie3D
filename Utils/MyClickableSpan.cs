using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;

namespace Oyadieyie3D.Utils
{
    internal sealed class MyClickableSpan : ClickableSpan
    {
        private Action<View> _onClick;
        private Action<TextPaint> _updateDrawState;
        public MyClickableSpan(Action<View> onClick, Action<TextPaint> updateDrawState)
        {
            _onClick = onClick;
            _updateDrawState = updateDrawState;
        }
        public override void OnClick(View widget)
        {
            _onClick?.Invoke(widget);
        }

        public override void UpdateDrawState(TextPaint ds)
        {
            base.UpdateDrawState(ds);
            _updateDrawState?.Invoke(ds);
        }
    }
}
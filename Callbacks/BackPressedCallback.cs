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
using AndroidX.Activity;

namespace Oyadieyie3D.Callbacks
{
    internal sealed class BackPressedCallback : OnBackPressedCallback
    {
        private Action _handleOnBackPressed;
        public BackPressedCallback(Action handleOnBackPressed):base(true)
        {
            _handleOnBackPressed = handleOnBackPressed;
        }
        public override void HandleOnBackPressed()
        {
            _handleOnBackPressed?.Invoke();
        }
    }
}
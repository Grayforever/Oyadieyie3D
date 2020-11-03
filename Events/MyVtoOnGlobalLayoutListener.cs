using System;
using Android.Views;

namespace Oyadieyie3D.Events
{
    internal sealed class MyVtoOnGlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private EventHandler<EventArgs> _onGlobalLayout;

        public event EventHandler<EventArgs> GlobalLayoutEvent
        {
            add
            {
                lock (this)
                {
                    _onGlobalLayout += value;
                }
            }
            remove
            {
                lock (this)
                {
                    _onGlobalLayout -= value;
                }
            }
        }

        public void OnGlobalLayout() => _onGlobalLayout?.Invoke(this, EventArgs.Empty);
    }
}
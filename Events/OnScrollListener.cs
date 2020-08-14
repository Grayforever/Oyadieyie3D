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
using AndroidX.RecyclerView.Widget;

namespace Oyadieyie3D.Events
{
    internal sealed class OnscrollListener : RecyclerView.OnScrollListener
    {
        private Action<RecyclerView, int, int> _onScrolled;

        public OnscrollListener(Action<RecyclerView, int, int> onScrolled)
        {
            _onScrolled = onScrolled;
        }
        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            _onScrolled?.Invoke(recyclerView, dx, dy);
        }
    }
}
using Android.Views;
using Google.Android.Material.BottomSheet;
using System;

namespace Oyadieyie3D.Callbacks
{
    public class BottomSheetCallback : BottomSheetBehavior.BottomSheetCallback
    {
        private Action<View, float> _onSlide;
        private Action<View, int> _onStateChanged;
        public BottomSheetCallback(Action<View, float> onSlide, Action<View, int> onStateChanged)
        {
            _onSlide = onSlide;
            _onStateChanged = onStateChanged;
        }
        public override void OnSlide(View bottomSheet, float newState) => _onSlide?.Invoke(bottomSheet, newState);

        public override void OnStateChanged(View p0, int p1) => _onStateChanged?.Invoke(p0, p1);
    }
}
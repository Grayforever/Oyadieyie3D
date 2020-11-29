using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.CoordinatorLayout.Widget;
using Google.Android.Material.BottomSheet;

namespace Oyadieyie3D.Utils
{
    [Register("id.Oyadieyie3D.Utils.LockableBottomsheetBehavior")]
    public class LockableBottomsheetBehavior : BottomSheetBehavior
    {
        private bool swipeEnabled = true;
        public LockableBottomsheetBehavior() : base() { }

        public LockableBottomsheetBehavior(Context context, IAttributeSet attrs):base(context, attrs) { }

        public override bool OnInterceptTouchEvent(CoordinatorLayout parent, Java.Lang.Object child, MotionEvent ev)
        {
            if(swipeEnabled)
                base.OnInterceptTouchEvent(parent, child, ev);

            return false;
        }

        public override bool OnTouchEvent(CoordinatorLayout parent, Java.Lang.Object child, MotionEvent ev)
        {
            if(swipeEnabled)
                base.OnTouchEvent(parent, child, ev);

            return false;
        }

        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int axes, int type)
        {
            if(swipeEnabled)
                base.OnStartNestedScroll(coordinatorLayout, child, directTargetChild, target, axes, type);

            return false;
        }

        public override void OnNestedPreScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dx, int dy, int[] consumed, int type)
        {
            if(swipeEnabled)
                base.OnNestedPreScroll(coordinatorLayout, child, target, dx, dy, consumed, type);

        }

        public override bool OnNestedPreFling(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, float velocityX, float velocityY)
        {
            if(swipeEnabled)
                base.OnNestedPreFling(coordinatorLayout, child, target, velocityX, velocityY);

            return false;
        }
    }
}
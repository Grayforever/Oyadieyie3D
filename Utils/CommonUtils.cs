using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using R = Oyadieyie3D.Resource;

namespace Oyadieyie3D.Utils
{
    public class CommonUtils
    {
        public static bool IsLand(Context context)
            => context.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Landscape;

        public static Point GetDisplayDimen(Context context)
        {
            var display = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>().DefaultDisplay;
            var size = new Point();
            display.GetSize(size);
            return size;
        }

        public static int GetStatusBarHeightPixel(Context context)
        {
            int result = 0;
            int resourceId = context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                result = context.Resources.GetDimensionPixelSize(resourceId);
            }
            return result;
        }

        public static int GetActionBarHeightPixel(Context context)
        {
            TypedValue tv = new TypedValue();
            if (context.Theme.ResolveAttribute(Android.Resource.Attribute.ActionBarSize, tv, true))
            {
                return TypedValue.ComplexToDimensionPixelSize(tv.Data, context.Resources.DisplayMetrics);
            }
            else if (context.Theme.ResolveAttribute(R.Attribute.actionBarSize, tv, true))
            {
                return TypedValue.ComplexToDimensionPixelSize(tv.Data, context.Resources.DisplayMetrics);
            }
            else
            {
                return 0;
            }
        }

        public static int GetTabHeight(Context context)
            => context.Resources.GetDimensionPixelSize(R.Dimension.tab_height);
    }
}
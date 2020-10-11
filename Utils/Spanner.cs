using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Text.Method;
using Android.Widget;
using System;

namespace Oyadieyie3D.Utils
{
    public class Spanner
    {
        private Context _context;
        private bool _shouldChangeColor;

        public event EventHandler OnSpanClick;
        public Spanner(Context context, bool shouldChangeColor)
        {
            _context = context;
            _shouldChangeColor = shouldChangeColor;
        }

        public void SetSpan(TextView spanner_tv, string[] textToSpanArray)
        {
            SpannableString ss = new SpannableString(textToSpanArray[0] + textToSpanArray[1]);

            var cs = new MyClickableSpan((widget) =>
            {
                OnSpanClick?.Invoke(this, new EventArgs());
            }, (ds) =>
            {
                if(_shouldChangeColor == true)
                    ds.Color = Color.ParseColor("#3F51B5");

                ds.UnderlineText = false;
                ds.SetTypeface(Typeface.DefaultBold);
                
            });
            ss.SetSpan(cs, textToSpanArray[0].Length, textToSpanArray[0].Length + textToSpanArray[1].Length, SpanTypes.ExclusiveExclusive);
            spanner_tv.TextFormatted = ss;
            spanner_tv.MovementMethod = LinkMovementMethod.Instance;
            spanner_tv.SetHighlightColor(Color.Transparent);
        }


    }
}
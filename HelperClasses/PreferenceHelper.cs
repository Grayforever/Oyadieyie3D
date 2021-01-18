using Android.Content;
using Android.Widget;
using AndroidX.Preference;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Java.Lang;

namespace Oyadieyie3D.HelperClasses
{
    public enum PlaceholderType
    {
        Normal,
        Profile,
        Landscape
    }

    public class PreferenceHelper
    {
        private static ISharedPreferences preferences;
        private static PreferenceHelper prefHelper;

        public PreferenceHelper(Context context)
        {
            preferences = PreferenceManager.GetDefaultSharedPreferences(context);
        }

        public bool ContainsKey(string key)
        {
            return preferences.Contains(key);
        }

        public void ClearPrefs()
        {
            var editor = preferences.Edit();
            editor.Clear();
            editor.Apply();
        }

        public static void Init(Context context)
        {
            if (context == null)
            {
                throw new NullPointerException("provided application context is null");
            }

            if (prefHelper == null)
            {
                lock (typeof(PreferenceHelper))
                {
                    if (prefHelper == null)
                    {
                        prefHelper = new PreferenceHelper(context);
                    }
                }
            }
        }

        public static PreferenceHelper Instance => prefHelper switch
        {
            null => throw new IllegalStateException("Shared preference is not initialized, call init"),
            _ => prefHelper
        };

        public void SetString(string key, string value)
        {
            var editor = preferences.Edit();
            editor.PutString(key, value);
            editor.Apply();
        }

        public static void RemoveKey(string key)
        {
            preferences.Edit().Remove(key).Apply();
        }

        public string GetString(string key, string defValue) => preferences.GetString(key, defValue);

        public bool GetBoolean(string key) => preferences.GetBoolean(key, false);

        public void SetBoolean(string key, bool isBoolean)
        {
            var editor = preferences.Edit();
            editor.PutBoolean(key, isBoolean);
            editor.Apply();
        }

        public void SetInt(string key, int num)
        {
            var editor = preferences.Edit();
            editor.PutInt(key, num);
            editor.Apply();
        }

        public int GetInt(string key) => preferences.GetInt(key, 0);

        public void LoggedOut() => ClearPrefs();

        internal void SetImageResource(ImageView imageview, string url, PlaceholderType placeholderType)
        {
            int placeholder = Resource.Drawable.placeholder;
            switch (placeholderType)
            {
                case PlaceholderType.Normal:
                    placeholder = Resource.Drawable.placeholder;
                    break;
                case PlaceholderType.Profile:
                    placeholder = Resource.Drawable.ic_account_circle;
                    break;
                case PlaceholderType.Landscape:
                    placeholder = Resource.Drawable.placeholder;
                    break;
            }
            var op = new RequestOptions();
            op.Placeholder(placeholder);

            Glide.With(imageview.Context)
                .SetDefaultRequestOptions(op)
                .Load(url)
                .Into(imageview);
        }
    }
}
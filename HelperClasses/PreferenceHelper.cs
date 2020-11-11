using Android.Content;
using AndroidX.Preference;
using Java.Lang;

namespace Oyadieyie3D.HelperClasses
{
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
    }
}
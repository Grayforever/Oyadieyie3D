using Android.Content;
using Android.Media;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using AndroidX.Preference;

namespace Oyadieyie3D.Fragments
{
    [Register("id.Oyadieyie3D.Fragments.PrefNotifsFragment")]
    public class PrefNotifsFragment : PreferenceFragmentCompat
    {
        private const string Ringtone_Key = "ringtone";
        private const int RingtoneRequestCode = 500;
        private ISharedPreferences prefs;
        private Preference ringtonePref;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.notifications_pref_screen, rootKey);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            ringtonePref = PreferenceScreen.FindPreference(Ringtone_Key);
            ringtonePref.Summary = GetToneName(prefs.GetString(Ringtone_Key, Settings.System.DefaultNotificationUri.ToString()));
        }

        private string GetToneName(string uri)
        {
            return RingtoneManager.GetRingtone(Context, Uri.Parse(uri)).GetTitle(Context);
        }

        public override bool OnPreferenceTreeClick(Preference preference)
        {
            if (preference.Key.Equals(Ringtone_Key))
            {
                
                var intent = new Intent(RingtoneManager.ActionRingtonePicker);
                intent.PutExtra(RingtoneManager.ExtraRingtoneType, (int)RingtoneType.Notification);
                intent.PutExtra(RingtoneManager.ExtraRingtoneShowDefault, true);
                intent.PutExtra(RingtoneManager.ExtraRingtoneDefaultUri, Settings.System.DefaultNotificationUri);

                string existingValue = GetRingTonePreferenceValue();
                if (existingValue != null)
                {
                    if(existingValue.Length == 0)
                    {
                        intent.PutExtra(RingtoneManager.ExtraRingtoneExistingUri, (Uri)null);
                    }
                    else
                    {
                        intent.PutExtra(RingtoneManager.ExtraRingtoneExistingUri, Uri.Parse(existingValue));
                    }
                    
                }
                else
                {
                    intent.PutExtra(RingtoneManager.ExtraRingtoneExistingUri, Settings.System.DefaultNotificationUri);
                }
                StartActivityForResult(intent, RingtoneRequestCode);
                return true;
            }
            else
            {
                return base.OnPreferenceTreeClick(preference);
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if(requestCode == RingtoneRequestCode && data != null)
            {
                Uri uri = data.GetParcelableExtra(RingtoneManager.ExtraRingtonePickedUri) as Uri;
                if(uri != null)
                {
                    SetRingtonePreferenceValue(uri.ToString());

                }
                else
                {
                    SetRingtonePreferenceValue("");
                }
            }
            else
            {
                base.OnActivityResult(requestCode, resultCode, data);
            }
        }

        private void SetRingtonePreferenceValue(string uri)
        {
            var editor = prefs.Edit();
            editor.PutString(Ringtone_Key, uri);
            editor.Commit();
            ringtonePref.Summary = GetToneName(uri);
        }

        private string GetRingTonePreferenceValue()
        {
            return prefs.GetString(Ringtone_Key, "content://settings/system/notification_sound");
        }
    }
}
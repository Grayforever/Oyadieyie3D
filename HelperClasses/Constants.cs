using Android.Net.Wifi.Aware;

namespace Oyadieyie3D.HelperClasses
{
    public static class Constants
    {
        //INTENTS & KEYS
        public static string POST_DATA_EXTRA => "extra_post_data";
        public static string TRANSITION_NAME => "extra_transition_name";
        public static string PARCEL_TYPE => "parcel_type";
        public static string IMG_URL_KEY => "img_url";
        public static string PROFILE_DATA_EXTRA => "extra_profile_data";
        public static string SWITCH_VALUE_EXTRA => "online_switch";

        //USER CONSTANTS
        public static string SNAPSHOT_FNAME => "fname";
        public static string SNAPSHOT_EMAIL => "email";
        public static string SNAPSHOT_PHONE => "phone";
        public static string SNAPSHOT_GENDER => "gender";
        public static string SNAPSHOT_STATUS => "status";
        public static string SNAPSHOT_PHOTO_URL => "photoUrl";

        //POST CONSTANTS
        public static string SNAPSHOT_POST_BODY => "post_body";
        public static string SNAPSHOT_POST_IMAGE_ID => "image_id";
        public static string SNAPSHOT_POST_OWNER_ID => "owner_id"; 
        public static string SNAPSHOT_POST_DOWNLOAD_URL => "download_url";
        public static string SNAPSHOT_POST_POST_DATE => "post_date";

        //EXCEPTION MSGS
        public static string EXCEPTION_MSG_DATA => "";
        public static string EXCEPTION_MSG_AUTH => "";
        public static string EXCEPTION_MSG_NRE => "";
        public static string EXCEPTION_MSG_NETWORK => "";


        public static string ONBDTITLE_1 => "Find tailors";
        public static string ONBDTITLE_2 => "Advertize your collection";
        public static string ONBDTITLE_3 => "Seamless payment";
        public static string ONBDESC_1 => "All tailors are sorted by customer rating";
        public static string ONBDESC_2 => "Get your collection advertized on the platform for free";
        public static string ONBDESC_3 => "Make payment and get paid directly in app";

        public static string DIALOG_OK => "OK";
        public static string DIALOG_LOADING => "Loading...";
        public static string DIALOG_SUCCESS => "Success";


        public static string PREF_NAME => "userInfo";
        public static string REF_USER_SESSION => "session";

        public static string SESION_CHILD => "status";

        public const string REG_STAGE_CREATE_PROFILE = "create_profile";

        public const string REG_STAGE_SET_PARTNER = "set_partner";

        public const string REG_STAGE_DONE = "reg_done";

    }
}
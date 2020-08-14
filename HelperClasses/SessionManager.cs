using Android.App;
using Android.Content;
using Firebase;
using Firebase.Auth;
//using Firebase.Firestore;

namespace Oyadieyie3D.HelperClasses
{
    public static class SessionManager
    {
        static ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        static ISharedPreferencesEditor editor;
        //public static FirebaseFirestore GetFirestore()
        //{
        //    var app = FirebaseApp.InitializeApp(Application.Context);
        //    FirebaseFirestore database;

        //    if (app == null)
        //    {
        //        var options = new FirebaseOptions.Builder()
        //            .SetProjectId("oyadieyie3d")
        //            .SetApplicationId("oyadieyie3d")
        //            .SetApiKey("AIzaSyD_uotb9-fNmlfII93imcZFNM-oALIisHw")
        //            .SetDatabaseUrl("https://oyadieyie3d.firebaseio.com")
        //            .SetStorageBucket("oyadieyie3d.appspot.com")
        //            .Build();

        //        app = FirebaseApp.InitializeApp(Application.Context, options);
        //        database = FirebaseFirestore.GetInstance(app);
        //    }
        //    else
        //    {
        //        database = FirebaseFirestore.GetInstance(app);
        //    }
        //    return database;
        //}

        public static FirebaseAuth GetFirebaseAuth()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseAuth mAuth;

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetProjectId("oyadieyie3d")
                    .SetApplicationId("oyadieyie3d")
                    .SetApiKey("AIzaSyD_uotb9-fNmlfII93imcZFNM-oALIisHw")
                    .SetDatabaseUrl("https://oyadieyie3d.firebaseio.com")
                    .SetStorageBucket("oyadieyie3d.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
                mAuth = FirebaseAuth.Instance;
            }
            else
            {
                mAuth = FirebaseAuth.Instance;
            }
            return mAuth;
        }

        public static void SaveFullName(string fullname)
        {
            editor = preferences.Edit();
            editor.PutString("fullname", fullname);
            editor.Apply();
        }

        public static string GetFullName()
        {
            string fullname = "";
            fullname = preferences.GetString("fullname", "");
            return fullname;
        }
    }
}
using Android.App;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

namespace Oyadieyie3D.HelperClasses
{
    public static class SessionManager
    {
        private static FirebaseApp InitFireApp()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetProjectId("oyadieyie3d")
                    .SetApplicationId("oyadieyie3d")
                    .SetApiKey("AIzaSyD_uotb9-fNmlfII93imcZFNM-oALIisHw")
                    .SetDatabaseUrl("https://oyadieyie3d.firebaseio.com")
                    .SetStorageBucket("oyadieyie3d.appspot.com")
                    .Build();

                app =  FirebaseApp.InitializeApp(Application.Context, options);
            }

            return app;
        }

        public static FirebaseAuth GetFirebaseAuth()
        {
            InitFireApp();
            return FirebaseAuth.Instance;
        }

        public static FirebaseDatabase GetFireDB()
        {
            InitFireApp();
            return FirebaseDatabase.Instance;
        }

        public static string UserId => GetFirebaseAuth().CurrentUser.Uid;

        public static DatabaseReference UserRef => GetFireDB().GetReference("users");
    }
}
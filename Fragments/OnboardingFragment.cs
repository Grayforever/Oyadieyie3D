using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Firebase.Auth;
using Google.Android.Material.TextField;
using Org.Json;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Events;
using Oyadieyie3D.HelperClasses;
using Oyadieyie3D.Utils;
using System;
using System.Collections.Generic;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using static Xamarin.Facebook.GraphRequest;

namespace Oyadieyie3D.Fragments
{
    public class OnboardingFragment : Fragment, IFacebookCallback, IGraphJSONObjectCallback
    {
        private TextInputLayout phoneEt;
        private const string TAG = "phone_et";
        private string[] textToSpanarray = { "Or log in with ", "Facebook" };
        private LoginResult loginResult;

        private ICallbackManager callbackManager;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            callbackManager = CallbackManagerFactory.Create();
            LoginManager.Instance.RegisterCallback(callbackManager, this);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.get_started_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            phoneEt = view.FindViewById<TextInputLayout>(Resource.Id.phone_et);
            var gsPhoneEt = view.FindViewById<TextInputEditText>(Resource.Id.gs_phone_et);
            var facebookTv = view.FindViewById<TextView>(Resource.Id.facebook_log_btn);
            var spanner = new Spanner(Activity, true);
            spanner.SetSpan(facebookTv, textToSpanarray);
            spanner.OnSpanClick += (s1, e1) =>
            {
                LoginManager.Instance.LogInWithReadPermissions(this, new List<string> { "public_profile", "email" });
            };
            gsPhoneEt.Click += GsPhoneEt_Click;
        }

        private void GsPhoneEt_Click(object sender, EventArgs e)
        {
            ParentFragmentManager.BeginTransaction()
                .AddSharedElement(phoneEt, "phone_et")
                .AddToBackStack(TAG)
                .Replace(Resource.Id.frag_container, new EnterPhoneFragment())
                .CommitAllowingStateLoss();
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        private void SetFacebookData(LoginResult loginResult)
        {
            GraphRequest graphRequest = GraphRequest.NewMeRequest(loginResult.AccessToken, this);
            Bundle parameters = new Bundle();
            parameters.PutString("fields", "id,email,first_name,last_name,picture");
            graphRequest.Parameters = parameters;
            graphRequest.ExecuteAsync();
        }

        public void OnCompleted(JSONObject @object, GraphResponse response)
        {
            try
            {
                string fbId = response.JSONObject.GetString("id");
                string _email = response.JSONObject.GetString("email");
                string firstname = response.JSONObject.GetString("first_name");
                string lastname = response.JSONObject.GetString("last_name");

                var intent = new Intent(Activity, typeof(MainActivity));
                intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask);
                StartActivity(intent);
            }
            catch (JSONException e)
            {
                e.PrintStackTrace();
            }
        }

        public void OnCancel()
        {
            OnboardingActivity.ShowError("Login canceled", "You canceled the login operation");
        }

        public void OnError(FacebookException error)
        {
            OnboardingActivity.ShowError(error.Source, error.Message);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            loginResult = result as LoginResult;

            var credentials = FacebookAuthProvider.GetCredential(loginResult.AccessToken.Token);
            SessionManager.GetFirebaseAuth().SignInWithCredential(credentials)
                .AddOnCompleteListener(new OncompleteListener((t) => 
                {
                    if (!t.IsSuccessful)
                        return;
                    OnboardingActivity.ShowSuccess();
                }));
        }
    }
}
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using CN.Pedant.SweetAlert;
using Firebase.Auth;
using Google.Android.Material.TextField;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Events;
using Oyadieyie3D.HelperClasses;
using Oyadieyie3D.Utils;
using System.Collections.Generic;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace Oyadieyie3D.Fragments
{
    public class OnboardingFragment : Fragment, IFacebookCallback
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

            gsPhoneEt.Click += (s2, e2) => GotoEnterPhone();

            var spanner = new Spanner(Activity, true);
            spanner.SetSpan(facebookTv, textToSpanarray);
            spanner.OnSpanClick += (s1, e1) => LoginManager.Instance.LogInWithReadPermissions(this, new List<string> { "public_profile", "email" });
        }

        private void GotoEnterPhone()
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
            callbackManager.OnActivityResult(requestCode, resultCode, data);
        }

        public void OnCancel() => Toast.MakeText(Activity, "Facebook login cancelled", ToastLength.Short).Show();

        public void OnError(FacebookException error) => OnboardingActivity.Instance.ShowError(error.Source, error.Message);

        public void OnSuccess(Java.Lang.Object result)
        {
            loginResult = result as LoginResult;
            var credentials = FacebookAuthProvider.GetCredential(loginResult.AccessToken.Token);
            var loginresult = SessionManager.GetFirebaseAuth().SignInWithCredentialAsync(credentials);

            var user = loginresult.Result as FirebaseUser;
            if (user == null)
                ShowUserNotfound();

            var userRef = SessionManager.GetFireDB().GetReference("users");
            userRef.OrderByKey().EqualTo(user.Uid).AddValueEventListener(new SingleValueListener(
            (s) =>
            {
                switch (s.Exists())
                {
                    case false:
                        ShowUserNotfound();
                        break;

                    default:
                        var intent = new Intent(Activity, typeof(MainActivity));
                        intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask); StartActivity(intent);
                        break;
                }
            }, (e) =>
            {
                OnboardingActivity.Instance.ShowError("Database error", e.Message);
            }));
        }

        private void ShowUserNotfound()
        {
            var resolverDialog = new SweetAlertDialog(Context, SweetAlertDialog.ErrorType);
            resolverDialog.SetTitleText("User not found");
            resolverDialog.SetContentText("You can only log in with facebook after we link your Oyadieyie account with Facebook. Do you wish to sign up instead?");
            resolverDialog.SetConfirmText("Yes");
            resolverDialog.SetCancelText("I dont think so");
            resolverDialog.SetCanceledOnTouchOutside(false);
            resolverDialog.SetCancelable(false);
            resolverDialog.SetConfirmClickListener(new SweetConfirmClick(
                (sad) => 
                {
                    GotoEnterPhone();
                    SessionManager.GetFirebaseAuth().SignOut();
                }));
            resolverDialog.SetCancelClickListener(new SweetConfirmClick(
                (sad) =>
                {
                    SessionManager.GetFirebaseAuth().SignOut();
                    Activity.Finish();
                }));
            resolverDialog.Show();
        }
    }
}
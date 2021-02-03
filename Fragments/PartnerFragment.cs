using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Button;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Events;
using Oyadieyie3D.HelperClasses;
using Fragment = AndroidX.Fragment.App.Fragment;
using R = Oyadieyie3D.Resource;

namespace Oyadieyie3D.Fragments
{
    public class PartnerFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            PreferenceHelper.Init(Context);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(R.Layout.account_type_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var buttonToggleGrp = view.FindViewById<MaterialButtonToggleGroup>(R.Id.acc_type_opt_tglgrp);
            var nextBtn = view.FindViewById<MaterialButton>(R.Id.acc_type_nxt_btn);
            buttonToggleGrp.AddOnButtonCheckedListener(new ButtonCheckedListener((group, id, isChecked) => 
            {
                
            }));

            nextBtn.Click += (s1, e1) =>
            {
                PreferenceHelper.Instance.SetString("firstRun", "regd");
                OnboardingActivity.Instance.SetStatus(Constants.REG_STAGE_DONE);


                string userId = SessionManager.GetFirebaseAuth().CurrentUser.Uid;
                var userRef = SessionManager.GetFireDB().GetReference($"users/{userId}/profile");
                userRef.AddValueEventListener(new SingleValueListener(
                onDataChange: (snapshot) =>
                {
                    if (!snapshot.Exists())
                        return;

                    string username = snapshot.Child(Constants.SNAPSHOT_FNAME) != null ? snapshot.Child(Constants.SNAPSHOT_FNAME).Value.ToString() : "";
                    string status = snapshot.Child(Constants.SNAPSHOT_GENDER) != null ? snapshot.Child(Constants.SNAPSHOT_GENDER).Value.ToString() : "";
                    string profileImgUrl = snapshot.Child(Constants.SNAPSHOT_PHOTO_URL) != null ? snapshot.Child(Constants.SNAPSHOT_PHOTO_URL).Value.ToString() : "";
                    string email = snapshot.Child(Constants.SNAPSHOT_EMAIL) != null ? snapshot.Child(Constants.SNAPSHOT_EMAIL).Value.ToString() : "";
                    string phone = snapshot.Child(Constants.SNAPSHOT_PHONE) != null ? snapshot.Child(Constants.SNAPSHOT_PHONE).Value.ToString() : "";

                    PreferenceHelper.Instance.SetString("username", username);
                    PreferenceHelper.Instance.SetString("status", status);
                    PreferenceHelper.Instance.SetString("profile_url", profileImgUrl);
                    PreferenceHelper.Instance.SetString("email", email);
                    PreferenceHelper.Instance.SetString("phone", phone);

                    var intent = new Intent(Activity, typeof(MainActivity));
                    intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask);
                    StartActivity(intent);
                    OnboardingActivity.Instance.DismissLoader();

                }, onCancelled: (error) =>
                {
                    Toast.MakeText(Context, error.Message, ToastLength.Short).Show();
                }));
            };
        }
    }
}
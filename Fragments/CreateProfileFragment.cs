using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using BumpTech.GlideLib;
using DE.Hdodenhof.CircleImageViewLib;
using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Java.Util;
using Oyadieyie3D.Activities;
using Oyadieyie3D.Events;
using Oyadieyie3D.HelperClasses;
using System;
using static Android.Widget.RadioGroup;
using static Oyadieyie3D.Fragments.CreatePostFragment;

namespace Oyadieyie3D.Fragments
{
    public class CreateProfileFragment : Fragment, IOnCheckedChangeListener
    {
        private TextInputEditText dobEditText;
        private FloatingActionButton fabPictureOptions;
        private CircleImageView profileImageView;
        private MaterialButton continueBtn;
        private TextInputLayout fullnameEt;
        private TextInputLayout emailEt;
        private TextInputLayout locationEt;
        private RadioGroup genderRadioGroup;
        private TextInputLayout dobLayout;
        private Android.Net.Uri img_uri;
        private static StorageReference imageRef;
        private enum Gender { Male, Female};
        private bool hasImage = false;

        private Gender userGender;

        public override void OnCreate(Bundle savedInstanceState) => base.OnCreate(savedInstanceState);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.profile_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            dobLayout = view.FindViewById<TextInputLayout>(Resource.Id.pro_dob_et);
            dobEditText = dobLayout.FindViewById<TextInputEditText>(Resource.Id.dob_et);
            fabPictureOptions = view.FindViewById<FloatingActionButton>(Resource.Id.pro_cam_fab);
            profileImageView = view.FindViewById<CircleImageView>(Resource.Id.pro_prof_iv);
            continueBtn = view.FindViewById<MaterialButton>(Resource.Id.pro_cont_btn);
            fullnameEt = view.FindViewById<TextInputLayout>(Resource.Id.pro_full_name_et);
            emailEt = view.FindViewById<TextInputLayout>(Resource.Id.pro_email_et);
            locationEt = view.FindViewById<TextInputLayout>(Resource.Id.pro_loc_et);
            genderRadioGroup = view.FindViewById<RadioGroup>(Resource.Id.pro_gender_grp);
            genderRadioGroup.SetOnCheckedChangeListener(this);
            continueBtn.Click += ContinueBtn_Click;
            fabPictureOptions.Click += FabPictureOptions_Click;
            dobEditText.Click += DobEditText_Click;

            dobEditText.TextChanged += EditText_TextChanged;
            fullnameEt.EditText.TextChanged+= EditText_TextChanged;
            emailEt.EditText.TextChanged+= EditText_TextChanged;
        }

        private void EditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e) => ShouldEnableBtn();

        private void ShouldEnableBtn()
        {
            bool isEnabled = fullnameEt.EditText.Text.Length >= 2 && Patterns.EmailAddress.Matcher(emailEt.EditText.Text).Matches() 
                && hasImage && !string.IsNullOrEmpty(dobEditText.Text);
            continueBtn.Enabled = isEnabled;
        }

        private void ContinueBtn_Click(object sender, EventArgs e) => SaveToDb();

        private async void SaveToDb()
        {
            try
            {
                OnboardingActivity.ShowLoader();
                var profileRef = SessionManager.GetFireDB().GetReference($"users/{SessionManager.UserId}/profile");
                var userMap = new HashMap();
                var stream = new System.IO.MemoryStream();
                var bitmap = MediaStore.Images.Media.GetBitmap(Context.ContentResolver, img_uri);
                await bitmap.CompressAsync(Bitmap.CompressFormat.Webp, 90, stream);
                var imgArray = stream.ToArray();

                imageRef = FirebaseStorage.Instance.GetReference($"profileImages/{SessionManager.UserId}");
                imageRef.PutBytes(imgArray).ContinueWithTask(new ContinuationTask(
                then: task =>
                {
                    if (!task.IsSuccessful)
                        throw task.Exception;

                })).AddOnCompleteListener(new OncompleteListener(
                onComplete: t =>
                {
                    if (!t.IsSuccessful)
                        throw t.Exception;

                    userMap.Put(Constants.SNAPSHOT_FNAME, fullnameEt.EditText.Text);
                    userMap.Put(Constants.SNAPSHOT_EMAIL, emailEt.EditText.Text);
                    userMap.Put(Constants.SNAPSHOT_GENDER, (int)userGender);
                    userMap.Put(Constants.SNAPSHOT_PHONE, SessionManager.GetFirebaseAuth().CurrentUser.PhoneNumber);
                    userMap.Put(Constants.SNAPSHOT_PHOTO_URL, t.Result.ToString());
                    profileRef.SetValue(userMap).AddOnCompleteListener(new OncompleteListener(
                    onComplete: task =>
                    {
                        try
                        {
                            if (!task.IsSuccessful)
                                throw task.Exception;

                            OnboardingActivity.SetStatus(Constants.REG_STAGE_SET_PARTNER);
                            ParentFragmentManager.BeginTransaction()
                                .Replace(Resource.Id.frag_container, new PartnerFragment())
                                .CommitAllowingStateLoss();

                        }
                        catch (DatabaseException de)
                        {
                            OnboardingActivity.DismissLoader();
                            OnboardingActivity.ShowError("Database Exception", de.Message);
                        }

                    }));
                    profileRef.KeepSynced(true);

                }));

            }
            catch (DatabaseException fde)
            {
                OnboardingActivity.DismissLoader();
                OnboardingActivity.ShowError("Database Exception", fde.Message);
            }
            catch (FirebaseNetworkException)
            {
                OnboardingActivity.DismissLoader();
                OnboardingActivity.ShowNoNetDialog(false);
            }
            catch (StorageException se)
            {
                OnboardingActivity.DismissLoader();
                OnboardingActivity.ShowError("Storage Exception", se.Message);
            }
            catch (Exception ex)
            {
                OnboardingActivity.DismissLoader();
                OnboardingActivity.ShowError("Exception", ex.Message);
            }
        }

        private void FabPictureOptions_Click(object sender, EventArgs e)
        {

            var picturefragment = new ProfileChooserFragment();
            picturefragment.OnCropComplete += Picturefragment_OnCropComplete;
            var ft = ChildFragmentManager.BeginTransaction();

            ft.Add(picturefragment, "Take_picture");
            ft.CommitAllowingStateLoss();
        }

        private void Picturefragment_OnCropComplete(object sender, ProfileChooserFragment.CropCompleteEventArgs e)
        {
            img_uri = e.imageUri;
            Glide.With(this).Load(img_uri).Into(profileImageView);
            hasImage = true;
        }

        private void DobEditText_Click(object sender, EventArgs e)
        {
            var datePicker = new DatePickerDialog();
            datePicker.OnDatePicked += DatePicker_OnDatePicked;
            var ft= ParentFragmentManager.BeginTransaction();
            ft.Add(datePicker, "date_picker");
            ft.CommitAllowingStateLoss();
        }

        private void DatePicker_OnDatePicked(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            var date = new DateTime(e.Year, e.Month, e.DayOfMonth);
            dobEditText.Text = date.ToShortDateString();
        }

        public void OnCheckedChanged(RadioGroup group, int checkedId)
        {
            switch (checkedId)
            {
                case Resource.Id.pro_male:
                    userGender = Gender.Male;
                    break;
                case Resource.Id.pro_female:
                    userGender = Gender.Female;
                    break;
            }
            ShouldEnableBtn();
        }

        private class ContinuationTask : Java.Lang.Object, IContinuation
        {
            private Action<Task> _then;

            public ContinuationTask(Action<Task> then)
            {
                _then = then;
            }

            public Java.Lang.Object Then(Task task)
            {
                _then?.Invoke(task);
                return imageRef.GetDownloadUrl();
            }
        }
    }
}
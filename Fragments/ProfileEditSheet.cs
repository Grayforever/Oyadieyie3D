using Android.OS;
using Android.Views;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.TextField;

namespace Oyadieyie3D.Fragments
{
    public class ProfileEditSheet : BottomSheetDialogFragment
    {
        private int _type;

        public ProfileEditSheet(int type)
        {
            _type = type;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.profile_edit_btmsht, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var editEt = view.FindViewById<TextInputLayout>(Resource.Id.prof_edit_et);
            editEt.RequestFocus();
            switch (_type)
            {
                case 0:
                    editEt.EditText.Hint = "Username";
                    editEt.EditText.InputType = Android.Text.InputTypes.TextVariationPersonName;
                    break;
                case 1:
                    editEt.EditText.Hint = "Phone";
                    editEt.EditText.InputType = Android.Text.InputTypes.ClassPhone;
                    break;    
            }
        }
    }
}
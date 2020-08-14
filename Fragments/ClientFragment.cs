using Android.OS;
using Android.Views;

namespace Oyadieyie3D.Fragments
{
    public class ClientFragment : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle(StyleNormal, Resource.Style.Base_Theme_MaterialComponents_Light_DialogWhenLarge);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.client_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

        }
    }
}
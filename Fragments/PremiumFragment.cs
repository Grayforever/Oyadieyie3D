using Android.OS;
using Android.Views;

namespace Oyadieyie3D.Fragments
{
    public class PremiumFragment : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.premium_fragment, container, false);
        }
    }
}
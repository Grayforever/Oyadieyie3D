using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using DanielStone.MaterialAboutLibrary;
using DanielStone.MaterialAboutLibrary.Items;
using DanielStone.MaterialAboutLibrary.Model;
using Java.Lang;

namespace Oyadieyie3D.Activities
{
    [Register("id.Oyadieyie3D.Activities.AboutActivity")]
    [Activity(Label = "About", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class AboutActivity : MaterialAboutActivity
    {
        protected override ICharSequence ActivityTitleFormatted => GetTextFormatted(Resource.String.app_name);

        protected override MaterialAboutList GetMaterialAboutList(Context context)
        {
            MaterialAboutCard.Builder aboutCard = new MaterialAboutCard.Builder();
            aboutCard.AddItem(new MaterialAboutTitleItem.Builder()
                .Text("Oyadieyie3D")
                .Desc("© 2020 Gray Labs")
                .Icon(Resource.Mipmap.ic_launcher)
                .Build());

            return new MaterialAboutList(aboutCard.Build());
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }
    }
}
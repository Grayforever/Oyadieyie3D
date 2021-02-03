using Google.Android.Material.Tabs;
using System;
using static Google.Android.Material.Tabs.TabLayoutMediator;

namespace Oyadieyie3D.HelperClasses
{
    public class TabConfigStrategy : Java.Lang.Object, ITabConfigurationStrategy
    {
        public Action<TabLayout.Tab, int> ConfigureTab;
        public void OnConfigureTab(TabLayout.Tab p0, int p1)
        {
            ConfigureTab(p0, p1);
        }
    }
}
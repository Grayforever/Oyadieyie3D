using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using System.Collections.Generic;

namespace Oyadieyie3D.Adapters
{
    public class FeaturedPagerAdapter : FragmentStateAdapter
    {
        private List<Fragment> FragmentList;

        public override int ItemCount => FragmentList.Count;

        public FeaturedPagerAdapter(FragmentActivity activity) : base(activity)
        {
            FragmentList = new List<Fragment>();
        }

        public override Fragment CreateFragment(int p0)
        {
            return FragmentList[p0];
        }

        public void AddFragment(Fragment fragment)
        {
            FragmentList.Add(fragment);
        }
    }
}
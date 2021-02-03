using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace Oyadieyie3D.Adapters
{
    public class CustomExpandableListAdapter : BaseExpandableListAdapter
    {
        private Context _context;
        private List<string> _expandableListTitle;
        private Dictionary<string, List<string>> _expandableListDetail;

        public CustomExpandableListAdapter(Context context, List<string> expandableListTitle, Dictionary<string, List<string>> expandableListDetail)
        {
            _context = context;
            _expandableListTitle = expandableListTitle;
            _expandableListDetail = expandableListDetail;
        }

        public override int GroupCount => throw new NotImplementedException();

        public override bool HasStableIds => throw new NotImplementedException();

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return _expandableListDetail.TryGetValue(_expandableListTitle[groupPosition], out _);
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            var key = _expandableListTitle[groupPosition];
            return _expandableListDetail[key].Count;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            string expandedListText = (string)GetChild(groupPosition, childPosition);
            if (convertView == null)
            {
                var inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.list_item, null);
            }

            var expandedListTextView = convertView.FindViewById<TextView>(Resource.Id.expandedListItem);
            expandedListTextView.Text = expandedListText;
            return convertView;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return _expandableListTitle[groupPosition];
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            string listTitle = (string)GetGroup(groupPosition);
            if (convertView == null)
            {
                var inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.list_group, null);
            }

            var listTitleTextView = convertView.FindViewById<TextView>(Resource.Id.listTitle);
            listTitleTextView.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
            listTitleTextView.Text = listTitle;
            return convertView;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}
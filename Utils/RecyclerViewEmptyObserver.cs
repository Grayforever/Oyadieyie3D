using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Oyadieyie3D.Utils
{
    public class RecyclerViewEmptyObserver : RecyclerView.AdapterDataObserver
    {
        private RecyclerView _rv;
        private View _view;

        public RecyclerViewEmptyObserver(RecyclerView rv, View view)
        {
            _rv = rv;
            _view = view;
            CheckIfEmpty();
        }

        private void CheckIfEmpty()
        {
            if(_view != null && _rv.GetAdapter() != null)
            {
                bool emptyViewVisible = _rv.GetAdapter().ItemCount == 0;
                _view.Visibility = emptyViewVisible ? ViewStates.Visible : ViewStates.Gone;
                _rv.Visibility = emptyViewVisible ? ViewStates.Gone : ViewStates.Visible;
            }
        }

        public override void OnChanged() => CheckIfEmpty();

        public override void OnItemRangeInserted(int positionStart, int itemCount) => CheckIfEmpty();

        public override void OnItemRangeRemoved(int positionStart, int itemCount) => CheckIfEmpty();
    }
}
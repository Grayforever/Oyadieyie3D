using Android.Views;
using System;

namespace Oyadieyie3D.Callbacks
{
    internal sealed class ActionModeCallback : Java.Lang.Object, ActionMode.ICallback
    {
        private Action<ActionMode, IMenuItem> _onActionItemClicked;
        private Action<ActionMode, IMenu> _onCreateActionMode;
        private Action<ActionMode> _onDestroyActionMode;
        public ActionModeCallback(Action<ActionMode, IMenuItem> onActionItemClicked, Action<ActionMode, IMenu> onCreateActionMode, 
            Action<ActionMode> onDestroyActionMode)
        {
            _onActionItemClicked = onActionItemClicked;
            _onCreateActionMode = onCreateActionMode;
            _onDestroyActionMode = onDestroyActionMode;
        }

        public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
        {
            _onActionItemClicked?.Invoke(mode, item);
            return OnActionItemClicked(mode, item);
        }

        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            _onCreateActionMode?.Invoke(mode, menu);
            return true;
        }

        public void OnDestroyActionMode(ActionMode mode)
        {
            _onDestroyActionMode?.Invoke(mode);
        }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
        {
            return false;
        }
    }
}
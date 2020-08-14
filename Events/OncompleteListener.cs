using Android.Gms.Tasks;
using System;

namespace Oyadieyie3D.Events
{
    public class OncompleteListener : Java.Lang.Object, IOnCompleteListener
    {
        private Action<Task> _onComplete;
        public OncompleteListener(Action<Task> onComplete)
        {
            _onComplete = onComplete;
        }
        public void OnComplete(Task task)
        {
            _onComplete?.Invoke(task);
        }
    }
}
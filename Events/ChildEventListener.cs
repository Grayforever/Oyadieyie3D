using Firebase.Database;
using System;

namespace Oyadieyie3D.Events
{
    internal sealed class ChildEventListener : Java.Lang.Object, IChildEventListener
    {
        private Action<DatabaseError> onCancelled;
        private Action<DataSnapshot, string> onChildAdded;
        private Action<DataSnapshot, string> onChildChanged;
        private Action<DataSnapshot, string> onChildMoved;
        private Action<DataSnapshot> onChildRemoved;
        public ChildEventListener(Action<DatabaseError> onCancelled, Action<DataSnapshot, string> onChildAdded, 
            Action<DataSnapshot, string> onChildChanged, Action<DataSnapshot, string> onChildMoved, Action<DataSnapshot> onChildRemoved)
        {
            this.onCancelled = onCancelled;
            this.onChildAdded = onChildAdded;
            this.onChildChanged = onChildChanged;
            this.onChildMoved = onChildMoved;
            this.onChildRemoved = onChildRemoved;
        }
        public void OnCancelled(DatabaseError error)
        {
            onCancelled?.Invoke(error);
        }

        public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
        {
            onChildAdded?.Invoke(snapshot, previousChildName);
        }

        public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
        {
            onChildChanged?.Invoke(snapshot, previousChildName);
        }

        public void OnChildMoved(DataSnapshot snapshot, string previousChildName)
        {
            onChildMoved?.Invoke(snapshot, previousChildName);
        }

        public void OnChildRemoved(DataSnapshot snapshot)
        {
            onChildRemoved?.Invoke(snapshot);
        }
    }
}
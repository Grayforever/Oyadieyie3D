using Firebase.Database;
using System;

namespace Oyadieyie3D.Events
{
    public sealed class SingleValueListener : Java.Lang.Object, IValueEventListener
    {
        private readonly Action<DataSnapshot> _onDataChange;
        private readonly Action<DatabaseError> _onCancelled;
        public SingleValueListener(Action<DataSnapshot> onDataChange, Action<DatabaseError> onCancelled)
        {
            _onDataChange = onDataChange;
            _onCancelled = onCancelled;
        }
        public void OnCancelled(DatabaseError error)
        {
            _onCancelled?.Invoke(error);
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            _onDataChange?.Invoke(snapshot);
        }
    }
}
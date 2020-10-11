using CN.Pedant.SweetAlert;
using System;

namespace Oyadieyie3D.Events
{
    internal sealed class SweetConfirmClick : Java.Lang.Object, SweetAlertDialog.IOnSweetClickListener
    {
        private Action<SweetAlertDialog> _onClick;
        public SweetConfirmClick(Action<SweetAlertDialog> onClick)
        {
            _onClick = onClick;
        }
        public void OnClick(SweetAlertDialog p0)
        {
            _onClick?.Invoke(p0);
        }
    }
}
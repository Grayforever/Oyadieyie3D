using Karumi.DexterLib;
using Karumi.DexterLib.Listeners;
using Karumi.DexterLib.Listeners.Multi;
using System;
using System.Collections.Generic;

namespace Oyadieyie3D.Events
{
    public sealed class MultiplePermissionsListener : Java.Lang.Object, IMultiplePermissionsListener
    {
        private Action<IList<PermissionRequest>, IPermissionToken> _onPermissionRationaleShouldBeShown;
        private Action<MultiplePermissionsReport> _onPermissionsChecked;
        public MultiplePermissionsListener(Action<IList<PermissionRequest>, IPermissionToken> onPermissionRationaleShouldBeShown, Action<MultiplePermissionsReport> onPermissionsChecked)
        {
            _onPermissionRationaleShouldBeShown = onPermissionRationaleShouldBeShown;
            _onPermissionsChecked = onPermissionsChecked;
        }
        public void OnPermissionRationaleShouldBeShown(IList<PermissionRequest> permissions, IPermissionToken token)
        {
            _onPermissionRationaleShouldBeShown?.Invoke(permissions, token);
        }

        public void OnPermissionsChecked(MultiplePermissionsReport report)
        {
            _onPermissionsChecked?.Invoke(report);
        }
    }
}
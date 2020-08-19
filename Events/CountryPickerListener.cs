using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Mukesh.CountryPickerLib;
using Com.Mukesh.CountryPickerLib.Listeners;

namespace Oyadieyie3D.Events
{
    internal sealed class CountryPickerListener : Java.Lang.Object, IOnCountryPickerListener
    {
        private Action<Country> _onSelectCountry;
        public CountryPickerListener(Action<Country> onSelectCountry)
        {
            _onSelectCountry = onSelectCountry;
        }
        public void OnSelectCountry(Country country)
        {
            _onSelectCountry?.Invoke(country);
        }
    }
}
using Android.Icu.Util;
using Android.OS;
using Android.Widget;
using AndroidX.Fragment.App;
using Google.Android.Material.DatePicker;
using Java.Util;
using System;
using Calendar = Android.Icu.Util.Calendar;
using CalendarField = Android.Icu.Util.CalendarField;

namespace Oyadieyie3D.Fragments
{
    public class DatePickerDialog : DialogFragment, MaterialStyledDatePickerDialog.IOnDateSetListener
    {
        public event EventHandler<DateSetEventArgs> OnDatePicked;
        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var c = Calendar.Instance;
            int[]date = { c.Get(CalendarField.Year), c.Get(CalendarField.Month), c.Get(CalendarField.DayOfMonth) };

            var datePickerDialog = new MaterialStyledDatePickerDialog(Activity, this, date[0], date[1], date[2]);
            datePickerDialog.DatePicker.MaxDate = new Date().Time;
            return datePickerDialog;
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth) => 
            OnDatePicked?.Invoke(this, new DateSetEventArgs { View = view, Year = year, Month = month + 1, DayOfMonth = dayOfMonth });

        public class DateSetEventArgs : EventArgs
        {
            public DatePicker View { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public int DayOfMonth { get; set; }
        }
    }
}
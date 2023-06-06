using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BucketList
{
    public static class CalendarExtensions
    {
        public static DatePickerDialog GetDatePickerDialog
            (
            this Context context,
            string dialogName,
            EventHandler<DatePickerDialog.DateSetEventArgs> onDateSet,
            EventHandler<CalendarView.DateChangeEventArgs> onDateChange,
            Action<CalendarView> calendarSetup = null
            )
        {
            var currentDate = DateTime.Now;

            LayoutInflater inflater = LayoutInflater.From(context);
            View datePickerView = inflater.Inflate(Resource.Layout.calendar, null);
            CalendarView calendarView = datePickerView.FindViewById<CalendarView>(Resource.Id.addGoalCalendar);
            DatePickerDialog datePickerDialog = new DatePickerDialog(
                                                context,
                                                onDateSet,
                                                currentDate.Year,
                                                currentDate.Month - 1,
                                                currentDate.Day
                                                );
            var dialogTextView = datePickerView.FindViewById<TextView>(Resource.Id.addGoalTopText);
            dialogTextView.Text = dialogName;
            datePickerDialog.SetView(datePickerView);
            calendarView.MinDate = DateTime.Now.GetDateTimeInMillis();
            calendarView.FirstDayOfWeek = 2;
            calendarView.DateChange += onDateChange;
            calendarSetup?.Invoke(calendarView);
            return datePickerDialog;
        }
    }
}
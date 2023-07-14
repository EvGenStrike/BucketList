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
    public class DateInPythonCalendar : Java.Lang.Object
    {
        public DateTime Deadline { get; set; } 
        public TextView View { get; set; }
        public IGoal Goal { get; set; }

        public DateInPythonCalendar(IGoal goal, DateTime deadline, TextView view)
        {
            Deadline = deadline;
            Goal = goal;
            View = view;
        }

        public DateInPythonCalendar(DateTime deadline, TextView view)
        {
            Deadline = deadline;
            View = view;
        }


    }
}
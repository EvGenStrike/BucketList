﻿using Android.App;
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
    public class DatePythonCalendar : Java.Lang.Object
    {
        public DateTime Deadline { get; set; } 
        public View View { get; set; }
        public Goal Goal { get; set; }
        public event EventHandler Click;
        public DatePythonCalendar(Goal goal, DateTime deadline, View view)
        {
            Deadline = deadline;
            Goal = goal;
            View = view;
        }

        public DatePythonCalendar(DateTime deadline, View view)
        {
            Deadline = deadline;
            View = view;
        }


    }
}
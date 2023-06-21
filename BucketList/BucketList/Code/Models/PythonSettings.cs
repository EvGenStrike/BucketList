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
    public class PythonSettings
    {
        public Color PythonColor { get; set; }
        public DeadlineMark PythondDeadlineMark { get; set; }

        public PythonSettings() { }
        
        public PythonSettings(Color pythonColor, DeadlineMark pythondDeadlineMark) 
        {
            PythonColor = pythonColor;
            PythondDeadlineMark = pythondDeadlineMark;
        }
    }
}
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
    public class Goal
    {
        public string GoalName { get; set; }
        public List<Goal> Subgoals { get; set; }
    }
}
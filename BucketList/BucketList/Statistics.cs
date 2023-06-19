using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace BucketList
{
    public class Statistics
    {
        public Dictionary<long, Goal> PreviouslyFailedGoals { get; set; }
        public int GoalsCreatedCount { get; set; }
        public int GoalsDoneCount { get; set; }
        public int GoalsFailedCount { get => PreviouslyFailedGoals.Count; set { } }
        public int GoalsDeletedCount { get; set; }

        public Statistics()
        {
            PreviouslyFailedGoals = new Dictionary<long, Goal>();
        }
    }
}
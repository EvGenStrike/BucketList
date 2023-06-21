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
    public class Subgoal : IGoal
    {
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public GoalType GoalType { get; set; } = GoalType.Future;
        public ImageView CircleState { get; set; }
        public Subgoal() { }

        public Subgoal(string subgoalName, DateTime deadline)
        {
            Name = subgoalName;
            Deadline = deadline;
        }
    }
}
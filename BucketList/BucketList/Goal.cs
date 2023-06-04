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
        public DateTime Deadline { get; set; }
        public List<Subgoal> Subgoals { get; set; }

        public Goal() { }

        public Goal(string subgoalName, DateTime deadline)
        {
            GoalName = subgoalName;
            Deadline = deadline;
            Subgoals = new List<Subgoal>();
        }

        public void AddSubgoal(Subgoal subgoal)
        {
            Subgoals.Add(subgoal);
        }

        public void RemoveSubgoal(Subgoal subgoal)
        {
            Subgoals.Remove(subgoal);
        }
    }
}
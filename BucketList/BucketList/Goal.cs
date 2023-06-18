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
    public class Goal : IGoal
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public DateTime Deadline { get; set; }
        public List<Subgoal> Subgoals { get; set; }
        public GoalType GoalType { get; set; }

        public Goal()
        {
            GoalType = GoalType.Future; 
        }

        public Goal(string subgoalName, DateTime deadline, string image = null)
        {
            Name = subgoalName;
            Deadline = deadline;
            Subgoals = new List<Subgoal>();
            ImagePath = image;
            GoalType = GoalType.Future;
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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BucketList
{
    [Activity(Label = "GoalActivity")]
    public class GoalActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_goal_screen);
            SetGoalText();
            SetSubGoals();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        private void SetGoalText()
        {
            var goalTextView = FindViewById<TextView>(Resource.Id.goal_name_text);
            var goalName = Intent.GetStringExtra("goalName");
            goalTextView.Text = goalName;
        }

        private void SetSubGoals()
        {
            var listView = FindViewById<ListView>(Resource.Id.subgoals_items);
            var subgoals = new List<Subgoal>
            {
                new Subgoal{SubgoalName = "миша щас три часа ночи я устал", Id = 0},
                new Subgoal{SubgoalName = "ъъъъъъъъъъъъъъъ", Id = 1},
            };
            var adapter = new SubgoalAdapter(this, subgoals);
            listView.Adapter = adapter;
        }
    }
}
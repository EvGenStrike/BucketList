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
using static AndroidX.RecyclerView.Widget.RecyclerView;

namespace BucketList
{
    [Activity(Label = "GoalActivity")]
    public class GoalActivity : Activity
    {
        public List<Subgoal> Subgoals;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_goal_screen);
            SetGoalText();
            SetSubGoals();
            SetListView();
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
            Subgoals = new List<Subgoal>
            {
                new Subgoal{SubgoalName = "Купить книгу", Id = 0},
                new Subgoal{SubgoalName = "Прочитать первую главу", Id = 1},
            };
        }

        private void SetListView()
        {
            var listView = FindViewById<ListView>(Resource.Id.subgoals_items);
            var adapter = new SubgoalAdapter(this, Subgoals);
            adapter.ItemClick += Adapter_ItemClick;
            listView.Adapter = adapter;
        }

        private void Adapter_ItemClick(object sender, int e)
        {
            Toast.MakeText(this, "asdfasf", ToastLength.Long).Show();
        }
    }
}
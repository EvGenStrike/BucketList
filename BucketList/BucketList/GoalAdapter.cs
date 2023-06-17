using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.FloatingActionButton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BucketList
{
    public class GoalAdapter : BaseAdapter<Goal>
    {

        private ListView listView;
        private List<Goal> goals;
        private Activity activity;
        public GoalAdapter
            (
            Activity activity,
            List<Goal> goals,
            ListView listView
            )
        {
            this.activity = activity;
            this.goals = goals;
            this.listView = listView;
        }

        public override int Count
        {
            get
            {
                return goals.Count;
            }
        }

        public override Goal this[int position] => goals[position];

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.all_goals_list_item, parent, false);

            var rectangle = view.FindViewById<ImageView>(Resource.Id.rect);
            var subgoalName = view.FindViewById<TextView>(Resource.Id.rectangle_1);
            subgoalName.Text = goals[position].GoalName;

            view.Touch += (sender, e) =>
            {
                //ItemClick?.Invoke(this, position);
                if (e.Event.Action == MotionEventActions.Down)
                {
                    // Измените внешний вид элемента при нажатии
                    view.Alpha = 0.5f;
                }
                else if (e.Event.Action == MotionEventActions.Up || e.Event.Action == MotionEventActions.Cancel)
                {
                    // Измените внешний вид элемента при отжатии или отмене нажатия
                    view.Alpha = 1f;

                    listView.PerformItemClick(view, position, listView.Adapter.GetItemId(position));
                }
            };
            view.LongClick += (sender, e) =>
            {
                    listView.PerformItemClick(view, position, listView.Adapter.GetItemId(position));
            };

            return view;
        }
    }
}
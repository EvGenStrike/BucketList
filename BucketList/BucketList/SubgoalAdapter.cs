using System;
using Android.Widget;
using System.Collections.Generic;
using Android.Views;
using Android.Util;
using System.Linq;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android;
using Google.Android.Material.FloatingActionButton;
using Android.App;
using static Android.Icu.Text.Transliterator;
using static Android.Widget.AdapterView;
using Android.OS;
using Java.Interop;

namespace BucketList
{
    public class SubgoalViewHolder : Java.Lang.Object
    {
        public ImageView subgoalCircleState { get; set; }
        public TextView subgoalName { get; set; }
        public FloatingActionButton subgoalCalendarButton { get; set; }
    }

    public class SubgoalAdapter : BaseAdapter<Subgoal>
    {
        public EventHandler calendarFabClick { get; set; }

        private ListView listView;
        private List<Subgoal> subgoals;
        private Activity activity;

        private static int id;
        public SubgoalAdapter
            (
            Activity activity,
            List<Subgoal> subgoals,
            ListView listView
            )
        {
            this.activity = activity;
            this.subgoals = subgoals;
            this.listView = listView;
        }

        public override int Count
        {
            get
            {
                return subgoals.Count;
            }
        }

        public override Subgoal this[int position] => subgoals[position];

        //public override Java.Lang.Object GetItem(int position)
        //{
        //    return subgoals[position];
        //}

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.subgoal_list_item, parent, false);

            var subgoalCircleState = view.FindViewById<ImageView>(Resource.Id.subgoal_circle_state);
            var subgoalName = view.FindViewById<TextView>(Resource.Id.subgoal_name);
            var subgoalCalendarButton = view.FindViewById<FloatingActionButton>(Resource.Id.subgoal_calendar_button);
            subgoalCalendarButton.Tag = GoalExtensions.SerializeSubgoal(subgoals[position]);
            subgoalName.Text = subgoals[position].SubgoalName;

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

            subgoalCalendarButton.Click += (sender, e) =>
            {
                calendarFabClick?.Invoke(sender, e);
            };

            return view;
        }

    }
}

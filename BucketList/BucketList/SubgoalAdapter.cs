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

namespace BucketList
{
    public class ViewHolder : Java.Lang.Object
    {
        public ImageView subgoalCircleState { get; set; }
        public TextView subgoalName { get; set; }
        public FloatingActionButton subgoalCalendarButton { get; set; }
    }

    public class SubgoalAdapter : BaseAdapter
    {
        private List<Subgoal> subgoals;

        private Activity activity;

        public SubgoalAdapter(Activity activity, List<Subgoal> subgoals)
        {
            this.activity = activity;
            this.subgoals = subgoals;
        }

        public override int Count
        {
            get
            {
                return subgoals.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return subgoals[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.subgoal_list_item, parent, false);

            var subgoalCircleState = view.FindViewById<ImageView>(Resource.Id.subgoal_circle_state);
            var subgoalName = view.FindViewById<TextView>(Resource.Id.subgoal_name);
            var subgoalCalendarButton = view.FindViewById<FloatingActionButton>(Resource.Id.subgoal_calendar_button);

            subgoalName.Text = subgoals[position].SubgoalName;

            return view;
        }
    }
}

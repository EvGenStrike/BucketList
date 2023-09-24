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
using static Android.Content.ClipData;
using static Android.Resource;
using Android.Views.Animations;

namespace BucketList
{
    public class SubgoalViewHolder : Java.Lang.Object
    {
        public ImageView subgoalCircleState { get; set; }
        public TextView subgoalName { get; set; }
        public FloatingActionButton subgoalCalendarButton { get; set; }
    }

    public class SubgoalAdapter : BaseAdapter<Subgoal>, IOnItemClickListener, IOnItemLongClickListener
    {
        public EventHandler calendarFabClick { get; set; }
        public event EventHandler<int> ItemLongClick;
        private ListView listView;
        private List<Subgoal> subgoals;
        private Activity activity;
        private long touchStartTime;
        private bool isLongClickPerformed;
        public ImageView subgoalCircleState { get; set; }

        private Handler longClickHandler;

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
        //    return goals[position];
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
            subgoalCalendarButton.Tag = SaveExtensions.SerializeSubgoal(subgoals[position]);
            subgoalName.Text = subgoals[position].Name;

            if(subgoals[position].GoalType == GoalType.Future)
                subgoalCircleState.Background = activity.GetDrawable(Resource.Drawable.red_circle);
            else
                subgoalCircleState.Background = activity.GetDrawable(Resource.Drawable.green_circle);

            view.Touch += (sender, e) =>
            {
                if (e.Event.Action == MotionEventActions.Down)
                {
                    view.Alpha = 0.5f;
                    isLongClickPerformed = false;

                    longClickHandler = new Handler();
                    longClickHandler.PostDelayed(() =>
                    {
                        if (!isLongClickPerformed)
                        {
                            // Выполнение действий при долгом нажатии (LongClick)
                            isLongClickPerformed = true;
                            view.Alpha = 1f;
                            ItemLongClick?.Invoke(this, position);
                            //listView.PerformLongClick();
                        }
                    }, 1000);
                }
                else if (e.Event.Action == MotionEventActions.Up || e.Event.Action == MotionEventActions.Cancel)
                {
                    if (!isLongClickPerformed)
                    {
                        // Выполнение действий при клике (Click)
                        view.Alpha = 1f;
                        var itemId = listView.Adapter.GetItemId(position);
                        listView.PerformItemClick(view, position, itemId);
                    }

                    // Отменяем задержку для LongClick
                    longClickHandler?.RemoveCallbacksAndMessages(null);
                    longClickHandler = null;
                }
            };








            //view.Touch += (sender, e) =>
            //{
            //    if (e.Event.Action == MotionEventActions.Down)
            //    {
            //        view.Alpha = 0.5f;
            //        touchStartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            //        isLongClickPerformed = false;

            //    }
            //    else if (e.Event.Action == MotionEventActions.Up)
            //    {
            //        long touchEndTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            //        long touchDuration = touchEndTime - touchStartTime;

            //        if (touchDuration >= 500 || !isLongClickPerformed)
            //        {
            //            // Выполнение действий при долгом нажатии (LongClick)
            //            isLongClickPerformed = true;
            //            listView.PerformLongClick();
            //            view.Alpha = 1f;
            //        }
            //        else if (!isLongClickPerformed)
            //        {
            //            // Выполнение действий при клике (Click)
            //            view.Alpha = 1f;
            //            var itemId = listView.Adapter.GetItemId(position);
            //            listView.PerformItemClick(view, position, itemId);
            //        }
            //    }
            //};







            //view.LongClick += (sender, e) =>
            //{
            //    listView.PerformLongClick();
            //};
            //view.Click += (sender, e) =>
            //{
            //    //ItemClick?.Invoke(this, position);
            //    //if (e.Event.Action == MotionEventActions.Down)
            //    //{
            //    //    // Измените внешний вид элемента при нажатии
            //    //    view.Alpha = 0.5f;
            //    //}
            //    //else if (e.Event.Action == MotionEventActions.Up || e.Event.Action == MotionEventActions.Cancel)
            //    //{
            //    // Измените внешний вид элемента при отжатии или отмене нажатия
            //    view.Alpha = 1f;
            //    var itemId = listView.Adapter.GetItemId(position);
            //    listView.PerformItemClick(view, position, itemId);
            //};
            subgoalCalendarButton.Click += (sender, e) =>
                        {
                            calendarFabClick?.Invoke(sender, e);
                        };

            return view;
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            var x = 1;
        }

        public bool OnItemLongClick(AdapterView parent, View view, int position, long id)
        {
            return true;
        }
    }
}

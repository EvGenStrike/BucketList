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
using static Android.Content.ClipData;

namespace BucketList
{
    public class GoalAdapter : BaseAdapter<Goal>, IAdapter
    {
        public event EventHandler<int> ItemLongClick;
        private ListView listView;
        private List<Goal> goals;
        private Activity activity;

        private long touchStartTime;
        private bool isLongClickPerformed;

        private Handler longClickHandler;

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
            var snakeImage = view.FindViewById<ImageView>(Resource.Id.rect1);
            var subgoalName = view.FindViewById<TextView>(Resource.Id.rectangle_1);
            snakeImage.SetImage(goals[position].ImagePath);
            subgoalName.Text = goals[position].Name;

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

            return view;
        }
    }
}
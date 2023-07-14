using Android.App;
using Android.Content;
using Android.Widget;
using System;
using System.Linq;

namespace BucketList
{
    [Activity(Label = "AddSubgoalActivity")]
    public class AddSubgoalActivity : AddGoalActivity
    {
        private Goal currentGoal;

        public override void InitializeAdditionalData()
        {
            currentGoal = SaveExtensions.DeserializeGoal(Intent.GetStringExtra("currentGoal"));
        }

        public override bool IsValidGoal(string goalName, DateTime goalDeadline)
        {
            if (currentGoal.Subgoals.Any(x => x.Name == goalName))
            {
                Toast.MakeText(this, "Подцель с таким названием уже существует", ToastLength.Short).Show();
                return false;
            }
            if (!GoalExtensions.IsValidDeadline(goalDeadline))
            {
                Toast.MakeText(this, "Вы не выбрали дедлайн", ToastLength.Short).Show();
                return false;
            }
            return true;
        }

        public override void CalendarSetup(CalendarView calendarView)
        {
            var selectedDate = GetGoalDateTime();
            calendarView.MaxDate = currentGoal.Deadline.GetDateTimeInMillis();
            if (GoalExtensions.IsValidDeadline(selectedDate))
            {
                calendarView.Date = selectedDate.GetDateTimeInMillis();
            }
        }
    }
}

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
using Android.Graphics;

namespace BucketList
{
    [Activity(Label = "CalendarActivity")]
    public class CalendarActivity : Activity
    {
        private List<DatePythonCalendar> datesPythonCalendar = new List<DatePythonCalendar>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_calendar);

            var goalsJson = Intent.GetStringExtra("goals");
            var goals = Extensions.DeserializeGoals(goalsJson);

            var currentDateTime = DateTime.Now;
            var firstDayOfMonth = new DateTime(currentDateTime.Year, currentDateTime.Month, 1);
            var dayOfWeek = firstDayOfMonth.DayOfWeek;
            var calendar = FindViewById<GridLayout>(Resource.Id.calendar);
            var indexFirstDayOfMonth = GetFirstDayIndexInCalendar(dayOfWeek, calendar);
            AddDatesToCalendar(goals, ref firstDayOfMonth, calendar, indexFirstDayOfMonth);
        }

        private void AddDatesToCalendar(List<Goal> goals, ref DateTime firstDayOfMonth, GridLayout calendar, int indexFirstDayOfMonth)
        {
            // Первые 7 элементов - дни недели, пропускаем их
            var indexFirstDateCell = 7;
            for (var indexCurrentDay = indexFirstDateCell; indexCurrentDay < calendar.ChildCount; indexCurrentDay++)
            {
                // Дата - это TextView в Layout
                var dateView = calendar.GetChildAt(indexCurrentDay) as TextView;
                if (dateView == null) return;

                var date = new DatePythonCalendar(firstDayOfMonth, dateView);
                dateView.Text = firstDayOfMonth.Day.ToString();
                datesPythonCalendar.Add(date);

                foreach (var goal in goals)
                {
                    SetDeadlineDate(goal, date);
                }

                // Изменяем текущий день на следующий
                firstDayOfMonth = firstDayOfMonth.AddDays(1);
            }
        }

        private int GetFirstDayIndexInCalendar(DayOfWeek dayOfWeek, GridLayout calendar)
        {
            for (var i = 0; i < 7; i++)
            {
                var dateView = calendar.GetChildAt(i) as TextView;
                if (dateView.Tag.ToString() == dayOfWeek.ToString())
                {
                    return i + 7;
                }
            }
            return 0;
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        private void SetDeadlineDate(Goal goal, DatePythonCalendar date)
        {
            if (date.Deadline.Date == goal.Deadline.Date)
            {
                date.Goal = goal;
                date.View.Tag = date;
                date.View.Click += OnDeadlineDate_Click;
                date.View.Background = GetDrawable(Resource.Drawable.deadlineMouse1);
            }
        }
        public void OnDeadlineDate_Click(object sender, EventArgs e)
        {
            var textView = sender as TextView;

            var goal = textView.Tag.JavaCast<DatePythonCalendar>().Goal;
            CreateDateDialog(goal);
        }
        private void CreateDateDialog(Goal goal)
        {
            var builder = new AlertDialog.Builder(this);
            var dialogView = LayoutInflater.Inflate(Resource.Layout.dialog_date, null);
            builder.SetView(dialogView);

            var buttonDismiss = dialogView.FindViewById<Button>(Resource.Id.buttonDismissDialog);
            var goalName = dialogView.FindViewById<TextView>(Resource.Id.goalName);
            var goalDeadline = dialogView.FindViewById<TextView>(Resource.Id.goalDeadline);
            var goalImage = dialogView.FindViewById<ImageView>(Resource.Id.goalImage);

            // Устанавливаем значения для вьюшек
            if (goal.ImagePath != null)
                goalImage.SetImage(goal.ImagePath);
            goalName.Text = goal.GoalName;
            goalDeadline.Text = goal.Deadline.Date.ToString();

            var dialog = builder.Create();
            // Зыкрытие по нажатию кнопки
            buttonDismiss.Click += (sender, e) => { dialog.Dismiss(); };
            dialog.Show();
        }
    }


}
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
using static Android.InputMethodServices.Keyboard;
using System.Globalization;
using Google.Android.Material.FloatingActionButton;

namespace BucketList
{
    [Activity(Label = "CalendarActivity")]
    public class CalendarActivity : Activity
    {
        private List<DatePythonCalendar> datesPythonCalendar = new List<DatePythonCalendar>();
        public List<TextView> textViewsWithClickEventSubscribe = new List<TextView>();
        private List<Goal> goals = new List<Goal>();
        private GridLayout calendar;
        private DateTime currentDateTimeInCalendar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_calendar);

            var goalsJson = Intent.GetStringExtra("goals");
            goals = Extensions.DeserializeGoals(goalsJson);

            var nextButton = FindViewById<FloatingActionButton>(Resource.Id.nextMonthButton);
            var previousButton = FindViewById<FloatingActionButton>(Resource.Id.previousMonthButton);
            nextButton.Click += NextButton_Click;
            previousButton.Click += PreviousButton_Click;

            var currentDateTime = DateTime.Now;
            SetMonthAndYear(currentDateTime);
            // Первый день текущего месяца
            currentDateTimeInCalendar = new DateTime(currentDateTime.Year, currentDateTime.Month, 1);
            var dayOfWeek = currentDateTimeInCalendar.DayOfWeek;
            calendar = FindViewById<GridLayout>(Resource.Id.calendar);
            // Получаем индекс первого дня текущего месяца в календаре
            var indexFirstDayOfMonth = GetFirstDayIndexInCalendar(dayOfWeek, calendar);
            // Индекс первой клетки с датой в календаре
            var indexFirstDateCell = 7;
            // Первый день в календаре
            var firstDayInCalendar = currentDateTimeInCalendar.AddDays(-(indexFirstDayOfMonth - indexFirstDateCell));
            AddDatesToCalendar(goals, ref firstDayInCalendar, calendar, indexFirstDateCell, currentDateTime);
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            ClearPreviousValuesFromCalendar();

            var currentDateTime = DateTime.Now;
            SetMonthAndYear(currentDateTimeInCalendar.AddMonths(-1));
            // Устанавливаем новую текущую дату
            if (currentDateTimeInCalendar.Month == 1)
                currentDateTimeInCalendar = new DateTime(currentDateTimeInCalendar.Year - 1,
                    12, 1);
            else
                currentDateTimeInCalendar = new DateTime(currentDateTimeInCalendar.Year,
                    currentDateTimeInCalendar.Month - 1, 1);
            var dayOfWeek = currentDateTimeInCalendar.DayOfWeek;
            // Получаем индекс первого дня текущего месяца в календаре
            var indexFirstDayOfMonth = GetFirstDayIndexInCalendar(dayOfWeek, calendar);
            // Индекс первой клетки с датой в календаре
            var indexFirstDateCell = 7;
            // Первый день в календаре
            var firstDayInCalendar = currentDateTimeInCalendar.AddDays(-(indexFirstDayOfMonth - indexFirstDateCell));
            AddDatesToCalendar(goals, ref firstDayInCalendar, calendar, indexFirstDateCell, currentDateTime);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            ClearPreviousValuesFromCalendar();

            var currentDateTime = DateTime.Now;
            SetMonthAndYear(currentDateTimeInCalendar.AddMonths(1));
            // Устанавливаем новую текущую дату
            if (currentDateTimeInCalendar.Month == 12)
                currentDateTimeInCalendar = new DateTime(currentDateTimeInCalendar.Year + 1,
                    1, 1);
            else
                currentDateTimeInCalendar = new DateTime(currentDateTimeInCalendar.Year,
                    currentDateTimeInCalendar.Month + 1, 1);
            var dayOfWeek = currentDateTimeInCalendar.DayOfWeek;
            // Получаем индекс первого дня текущего месяца в календаре
            var indexFirstDayOfMonth = GetFirstDayIndexInCalendar(dayOfWeek, calendar);
            // Индекс первой клетки с датой в календаре
            var indexFirstDateCell = 7;
            // Первый день в календаре
            var firstDayInCalendar = currentDateTimeInCalendar.AddDays(-(indexFirstDayOfMonth - indexFirstDateCell));
            AddDatesToCalendar(goals, ref firstDayInCalendar, calendar, indexFirstDateCell, currentDateTime);
        }

        private void ClearPreviousValuesFromCalendar()
        {
            // Восстанавливаем изначальное состояние TextView
            foreach (var date in datesPythonCalendar)
            {
                date.View.Background = GetDrawable(Resource.Drawable.dateInCalendarWithPython);
                date.View.SetTextColor(Color.Black);
            }

            foreach (var textView in textViewsWithClickEventSubscribe)
                textView.Click -= OnDeadlineDate_Click;

            datesPythonCalendar.Clear();
        }

        private void SetMonthAndYear(DateTime currentDateTime)
        {
            var culture = new CultureInfo("ru-RU"); // Устанавливаем русскую культуру
            var month = culture.DateTimeFormat.GetMonthName(currentDateTime.Month);
            var monthName = FindViewById<TextView>(Resource.Id.monthName);
            // Делаем первую букву заглавной
            monthName.Text = char.ToUpper(month[0]) + month.Substring(1) + "\n" + currentDateTime.Year.ToString();
        }

        private void AddDatesToCalendar(List<Goal> goals,
            ref DateTime firstDayInCalendar,
            GridLayout calendar,
            int indexFirstDateCell,
            DateTime currentDateTime)
        {
            // Первые 7 элементов - дни недели, пропускаем их
            for (var indexCurrentDay = indexFirstDateCell; indexCurrentDay < calendar.ChildCount; indexCurrentDay++)
            {
                // Дата - это TextView в Layout
                var dateView = calendar.GetChildAt(indexCurrentDay) as TextView;
                if (dateView == null) return;

                var date = new DatePythonCalendar(firstDayInCalendar, dateView);
                dateView.Text = firstDayInCalendar.Day.ToString();
                datesPythonCalendar.Add(date);

                foreach (var goal in goals)
                {
                    SetDate(goal, date, currentDateTime);
                }

                // Изменяем текущий день на следующий
                firstDayInCalendar = firstDayInCalendar.AddDays(1);
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

        private void SetDate(Goal goal, DatePythonCalendar date, DateTime currentDateTime)
        {
            // Рисуем мышку
            if (date.Deadline.Date == goal.Deadline.Date)
            {
                date.Goal = goal;
                date.View.Tag = date;
                date.View.Click += OnDeadlineDate_Click;
                textViewsWithClickEventSubscribe.Add(date.View);
                date.View.Background = GetDrawable(Resource.Drawable.deadlineMouse1);
            }

            // Рисуем питона
            if (date.Deadline.Date < currentDateTime.Date)
            {
                date.View.SetTextColor(Color.LightGray);
                date.View.Background = GetDrawable(Resource.Drawable.pythonBody);
            }
            else if (date.Deadline.Date == currentDateTime.Date)
            {
                date.View.Background = GetDrawable(Resource.Drawable.pythonHead);
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
            goalDeadline.Text = goal.Deadline.Date.ToShortDateString();

            var dialog = builder.Create();
            // Зыкрытие по нажатию кнопки
            buttonDismiss.Click += (sender, e) =>
            {
                dialog.Dismiss(); 
            };

            dialog.Show();
        }
    }
}
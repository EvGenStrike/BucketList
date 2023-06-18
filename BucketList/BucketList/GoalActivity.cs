using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Util;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Json.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Android.Content.PM;
using Google.Android.Material.FloatingActionButton;

namespace BucketList
{
    [Activity(Label = "GoalActivity")]
    public class GoalActivity : Activity
    {
        private const int ReadExternalStoragePermissionRequestCode = 100;

        public List<Goal> Goals { get; private set; }
        public Goal CurrentGoal { get; private set; }

        private Subgoal currentSubgoal;
        User user;

        private DateTime selectedDate = DateTime.MinValue;
        private int itemIdThatWantsToChangeDate = -1; //-1 значит item = CurrentGoal, 0..n значит item = CurrentGoal.Subgoals[i];

        private RelativeLayout calendar;
        private Button buttonCalendarOpen;
        public List<DatePythonCalendar> datesPythonCalendar = new List<DatePythonCalendar>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_goal_screen);
            SetGoals();
            SetCalendarValues();
            Initialize();
            SetGoalText();
            SetGoalImage();
            SetListView();
            SetFabAddSubgoal();
            SetGoalCalendarFab();
            SetDoGoalButton();
            SetPythonCalendarView();
        }

        private void SetCalendarValues()
        {
            calendar = FindViewById<RelativeLayout>(Resource.Id.deadlineCalendarGoalScreen);
            buttonCalendarOpen = FindViewById<Button>(Resource.Id.calendarButtonGoalScreen);
            buttonCalendarOpen.Click += ButtonCalendarOpen_Click;
        }

        private void ButtonCalendarOpen_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(CalendarActivity));
            intent.PutExtra("goals", Extensions.SerializeSubgoals(CurrentGoal.Subgoals));
            StartActivity(intent);
        }

        private void SetDoGoalButton()
        {
            var button = FindViewById<Button>(Resource.Id.goal_screen_do_goal_button);
            if (CurrentGoal.GoalType == GoalType.Done)
            {
                button.Text = "Цель уже выполнена";
                return;
            }
            if (CurrentGoal.GoalType == GoalType.Failed)
            {
                button.Text = "Цель просрочена";

                button.BackgroundTintList = GetColorStateList(Resource.Color.colorGray);
                button.SetTextColor(Android.Graphics.Color.LightGray);
                return;
            }
            button.Click += DoGoalButton_Click;
        }

        private void DoGoalButton_Click(object sender, EventArgs e)
        {
            CurrentGoal.GoalType = GoalType.Done;
            Extensions.OverwriteGoals(Goals);
            user = Extensions.GetSavedUser();
            user.UserStatistics.GoalsDoneCount++;
            Extensions.OverwriteUser(user);
            OnBackPressed();
        }

        private void SetGoalImage()
        {
            var goalImage = FindViewById<ImageView>(Resource.Id.goal_screen_image);
            goalImage.SetImage(CurrentGoal.ImagePath);
        }

        private void SetGoalCalendarFab()
        {
            var goalCalendarFab = FindViewById<FloatingActionButton>(Resource.Id.goal_screen_calendar_button);
            goalCalendarFab.Click += GoalCalendarFab_Click;
        }

        private void GoalCalendarFab_Click(object sender, EventArgs e)
        {
            var datePickerDialog = GetDatePickerDialog
                (
                    $"Изменить дедлайн цели \"{CurrentGoal.Name}\"?",
                    (calendarView) =>
                        {
                            calendarView.MinDate = CurrentGoal.Subgoals.Count > 0 ? CurrentGoal.Subgoals.Max(x => x.Deadline).GetDateTimeInMillis() : DateTime.Now.GetDateTimeInMillis();
                            calendarView.Date = CurrentGoal.Deadline.GetDateTimeInMillis();
                        }
                );
            itemIdThatWantsToChangeDate = -1;
            datePickerDialog.Show();
        }

        private void OnDateChange(object sender, CalendarView.DateChangeEventArgs args)
        {
            var selectedDate = new DateTime(args.Year, args.Month + 1, args.DayOfMonth);
            this.selectedDate = selectedDate;
        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            var year = selectedDate.Year;
            if (year == 1) return;
            var month = selectedDate.Month;
            var dayOfMonth = selectedDate.Day;
            var selectedDateInString
                = $"{string.Format("{0:00}", dayOfMonth)}.{string.Format("{0:00}", month)}.{year}";
            //Toast.MakeText(this, $"Выбранная дата: {selectedDateInString}", ToastLength.Short).Show();            
            if (itemIdThatWantsToChangeDate == -1)
            {
                CurrentGoal.Deadline = selectedDate;
                Toast.MakeText(this, $"Дедлайн цели \"{CurrentGoal.Name}\" изменён на {selectedDateInString}", ToastLength.Long).Show();
            }
            else
            {
                var subgoal = CurrentGoal.Subgoals[itemIdThatWantsToChangeDate];
                subgoal.Deadline = selectedDate;
                Toast.MakeText(this, $"Дедлайн подцели \"{subgoal.Name}\" изменён на {selectedDateInString}", ToastLength.Long).Show();
            }
        }

        public void SetGoals()
        {
            Goals = Extensions.GetSavedGoals();
        }

        public void Initialize()
        {
            var goal = JsonNet.Deserialize<Goal>(Intent.GetStringExtra("goal"));
            var desiredGoal = Goals.FirstOrDefault(x => x.Name == goal.Name);
            CurrentGoal = desiredGoal;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && data != null)
            {
                var tempGoal = JsonNet.Deserialize<Goal>(data.GetStringExtra("goal"));
                var newSubgoal = new Subgoal(tempGoal.Name, tempGoal.Deadline);
                AddSubgoal(newSubgoal);
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        private void SetGoalText()
        {
            var goalTextView = FindViewById<TextView>(Resource.Id.goal_name_text);
            goalTextView.Text = CurrentGoal.Name;
        }

        private void SetListView()
        {
            UpdateSubgoalsListView();
            var listView = FindViewById<ListView>(Resource.Id.goal_screen_subgoals_list_view);
            listView.ItemClick += ListView_ItemClick;
            listView.LongClick += ListView_LongClick;
        }

        private void ListView_LongClick(object sender, View.LongClickEventArgs e)
        {
            Toast.MakeText(this, "я в ахуе, гении", 0).Show();
        }

        private void SetFabAddSubgoal()
        {
            var fab = FindViewById<FloatingActionButton>(Resource.Id.goal_screen_add_subgoal);
            if (CurrentGoal.GoalType == GoalType.Failed)
            {
                fab.BackgroundTintList = GetColorStateList(Resource.Color.colorGray);
                return;
            }
            fab.Click += Fab_Click;
        }

        private void Fab_Click(object sender, EventArgs e)
        {
            SwitchToAddGoalScreen();
        }

        private void SwitchToAddGoalScreen()
        {
            var intent = new Intent(this, typeof(AddSubgoalActivity));
            intent.PutExtra("maxDeadline", CurrentGoal.Deadline.ToString());
            intent.PutExtra("currentGoal", Extensions.SerializeGoal(CurrentGoal));
            StartActivityForResult(intent, 1);
        }

        private void UpdateSubgoalsListView()
        {
            Extensions.OverwriteGoals(Goals);
            var listView = FindViewById<ListView>(Resource.Id.goal_screen_subgoals_list_view);
            //var adapter = new ArrayAdapter<string>(this,
            //    Resource.Layout.subgoal_list_item,
            //    Resource.Id.subgoal_name,
            //    CurrentGoal.Subgoals.Select(x => x.Name).ToList());
            var adapter = new SubgoalAdapter(this, CurrentGoal.Subgoals, listView);
            adapter.calendarFabClick += CalendarFab_Click;
            
            listView.Adapter = adapter;
            //for (var i = 0; i < listView.Adapter.Count; i++)
            //{
            //    var item = listView.Adapter.GetItem(i);
            //    var view = listView.Adapter.GetView(i, null, listView);
            //    var button = view.FindViewById<Button>(Resource.Id.subgoal_calendar_button);
            //    button.Focusable = true;
            //    button.Click += CalendarFab_Click;
            //}
        }

        private void CalendarFab_Click(object sender, EventArgs e)
        {
            var fab = sender as FloatingActionButton;
            var subgoal = Extensions.DeserializeSubgoal((string)fab.Tag);
            var datePickerDialog = GetDatePickerDialog
                (
                $"Изменить дедлайн подцели \"{subgoal.Name}\"?",
                (calendarView) =>
                    {
                        calendarView.MaxDate = CurrentGoal.Deadline.GetDateTimeInMillis();
                        calendarView.Date = subgoal.Deadline.GetDateTimeInMillis();
                    }
                );
            itemIdThatWantsToChangeDate = CurrentGoal.Subgoals.IndexOf(CurrentGoal.Subgoals.First(x => x.Name == subgoal.Name));
            datePickerDialog.Show();
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            ListView myListView = sender as ListView;

            var selectedItem = myListView.GetItemAtPosition(e.Position);
            currentSubgoal = selectedItem.Cast<Subgoal>();

            RegisterForContextMenu(myListView);

            OpenContextMenu(myListView);
        }

        private void ShowContextMenu(object sender, AdapterView.ItemClickEventArgs e)
        {
            ListView myListView = sender as ListView;

            var selectedItem = myListView.GetItemAtPosition(e.Position);
            currentSubgoal = selectedItem.Cast<Subgoal>();

            RegisterForContextMenu(myListView);

            OpenContextMenu(myListView);
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            // Установите заголовок контекстного меню
            menu.SetHeaderTitle("Удалить подцель?");

            // Добавьте пункт меню для удаления элемента
            menu.Add(Menu.None, 0, Menu.None, "Да");
            menu.Add(Menu.None, 1, Menu.None, "Нет");
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            if (item.ItemId == 0)
            {
                RemoveSubgoal(currentSubgoal);
                return true;
            }

            return base.OnContextItemSelected(item);
        }
        private void AddSubgoal(Subgoal subgoal)
        {
            CurrentGoal.AddSubgoal(subgoal);
            foreach (var date in datesPythonCalendar)
            {
                AddSubgoalUpdateCalendar(subgoal, date);
            }
            UpdateSubgoalsListView();
        }

        private void AddSubgoalUpdateCalendar(Subgoal goal, DatePythonCalendar date)
        {
            if (date.Deadline.Date == goal.Deadline.Date)
            {
                date.Goal = goal;
                date.View.Tag = date;
                date.View.Background = GetDrawable(Resource.Drawable.deadlineMouse1);
            }
        }

        private void RemoveSubgoal(Subgoal goal)
        {
            CurrentGoal.RemoveSubgoal(goal);
            foreach (var date in datesPythonCalendar)
            {
                DeleteSubgoalUpdateCalendar(goal, date);
            }
            UpdateSubgoalsListView();
        }

        private void DeleteSubgoalUpdateCalendar(Subgoal goal, DatePythonCalendar date)
        {
            if (goal.Deadline.Date == date.Deadline.Date)
            {
                date.Goal = null;
                date.View.Background = GetDrawable(Resource.Drawable.dateInCalendarWithPython);
            }
        }

        private void SetPythonCalendarView()
        {
            var currentDateTime = DateTime.Now.AddDays(-2);

            for (var i = 1; i < calendar.ChildCount; i++)
            {
                // Дата - это TextView в Layout
                var dateView = calendar.GetChildAt(i) as TextView;
                if (dateView == null) continue;

                dateView.Text = currentDateTime.Day.ToString();
                SetDatesList(currentDateTime, dateView);
                currentDateTime = currentDateTime.AddDays(1);
            }

            DrawDatesWithoutGoals();
            DrawDatesWithGoals();
        }

        private void DrawDatesWithoutGoals()
        {
            var currentDateTime = DateTime.Now;

            foreach (var date in datesPythonCalendar.Where(x => x.Goal == null))
            {
                if (date.Deadline.Date < currentDateTime.Date)
                {
                    date.View.SetTextColor(Android.Graphics.Color.LightGray);
                    date.View.Background = GetDrawable(Resource.Drawable.pythonBody);
                }
                else if (date.Deadline.Date == currentDateTime.Date)
                {
                    date.View.SetTextColor(Android.Graphics.Color.LightGray);
                    date.View.Background = GetDrawable(Resource.Drawable.pythonHead);
                }
                else
                {
                    date.View.SetTextColor(Android.Graphics.Color.Black);
                    date.View.Background = GetDrawable(Resource.Drawable.dateInCalendarWithPython);
                }
            }
        }

        private void DrawDatesWithGoals()
        {
            var currentDateTime = DateTime.Now;

            foreach (var date in datesPythonCalendar.Where(x => x.Goal != null))
            {
                if (date.Deadline.Date < currentDateTime.Date)
                    date.View.Background = GetDrawable(Resource.Drawable.deadlineMouseDeadWithPythonBody);
                else if (date.Deadline.Date == currentDateTime.Date)
                {
                    date.View.SetTextColor(Android.Graphics.Color.LightGray);
                    date.View.Background = GetDrawable(Resource.Drawable.deadlineMouseDeadWithPythonHead);
                }
                else
                    date.View.Background = GetDrawable(Resource.Drawable.deadlineMouse1);
            }
        }

        private void SetDatesList(DateTime firstDayInCalendar, TextView dateView)
        {
            var canAddDateWithoutGoal = true;

            foreach (var goal in CurrentGoal.Subgoals)
            {
                if (goal.Deadline.Date == firstDayInCalendar.Date)
                {
                    datesPythonCalendar.Add(new DatePythonCalendar(goal, goal.Deadline, dateView));
                    canAddDateWithoutGoal = false;
                    break;
                }
            }

            if (canAddDateWithoutGoal)
                datesPythonCalendar.Add(new DatePythonCalendar(firstDayInCalendar, dateView));
        }

        private DatePickerDialog GetDatePickerDialog(string dialogName, Action<CalendarView> calendarSetup = null)
        {
            var datePickerDialog = this.GetDatePickerDialog(dialogName, OnDateSet, OnDateChange, calendarSetup);
            return datePickerDialog;
        }
    }
}
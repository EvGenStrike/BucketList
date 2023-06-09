using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Util;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using Json.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace BucketList
{
    [Activity(Label = "GoalActivity")]
    public class GoalActivity : Activity
    {
        public List<Goal> Goals { get; private set; }
        public Goal CurrentGoal { get; private set; }

        private Subgoal currentSubgoal;

        private DateTime selectedDate = DateTime.MinValue;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_goal_screen);
            SetGoals();
            Initialize();
            SetGoalText();
            SetListView();
            SetFabAddSubgoal();
            SetGoalCalendarFab();
        }

        private void SetGoalCalendarFab()
        {
            var goalCalendarFab = FindViewById<FloatingActionButton>(Resource.Id.goal_screen_calendar_button);
            goalCalendarFab.Click += GoalCalendarFab_Click;
        }

        private void GoalCalendarFab_Click(object sender, EventArgs e)
        {
            var datePickerDialog = GetDatePickerDialog($"Изменить дедлайн цели \"{CurrentGoal.GoalName}\"?");
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
            var month = selectedDate.Month;
            var dayOfMonth = selectedDate.Day;
            var selectedDateInString = $"{dayOfMonth}-{month}-{year}";
            Toast.MakeText(this, $"Выбранная дата: {selectedDateInString}", ToastLength.Short).Show();            
        }

        public void SetGoals()
        {
            Goals = GoalExtensions.GetSavedGoals();
        }

        public void Initialize()
        {
            var goal = JsonNet.Deserialize<Goal>(Intent.GetStringExtra("goal"));
            var desiredGoal = Goals.FirstOrDefault(x => x.GoalName == goal.GoalName);
            CurrentGoal = desiredGoal;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && data != null)
            {
                var tempGoal = JsonNet.Deserialize<Goal>(data.GetStringExtra("goal"));
                var newSubgoal = new Subgoal(tempGoal.GoalName, tempGoal.Deadline);
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
            goalTextView.Text = CurrentGoal.GoalName;
        }

        private void SetListView()
        {
            UpdateSubgoalsListView();
            var listView = FindViewById<ListView>(Resource.Id.goal_screen_subgoals_list_view);
            listView.ItemClick += ListView_ItemClick;
        }

        private void SetFabAddSubgoal()
        {
            var fab = FindViewById<FloatingActionButton>(Resource.Id.goal_screen_add_subgoal);
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
            intent.PutExtra("currentGoal", GoalExtensions.SerializeGoal(CurrentGoal));
            StartActivityForResult(intent, 1);
        }

        private void AddSubgoal(Subgoal subgoal)
        {
            CurrentGoal.AddSubgoal(subgoal);
            UpdateSubgoalsListView();
        }

        private void UpdateSubgoalsListView()
        {
            GoalExtensions.OverwriteGoals(Goals);
            var listView = FindViewById<ListView>(Resource.Id.goal_screen_subgoals_list_view);
            var adapter = new SubgoalAdapter(this, CurrentGoal.Subgoals, listView);
            adapter.calendarFabClick += CalendarFab_Click;
            listView.Adapter = adapter;
        }

        private void CalendarFab_Click(object sender, EventArgs e)
        {
            var fab = sender as FloatingActionButton;
            var subgoal = GoalExtensions.DeserializeSubgoal((string)fab.Tag);
            var datePickerDialog = GetDatePickerDialog
                (
                $"Изменить дедлайн подцели \"{subgoal.SubgoalName}\"?",
                (calendarView) =>
                    {
                        calendarView.MaxDate = CurrentGoal.Deadline.GetDateTimeInMillis();
                    }
                );
            datePickerDialog.Show();
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            ShowContextMenu(sender, e);
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

        private void RemoveSubgoal(Subgoal goal)
        {
            CurrentGoal.RemoveSubgoal(goal);
            UpdateSubgoalsListView();
        }

        private DatePickerDialog GetDatePickerDialog(string dialogName, Action<CalendarView> calendarSetup = null)
        {
            var datePickerDialog = this.GetDatePickerDialog(dialogName, OnDateSet, OnDateChange, calendarSetup);
            return datePickerDialog;
        }
    }
}
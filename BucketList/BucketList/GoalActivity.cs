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
using Android.Content.PM;


namespace BucketList
{
    [Activity(Label = "GoalActivity")]
    public class GoalActivity : Activity
    {
        private const int ReadExternalStoragePermissionRequestCode = 100;

        public List<Goal> Goals { get; private set; }
        public Goal CurrentGoal { get; private set; }

        private Subgoal currentSubgoal;

        private DateTime selectedDate = DateTime.MinValue;
        private int itemIdThatWantsToChangeDate = -1; //-1 значит item = CurrentGoal, 0..n значит item = CurrentGoal.Subgoals[i];


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_goal_screen);
            SetGoals();
            Initialize();
            SetGoalText();
            SetGoalImage();
            SetListView();
            SetFabAddSubgoal();
            SetGoalCalendarFab();
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
                    $"Изменить дедлайн цели \"{CurrentGoal.GoalName}\"?",
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
                Toast.MakeText(this, $"Дедлайн цели \"{CurrentGoal.GoalName}\" изменён на {selectedDateInString}", ToastLength.Long).Show();
            }
            else
            {
                var subgoal = CurrentGoal.Subgoals[itemIdThatWantsToChangeDate];
                subgoal.Deadline = selectedDate;
                Toast.MakeText(this, $"Дедлайн подцели \"{subgoal.SubgoalName}\" изменён на {selectedDateInString}", ToastLength.Long).Show();
            }
        }

        public void SetGoals()
        {
            Goals = Extensions.GetSavedGoals();
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
            intent.PutExtra("currentGoal", Extensions.SerializeGoal(CurrentGoal));
            StartActivityForResult(intent, 1);
        }

        private void AddSubgoal(Subgoal subgoal)
        {
            CurrentGoal.AddSubgoal(subgoal);
            UpdateSubgoalsListView();
        }

        private void UpdateSubgoalsListView()
        {
            Extensions.OverwriteGoals(Goals);
            var listView = FindViewById<ListView>(Resource.Id.goal_screen_subgoals_list_view);
            var adapter = new SubgoalAdapter(this, CurrentGoal.Subgoals, listView);
            adapter.calendarFabClick += CalendarFab_Click;
            listView.Adapter = adapter;
        }

        private void CalendarFab_Click(object sender, EventArgs e)
        {
            var fab = sender as FloatingActionButton;
            var subgoal = Extensions.DeserializeSubgoal((string)fab.Tag);
            var datePickerDialog = GetDatePickerDialog
                (
                $"Изменить дедлайн подцели \"{subgoal.SubgoalName}\"?",
                (calendarView) =>
                    {
                        calendarView.MaxDate = CurrentGoal.Deadline.GetDateTimeInMillis();
                        calendarView.Date = subgoal.Deadline.GetDateTimeInMillis();
                    }
                );
            itemIdThatWantsToChangeDate = CurrentGoal.Subgoals.IndexOf(CurrentGoal.Subgoals.First(x => x.SubgoalName == subgoal.SubgoalName));
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
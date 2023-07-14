using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.FloatingActionButton;
using Java.Time.Temporal;
using Json.Net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BucketList
{
    [Activity(Label = "GoalActivity")]
    public class GoalActivity : Activity
    {
        private List<Goal> Goals;
        private Goal CurrentGoal;
        private User user;

        private ImageView goalImage;
        private Button doGoalButton;
        private FloatingActionButton goalCalendarFab;
        private TextView goalNameTextView;
        private ListView subgoalsListView;
        private FloatingActionButton subgoalAddFab;

        private Subgoal currentSubgoal;
        private bool isLongClick;

        private DateTime selectedDate = DateTime.MinValue;
        private IGoal GoalToChangeDate;

        private RelativeLayout calendar;
        private Button buttonCalendarOpen;
        private List<DateInPythonCalendar> datesPythonCalendar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_goal_screen);
            Initialize();
            SetCalendarValues();
            SetGoalText();
            SetGoalImage();
            SetListView();
            SetFabAddSubgoal();
            SetGoalCalendarFab();
            SetDoGoalButton();
        }


        public void Initialize()
        {
            InitializeActivityViews();
            InitializeGoals();
            InitializeUser();
            InitializeDatesPythonCalendar();
        }

        private void InitializeDatesPythonCalendar()
        {
            datesPythonCalendar = new List<DateInPythonCalendar>();
        }

        private void InitializeActivityViews()
        {
            goalImage = FindViewById<ImageView>(Resource.Id.goal_screen_image);
            calendar = FindViewById<RelativeLayout>(Resource.Id.deadlineCalendarGoalScreen);
            buttonCalendarOpen = FindViewById<Button>(Resource.Id.calendarButtonGoalScreen);
            doGoalButton = FindViewById<Button>(Resource.Id.goal_screen_do_goal_button);
            goalCalendarFab = FindViewById<FloatingActionButton>(Resource.Id.goal_screen_calendar_button);
            goalNameTextView = FindViewById<TextView>(Resource.Id.goal_name_text);
            subgoalsListView = FindViewById<ListView>(Resource.Id.goal_screen_subgoals_list_view);
            subgoalAddFab = FindViewById<FloatingActionButton>(Resource.Id.goal_screen_add_subgoal);
        }

        private void InitializeGoals()
        {
            Goals = SaveExtensions.GetSavedGoals();
            var goal = SaveExtensions.DeserializeGoal(Intent.GetStringExtra("goal"));
            CurrentGoal = Goals.FirstOrDefault(x => x.Name == goal.Name);
        }

        private void InitializeUser()
        {
            user = SaveExtensions.GetSavedUser();
        }

        private void SetCalendarValues()
        {            
            buttonCalendarOpen.Click += ButtonCalendarOpen_Click;
        }

        private void ButtonCalendarOpen_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(CalendarActivity));
            intent.PutExtra("goals", SaveExtensions.SerializeSubgoals(CurrentGoal.Subgoals));
            StartActivity(intent);
        }

        private void SetDoGoalButton()
        {
            if (CurrentGoal.GoalType == GoalType.Future)
            {
                doGoalButton.Click += DoGoalButton_Click;
                return;
            }

            doGoalButton.BackgroundTintList = GetColorStateList(Resource.Color.colorGray);
            doGoalButton.SetTextColor(Android.Graphics.Color.LightGray);
            doGoalButton.Text = CurrentGoal.GoalType == GoalType.Done
                ? "Цель уже выполнена"
                : "Цель просрочена";
        }

        private void DoGoalButton_Click(object sender, EventArgs e)
        {
            CurrentGoal.GoalType = GoalType.Done;

            SaveExtensions.OverwriteGoals(Goals);

            user.UserStatistics.GoalsDoneCount++;
            SaveExtensions.OverwriteUser(user);

            OnBackPressed();
        }

        private void SetGoalImage()
        {
            goalImage.SetImage(CurrentGoal.ImagePath);
        }

        private void SetGoalCalendarFab()
        {
            goalCalendarFab.Click += GoalCalendarFab_Click;
        }

        private void GoalCalendarFab_Click(object sender, EventArgs e)
        {
            var datePickerDialog = GetDatePickerDialog
                (
                    $"Изменить дедлайн цели \"{CurrentGoal.Name}\"?",
                    (calendarView) =>
                        {
                            calendarView.MinDate = CurrentGoal.Subgoals.Count > 0
                                ? CurrentGoal.Subgoals.Max(x => x.Deadline).GetDateTimeInMillis()
                                : DateTime.Now.GetDateTimeInMillis();
                            calendarView.Date = CurrentGoal.Deadline.GetDateTimeInMillis();
                        }
                );
            GoalToChangeDate = CurrentGoal;
            datePickerDialog.Show();
        }

        private void OnDateChange(object sender, CalendarView.DateChangeEventArgs args)
        {
            selectedDate = new DateTime(args.Year, args.Month + 1, args.DayOfMonth);
        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            if (!GoalExtensions.IsValidDeadline(selectedDate))
            {
                Toast.MakeText(this, "Неверный дедлайн", ToastLength.Short).Show();
                return;
            }

            var selectedDateInString = selectedDate.ToNiceString();
            GoalToChangeDate.Deadline = selectedDate;
            if (GoalToChangeDate is Goal)
                Toast.MakeText(this, $"Дедлайн цели \"{CurrentGoal.Name}\" изменён на {selectedDateInString}", ToastLength.Long).Show();
            else
                Toast.MakeText(this, $"Дедлайн подцели \"{GoalToChangeDate.Name}\" изменён на {selectedDateInString}", ToastLength.Long).Show();
    
            UpdateSubgoalsListView();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && data != null)
            {
                var tempGoal = SaveExtensions.DeserializeGoal(data.GetStringExtra("goal"));
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
            goalNameTextView.Text = CurrentGoal.Name;
        }

        private void SetListView()
        {
            UpdateSubgoalsListView();
            subgoalsListView.ItemClick += ListView_ItemClick;
        }

        private void ListViewAdapter_LongClick(object sender, int e)
        {
            isLongClick = true;

            var adapter = sender as SubgoalAdapter;
            var selectedItem = adapter.GetItem(e).Cast<Subgoal>();
            currentSubgoal = CurrentGoal.Subgoals.First(x => x.Name == selectedItem.Name);

            ContextMenuExtensions.CreateContextMenu(this, subgoalsListView);
        }

        private void SetFabAddSubgoal()
        {
            var goalType = CurrentGoal.GoalType;
            if (goalType == GoalType.Failed || goalType == GoalType.Done)
            {
                subgoalAddFab.BackgroundTintList = GetColorStateList(Resource.Color.colorGray);
                return;
            }
            subgoalAddFab.Click += SubgoalAddFab_Click;
        }

        private void SubgoalAddFab_Click(object sender, EventArgs e)
        {
            SwitchToAddGoalScreen();
        }

        private void SwitchToAddGoalScreen()
        {
            var intent = new Intent(this, typeof(AddSubgoalActivity));
            intent.PutExtra("currentGoal", SaveExtensions.SerializeGoal(CurrentGoal));
            StartActivityForResult(intent, 1);
        }

        private void UpdateSubgoalsListView()
        {
            SaveExtensions.OverwriteGoals(Goals);
            
            var adapter = new SubgoalAdapter(this, CurrentGoal.Subgoals, subgoalsListView);
            adapter.calendarFabClick += CalendarFab_Click;
            adapter.ItemLongClick += ListViewAdapter_LongClick;
            subgoalsListView.Adapter = adapter;
            datesPythonCalendar.Clear();
            SetPythonCalendarView();
        }

        private void CalendarFab_Click(object sender, EventArgs e)
        {
            var fab = sender as FloatingActionButton;
            var subgoal = SaveExtensions.DeserializeSubgoal((string)fab.Tag);
            var datePickerDialog = GetDatePickerDialog
                (
                $"Изменить дедлайн подцели \"{subgoal.Name}\"?",
                (calendarView) =>
                    {
                        calendarView.MaxDate = CurrentGoal.Deadline.GetDateTimeInMillis();
                        calendarView.Date = subgoal.Deadline.GetDateTimeInMillis();
                    }
                );
            GoalToChangeDate = CurrentGoal.Subgoals.First(x => x.Name == subgoal.Name);
            datePickerDialog.Show();
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            isLongClick = false;
            var selectedItem = subgoalsListView.GetItemAtPosition(e.Position);
            currentSubgoal = selectedItem.Cast<Subgoal>();

            ContextMenuExtensions.CreateContextMenu(this, subgoalsListView);
        }


        public override void OnCreateContextMenu(IContextMenu menu, View view, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, view, menuInfo);

            if (view is ListView)
            {
                if (isLongClick)
                    menu.SetHeaderTitle("Удалить подцель?");
                else
                    menu.SetHeaderTitle("Выполнить подцель?");

                menu.AddOption(ContextMenuOptions.Yes);
                menu.AddOption(ContextMenuOptions.No);

            }            
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var itemId = (ContextMenuOptions)item.ItemId;
            if (itemId == ContextMenuOptions.Yes)
            {
                if (isLongClick)
                    RemoveSubgoal(currentSubgoal);
                else
                {
                    currentSubgoal.GoalType = GoalType.Done;
                    UpdateSubgoalsListView();
                }
                return true;
            }

            return base.OnContextItemSelected(item);
        }

        private void AddSubgoal(Subgoal subgoal)
        {
            CurrentGoal.AddSubgoal(subgoal);
            AddSubgoalUpdateCalendar(subgoal);
            UpdateSubgoalsListView();
        }

        private void AddSubgoalUpdateCalendar(Subgoal goal)
        {
            foreach (var date in datesPythonCalendar)
            {
                if (date.Deadline.Date == goal.Deadline.Date)
                {
                    date.Goal = goal;
                    date.View.Tag = date;
                    date.View.Background = GetDrawable(Resource.Drawable.deadlineMouse1);
                }
            }
            
        }

        private void RemoveSubgoal(Subgoal goal)
        {
            CurrentGoal.RemoveSubgoal(goal);
            DeleteSubgoalUpdateCalendar(goal);
            UpdateSubgoalsListView();
        }

        private void DeleteSubgoalUpdateCalendar(Subgoal goal)
        {
            foreach (var date in datesPythonCalendar)
            {
                if (goal.Deadline.Date == date.Deadline.Date)
                {
                    date.Goal = null;
                    date.View.Background = GetDrawable(Resource.Drawable.dateInCalendarWithPython);
                }
            }
            
        }

        private void SetPythonCalendarView()
        {
            var currentDateTime = DateTime.Now.AddDays(-2);

            for (var i = 1; i < calendar.ChildCount; i++)
            {
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
                {
                    var goal = date.Goal as Subgoal;
                    if (goal.GoalType == GoalType.Done)
                        date.View.Background = GetDrawable(Resource.Drawable.deadlineMouseDead);
                    else
                        date.View.Background = GetDrawable(Resource.Drawable.deadlineMouse1);
                }
            }
        }

        private void SetDatesList(DateTime firstDayInCalendar, TextView dateView)
        {
            var canAddDateWithoutGoal = true;

            foreach (var goal in CurrentGoal.Subgoals)
            {
                if (goal.Deadline.Date == firstDayInCalendar.Date)
                {
                    datesPythonCalendar.Add(new DateInPythonCalendar(goal, goal.Deadline, dateView));
                    canAddDateWithoutGoal = false;
                    break;
                }
            }

            if (canAddDateWithoutGoal)
                datesPythonCalendar.Add(new DateInPythonCalendar(firstDayInCalendar, dateView));
        }

        private DatePickerDialog GetDatePickerDialog(string dialogName, Action<CalendarView> calendarSetup = null)
        {
            var datePickerDialog = CalendarExtensions.GetDatePickerDialog(this, dialogName, OnDateSet, OnDateChange, calendarSetup);
            return datePickerDialog;
        }
    }
}
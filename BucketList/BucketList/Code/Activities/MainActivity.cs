﻿using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Android.Widget;
using Android.Content;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using System.Linq;
using Json.Net;
using AlertDialog = Android.App.AlertDialog;
using Enum = System.Enum;

namespace BucketList
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private static readonly int NOTIFICATION_ID = 1000;
        private static readonly string CHANNEL_ID = "location_notification";
        private int _count = 0;


        public List<Goal> Goals;
        public List<DateInPythonCalendar> datesPythonCalendar;
        private User user;
        private string userName;
        private Goal currentGoalName;
        private GoalType currentGoalType;
        private RelativeLayout calendar;
        private Button buttonCalendarOpen;
        private ListView listView;

        Button goalTypeButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Initialize();
            SetTitle(Resource.String.empty_string);
            SetContentView(Resource.Layout.activity_main);
            SetCalendarValues();
            SetListView();
            SetNavigationView();
            SetUserName();
            SetFab();
            //SetPythonCalendarView();
            SetToolbar();
            SetSearchView();
            UpdateStatistics();
            SetGoalsTypeButton();
        }

        private void SetCalendarValues()
        {
            calendar = FindViewById<RelativeLayout>(Resource.Id.deadlineCalendar);
            buttonCalendarOpen = FindViewById<Button>(Resource.Id.calendarButton);
            buttonCalendarOpen.Click += ButtonCalendarOpen_Click;
        }

        private void SetGoalsTypeButton()
        {
            goalTypeButton = FindViewById<Button>(Resource.Id.button_future_goals);
            goalTypeButton.Click += GoalsTypeButton_Click;
        }

        private void GoalsTypeButton_Click(object sender, EventArgs e)
        {
            var builder = new AlertDialog.Builder(this);
            var dialogView = LayoutInflater.Inflate(Resource.Layout.dialog_filter, null);
            builder.SetView(dialogView);

            var buttonFuture = dialogView.FindViewById<Button>(Resource.Id.buttonFuture);
            var buttonDone = dialogView.FindViewById<Button>(Resource.Id.buttonDone);
            var buttonFailed = dialogView.FindViewById<Button>(Resource.Id.buttonFailed);
            var buttonAll = dialogView.FindViewById<Button>(Resource.Id.buttonAll);

            // Кнопки фильтров
            var filteredButtons = new List<Button>()
            {
                buttonFuture,
                buttonDone,
                buttonFailed,
                buttonAll
            };

            var dialog = builder.Create();
            dialog.Show();

            // Подписываемся на клик кнопки и зыкрываем диалог по клику
            foreach (var filteredButton in filteredButtons)
            {
                // Получаю enum GoalType из Tag кнопки
                var buttonGoalType = (GoalType)Enum.Parse(typeof(GoalType), filteredButton.Tag.ToString());

                filteredButton.Click += (sender, e) =>
                {
                    ChangeCurrentGoalType(buttonGoalType);
                    dialog.Dismiss();
                };

                //Устанавливаем цвет кнопки и возможность прожатия
                if (buttonGoalType == currentGoalType)
                {
                    filteredButton.BackgroundTintList = GetColorStateList(Resource.Color.colorGray);
                    filteredButton.Enabled = false;
                }
                else
                    filteredButton.BackgroundTintList = GetColorStateList(Resource.Color.colorMainViolet);
            }
        }

        private void UpdateStatistics()
        {
            var failedGoals = GetFailedGoals();
            var previouslyFailedGoals = user.UserStatistics.PreviouslyFailedGoals;
            foreach (var failedGoal in failedGoals)
            {
                long hashcode
                    =
                    failedGoal.Name.GetHashCode()
                    + (failedGoal.ImagePath is null ? 0 : failedGoal.ImagePath.GetHashCode())
                    + failedGoal.Deadline.GetHashCode()
                    ;
                if (previouslyFailedGoals.ContainsKey(hashcode))
                    continue;
                previouslyFailedGoals.Add(hashcode, failedGoal);
            }
            SaveExtensions.OverwriteUser(user);
        }

        private List<Goal> GetFailedGoals()
        {
            return Goals
                    .Where(x => x.Deadline < DateTime.Now)
                    .ToList();
        }

        private void SetSearchView()
        {
            var searchView = FindViewById<Android.Widget.SearchView>(Resource.Id.main_screen_search);
            searchView.QueryTextChange += SearchView_QueryTextChange;
        }

        private void SearchView_QueryTextChange(object sender, Android.Widget.SearchView.QueryTextChangeEventArgs e)
        {
            var searchView = sender as Android.Widget.SearchView;
            var text = searchView.Query.ToLower().Trim();
            if (string.IsNullOrEmpty(text))
            {
                UpdateGoalsView();
                return;
            }
            var goals =
                Goals
                .Where(x => x.Name.ToLower().Contains(text))
                .ToList();
            UpdateGoalsViewForView(goals);
        }

        private void Initialize()
        {
            datesPythonCalendar = new List<DateInPythonCalendar>();

            if (string.IsNullOrEmpty(SaveExtensions.ReadGoals()))
                SaveExtensions.OverwriteGoals(SaveExtensions.SerializeGoals(new List<Goal>()));

            Goals = SaveExtensions.GetSavedGoals();
            user = SaveExtensions.GetSavedUser();

            currentGoalType = GoalType.Any;
            SetFailedGoals();
        }

        private void SetFailedGoals()
        {
            foreach (var goal in Goals)
            {
                if (goal.Deadline < DateTime.Now)
                {
                    goal.GoalType = GoalType.Failed;
                }
            }
        }

        private void MyListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            // Получите ссылку на ListView
            ListView myListView = sender as ListView;

            // Получите выбранный элемент
            var selectedItem = (string)myListView.GetItemAtPosition(e.Position);
            currentGoalName = Goals.First(x => x.Name == selectedItem);

            // Отобразите контекстное меню
            RegisterForContextMenu(myListView);

            // Откройте контекстное меню для выбранного элемента
            OpenContextMenu(myListView);
        }

        public override void OnCreateContextMenu(IContextMenu menu, View view, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, view, menuInfo);

            // Меню для цели
            if (view is ListView)
            {
                menu.SetHeaderTitle("Удалить цель?");

                menu.Add(Menu.None, 0, Menu.None, "Да");
                menu.Add(Menu.None, 1, Menu.None, "Нет");
            }
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            // Меню для цели
            if (item.ItemId == 0)
            {
                RemoveGoal(currentGoalName);
                return true;
            }

            return base.OnContextItemSelected(item);
        }

        private void ChangeCurrentGoalType(GoalType newGoalType)
        {
            currentGoalType = newGoalType;
            if (currentGoalType == GoalType.Any)
            {
                var goals = Goals.ToList();
                datesPythonCalendar.Clear();
                SetPythonCalendarView();
                UpdateGoalsViewForView(goals);
            }
            else
            {
                var goals = Goals.Where(x => x.GoalType == newGoalType).ToList();
                datesPythonCalendar.Clear();
                SetPythonCalendarView();
                UpdateGoalsViewForView(goals);
            }
            
        }
         
        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                //base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(this, typeof(AddGoalActivity));
            StartActivityForResult(intent, 1);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            var id = item.ItemId;

            if (id == Resource.Id.nav_statistics)
            {
                var intent = new Intent(this, typeof(StatisticsActivity));
                StartActivityForResult(intent, 1);
            }
            else if (id == Resource.Id.nav_account)
            {
                var intent = new Intent(this, typeof(AccountActivity));
                StartActivityForResult(intent, 1);
            }
            else if (id == Resource.Id.nav_python_settings)
            {
                var intent = new Intent(this, typeof(PythonActivity));
                StartActivity(intent);
            }
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            SetUserName();
            Goals = SaveExtensions.GetSavedGoals();
            UpdateGoalsView();
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && data != null)
            {
                var newGoal = SaveExtensions.DeserializeGoal(data.GetStringExtra("goal"));
                AddGoal(newGoal);
            }
            datesPythonCalendar.Clear();
            SetPythonCalendarView();
        }

        private List<Goal> GetGoalsForListViewWithGoalType(GoalType goalType)
        {
            if (Goals != null)
            {
                if (goalType == GoalType.Any)
                    return Goals.ToList();
                var goals = Goals.Where(x => x.GoalType == goalType);
                if (goals != null)
                    return goals.ToList();
            }
            return new List<Goal>();
        }

        private void AddGoal(Goal goal)
        {
            Goals.Add(goal);
            foreach (var date in datesPythonCalendar)
            {
                AddGoalUpdateCalendar(goal, date);
            }
            user.UserStatistics.GoalsCreatedCount++;
            SaveExtensions.OverwriteUser(user);
            UpdateGoalsView();

            var notificationTime = DateTime.Now.AddSeconds(15);
            ShowNotification(notificationTime, "чел", "АВХАВХЗАВХАВХАВХАВХВАВХААВХАВХВАХВХАХАВАХВХВАХВАХАВХАВХАВХВАХВАХВАХВАХАВ");


            //ShowNotification("test", "fdg");

            //var jobScheduler = (JobScheduler)GetSystemService(JobSchedulerService);

            //var jobBuilder = new JobInfo.Builder(0, new ComponentName(this, Java.Lang.Class.FromType(typeof(MyJobService))));
            //jobBuilder.SetPersisted(true);
            //jobBuilder.SetRequiredNetworkType(NetworkType.Any);
            //jobBuilder.SetRequiresCharging(false);

            //var currentTimeMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
            //var notificationTimeMillis = (long)(notificationTime - new DateTime(1970, 1, 1)).TotalMilliseconds;

            //jobBuilder.SetMinimumLatency(notificationTimeMillis - currentTimeMillis);

            //var jobInfo = jobBuilder.Build();

            //jobScheduler.Schedule(jobInfo);
        }

        private void ShowNotification(DateTime notificationTime, string title, string message)
        {
            var intent = new Intent(this, typeof(NotificationService));
            intent.PutExtra("notificationTime", notificationTime.GetDateTimeInMillis());
            intent.PutExtra("title", title);
            intent.PutExtra("message", message);

            StartService(intent);
        }

        private void RemoveGoal(Goal goal)
        {
            ImageExtensions.DeleteImage(goal.ImagePath);
            Goals.Remove(goal);
            foreach (var date in datesPythonCalendar)
            {
                DeleteGoalUpdateCalendar(goal, date);
            }
            user.UserStatistics.GoalsDeletedCount++;
            SaveExtensions.OverwriteUser(user);
            UpdateGoalsView();
        }

        private void UpdateGoalsView()
        {
            var serializedGoals = SaveExtensions.SerializeGoals(Goals);
            SaveExtensions.OverwriteGoals(serializedGoals);
            //var listView = FindViewById<ListView>(Resource.Id.goalsListView);
            //var adapter = new ArrayAdapter<string>(this, Resource.Layout.all_goals_list_item, Goals.Select(x => x.Name).ToList());
            //listView.Adapter = adapter;
            var goals = GetGoalsForListViewWithGoalType(currentGoalType);
            UpdateGoalsViewForView(goals);
            SetPythonCalendarView();
        }

        private void UpdateGoalsViewForView(List<Goal> goals)
        {
            var listView = FindViewById<ListView>(Resource.Id.goalsListView);
            var adapter = new GoalAdapter(this, goals, listView);
            adapter.ItemLongClick += Adapter_ItemLongClick;
            //adapter.ItemLongClick += GoalAdapter_ItemLongClick;
            listView.Adapter = adapter;
            //var adapter = new ArrayAdapter<string>(this, Resource.Layout.all_goals_list_item, Resource.Id.rectangle_1 , goals);
            //listView.Adapter = adapter;
        }

        private void Adapter_ItemLongClick(object sender, int e)
        {
            // Получите ссылку на ListView
            var myListView = listView;
            var adapter = sender as GoalAdapter;

            // Получите выбранный элемент
            var selectedItem = adapter.GetItem(e).Cast<Goal>(); ;
            currentGoalName = Goals.First(x => x.Name == selectedItem.Name);

            // Отобразите контекстное меню
            RegisterForContextMenu(myListView);

            // Откройте контекстное меню для выбранного элемента
            OpenContextMenu(myListView);
        }

        private void GoalAdapter_ItemLongClick(object sender, int e)
        {
            // Получите ссылку на ListView
            ListView myListView = sender as ListView;

            // Получите выбранный элемент
            var selectedItem = myListView.GetItemAtPosition(e).Cast<Goal>();
            currentGoalName = Goals.First(x => x.Name == selectedItem.Name);

            // Отобразите контекстное меню
            RegisterForContextMenu(myListView);

            // Откройте контекстное меню для выбранного элемента
            OpenContextMenu(myListView);
        }

        private void OnGoalClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            // Получите ссылку на ListView
            ListView myListView = sender as ListView;

            // Получите выбранный элемент
            var selectedItem = myListView.GetItemAtPosition(e.Position).Cast<Goal>();
            Intent intent = new Intent(this, typeof(GoalActivity));
            intent.PutExtra("goal", JsonNet.Serialize(Goals.First(x => x.Name == selectedItem.Name)));
            StartActivityForResult(intent, 1);
        }

        private void SetNavigationView()
        {
            var user = SaveExtensions.GetSavedUser();
            userName = user.UserName;
            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            var headerView = navigationView.GetHeaderView(0);
            var userImage = headerView.FindViewById<ImageView>(Resource.Id.imageView);
            userImage.SetImage(user.UserPhotoPath);
            navigationView.SetNavigationItemSelectedListener(this);
        }

        private void SetUserName()
        {
            var user = SaveExtensions.GetSavedUser();
            userName = user.UserName;
            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            var headerView = navigationView.GetHeaderView(0);
            var usernameTextView = headerView.FindViewById<TextView>(Resource.Id.usernameMainTextView);
            if (!string.IsNullOrEmpty(userName))
            {
                usernameTextView.Text = userName;
            }
        }

        private void SetListView()
        {
            var listView = FindViewById<ListView>(Resource.Id.goalsListView);
            this.listView = listView;
            UpdateGoalsView();
            listView.ItemClick += OnGoalClick;
            //listView.LongClick += MyListView_ItemLongClick;
        }


        private void SetFab()
        {
            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
        }

        private void SetToolbar()
        {
            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();
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
                {
                    var goal = date.Goal as Goal;
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

            if (currentGoalType != GoalType.Any)
            {
                foreach (var goal in Goals.Where(x => x.GoalType == currentGoalType))
                {
                    if (goal.Deadline.Date == firstDayInCalendar.Date)
                    {
                        datesPythonCalendar.Add(new DateInPythonCalendar(goal, goal.Deadline, dateView));
                        canAddDateWithoutGoal = false;
                        break;
                    }
                }
            }
            else
            {
                foreach (var goal in Goals)
                {
                    if (goal.Deadline.Date == firstDayInCalendar.Date)
                    {
                        datesPythonCalendar.Add(new DateInPythonCalendar(goal, goal.Deadline, dateView));
                        canAddDateWithoutGoal = false;
                        break;
                    }
                }
            }
            

            if (canAddDateWithoutGoal)
                datesPythonCalendar.Add(new DateInPythonCalendar(firstDayInCalendar, dateView));
        }

        private void ButtonCalendarOpen_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(CalendarActivity));
            intent.PutExtra("goals", SaveExtensions.SerializeGoals(Goals));
            StartActivity(intent);
        }

        private void AddGoalUpdateCalendar(Goal goal, DateInPythonCalendar date)
        {
            if (date.Deadline.Date == goal.Deadline.Date)
            {
                date.Goal = goal;
                date.View.Tag = date;
                if (goal.GoalType != GoalType.Done)
                    date.View.Background = GetDrawable(Resource.Drawable.deadlineMouse1);
                else
                    date.View.Background = GetDrawable(Resource.Drawable.deadlineMouseDead);
            }
        }

        private void DeleteGoalUpdateCalendar(Goal goal, DateInPythonCalendar date)
        {
            if (goal.Deadline.Date == date.Deadline.Date)
            {
                date.Goal = null;
                date.View.Background = GetDrawable(Resource.Drawable.dateInCalendarWithPython);
            }
        }
    }
}




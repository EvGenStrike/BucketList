using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Google.Android.Material.Snackbar;
using Android.Widget;
using Java.Lang;
using System.Linq;
using AndroidX.Core.Util;
using Android.Content;
using System.IO;
using Android.Text;
using Google.Android.Material.Resources;
using Android.Text.Style;
using Android.Util;
using Json.Net;
using AlertDialog = Android.App.AlertDialog;
using Java.Util;
using AndroidX.Core.App;

namespace BucketList
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private static readonly int NOTIFICATION_ID = 1000;
        private static readonly string CHANNEL_ID = "location_notification";
        private int _count = 0;


        public List<Goal> Goals;
        public List<DatePythonCalendar> datesPythonCalendar;
        private User user;
        private string userName;
        private Goal currentGoalName;
        private GoalType currentGoalType;

        Button goalTypeButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Initialize();
            SetTitle(Resource.String.empty_string);
            SetContentView(Resource.Layout.activity_main);
            SetListView();
            SetNavigationView();
            SetUserName();
            SetFab();
            SetToolbar();
            SetPythonCalendarView();
            SetSearchView();
            UpdateStatistics();
            SetGoalsTypeButton();
        }

        private void SetGoalsTypeButton()
        {
            goalTypeButton = FindViewById<Button>(Resource.Id.button_future_goals);
            goalTypeButton.Click += GoalsTypeButton_Click;
            UpdateGoalTypeButtonText();
        }

        private void UpdateGoalTypeButtonText()
        {
            switch (currentGoalType)
            {
                case GoalType.Future:
                    goalTypeButton.Text = "Поставленные цели";
                    break;
                case GoalType.Done:
                    goalTypeButton.Text = "Выполненные цели";
                    break;
                case GoalType.Failed:
                    goalTypeButton.Text = "Просроченные цели";
                    break;
            }
        }

        private void GoalsTypeButton_Click(object sender, EventArgs e)
        {
            var goalsTypeButton = sender as Button;

            RegisterForContextMenu(goalsTypeButton);

            OpenContextMenu(goalsTypeButton);
        }

        private void UpdateStatistics()
        {
            var failedGoals = GetFailedGoals();
            var previouslyFailedGoals = user.UserStatistics.PreviouslyFailedGoals;
            foreach (var failedGoal in failedGoals)
            {
                var hashcode = failedGoal.GetHashCode();
                if (previouslyFailedGoals.ContainsKey(hashcode))
                    continue;
                previouslyFailedGoals.Add(hashcode, failedGoal);
            }
            Extensions.OverwriteUser(user);
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
                .Select(x => x.GoalName)
                .Where(x => x.ToLower().Contains(text))
                .ToList();
            UpdateGoalsViewForView(goals);
        }

        private void Initialize()
        {
            datesPythonCalendar = new List<DatePythonCalendar>();
            if (string.IsNullOrEmpty(Extensions.ReadGoals()))
                Extensions.OverwriteGoals(Extensions.SerializeGoals(new List<Goal>()));
            Goals = Extensions.GetSavedGoals();
            user = Extensions.GetSavedUser();
            currentGoalType = GoalType.Future;
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
            currentGoalName = Goals.First(x => x.GoalName == selectedItem);

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

            // Меню для выбора отображаемых целей
            if (view is Button)
            {
                menu.SetHeaderTitle("Выберите тип отображаемых целей");

                menu.Add(Menu.None, 2, Menu.None, "Поставленные цели");
                menu.Add(Menu.None, 3, Menu.None, "Выполненные цели");
                menu.Add(Menu.None, 4, Menu.None, "Просроченные цели");
            }

            // Меню для даты в календаре с питоном
            else if (view is TextView)
            {

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

            // Меню для выбора отображаемых целей
            //"Поставленные цели"
            if (item.ItemId == 2)
            {
                ChangeCurrentGoalType(GoalType.Future);
                UpdateGoalTypeButtonText();
                return true;
            }
            //"Выполненые цели"
            if (item.ItemId == 3)
            {
                ChangeCurrentGoalType(GoalType.Done);
                UpdateGoalTypeButtonText();
                return true;
            }
            //"Просроченные цели"
            if (item.ItemId == 4)
            {
                ChangeCurrentGoalType(GoalType.Failed);
                UpdateGoalTypeButtonText();
                return true;
            }

            return base.OnContextItemSelected(item);
        }

        private void ChangeCurrentGoalType(GoalType newGoalType)
        {
            currentGoalType = newGoalType;
            var goals = Goals.Where(x => x.GoalType == newGoalType).Select(x => x.GoalName).ToList();
            UpdateGoalsViewForView(goals);
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

            }
            else if (id == Resource.Id.nav_python_settings)
            {

            }
            else if (id == Resource.Id.nav_allow_notifications)
            {

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
            Goals = Extensions.GetSavedGoals();
            UpdateGoalsView();
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && data != null)
            {
                var newGoal = JsonNet.Deserialize<Goal>(data.GetStringExtra("goal"));
                AddGoal(newGoal);
            }
        }

        private List<string> GetGoalsForListViewWithGoalType(GoalType goalType)
        {
            return Goals.Where(x => x.GoalType == goalType).Select(x => x.GoalName).ToList();
        }

        private void AddGoal(Goal goal)
        {
            Goals.Add(goal);
            foreach (var date in datesPythonCalendar)
            {
                SetDeadlineDate(goal, date);
            }
            user.UserStatistics.GoalsCreatedCount++;
            Extensions.OverwriteUser(user);
            UpdateGoalsView();

            var notificationTime = DateTime.Now;

            //var builder = new NotificationCompat.Builder(this, CHANNEL_ID).SetAutoCancel(true)
            //    .SetContentTitle("test")
            //    .SetNumber(_count)
            //    .SetSmallIcon(Resource.Drawable.snake_image)
            //    .SetContentText($"пожалуйста заработай");

            //var nmc = NotificationManagerCompat.From(this);
            //nmc.Notify(NOTIFICATION_ID, builder.Build());
            //_count++;

            //// Получаем объект AlarmManager
            //var alarmManager = (AlarmManager)GetSystemService(Context.AlarmService);

            //// Создаем интент для запуска вашей службы уведомлений
            //var intent = new Intent(this, typeof(NotificationService));

            //// Устанавливаем дату и время, за день до которой нужно отправить уведомление
            //var notificationDate = notificationTime.AddMinutes(1);

            //// Устанавливаем время, в которое нужно запустить уведомление
            //var calendar = Calendar.GetInstance(Java.Util.TimeZone.Default);
            //calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
            //calendar.Set(CalendarField.HourOfDay, notificationDate.Hour);
            //calendar.Set(CalendarField.Minute, notificationDate.Minute);
            //calendar.Set(CalendarField.Second, 0);

            //// Устанавливаем интент в AlarmManager для запуска службы уведомлений в нужное время
            //var pendingIntent = PendingIntent.GetService(this, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            //alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, calendar.TimeInMillis, pendingIntent);
        }

        private void RemoveGoal(Goal goal)
        {
            Extensions.DeleteImage(goal.ImagePath);
            Goals.Remove(goal);
            foreach (var date in datesPythonCalendar)
            {
                DeleteDeadlineFromDate(goal, date);
            }
            user.UserStatistics.GoalsDeletedCount++;
            Extensions.OverwriteUser(user);
            UpdateGoalsView();
        }

        private void UpdateGoalsView()
        {
            var serializedGoals = Extensions.SerializeGoals(Goals);
            Extensions.OverwriteGoals(serializedGoals);
            //var listView = FindViewById<ListView>(Resource.Id.goalsListView);
            //var adapter = new ArrayAdapter<string>(this, Resource.Layout.all_goals_list_item, Goals.Select(x => x.GoalName).ToList());
            //listView.Adapter = adapter;
            UpdateGoalsViewForView(GetGoalsForListViewWithGoalType(currentGoalType));
        }

        private void UpdateGoalsViewForView(List<string> goals)
        {
            var listView = FindViewById<ListView>(Resource.Id.goalsListView);
            var adapter = new ArrayAdapter<string>(this, Resource.Layout.all_goals_list_item, goals);
            listView.Adapter = adapter;
        }

        private void OnGoalClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            // Получите ссылку на ListView
            ListView myListView = sender as ListView;

            // Получите выбранный элемент
            var selectedItem = (string)myListView.GetItemAtPosition(e.Position);
            Intent intent = new Intent(this, typeof(GoalActivity));
            intent.PutExtra("goal", JsonNet.Serialize(Goals.First(x => x.GoalName == selectedItem)));
            StartActivityForResult(intent, 1);
        }

        private void SetNavigationView()
        {
            var user = Extensions.GetSavedUser();
            userName = user.UserName;
            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            var headerView = navigationView.GetHeaderView(0);
            var userImage = headerView.FindViewById<ImageView>(Resource.Id.imageView);
            userImage.SetImage(user.UserPhotoPath);
            navigationView.SetNavigationItemSelectedListener(this);
        }

        private void SetUserName()
        {
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
            UpdateGoalsView();
            listView.ItemClick += OnGoalClick;
            listView.ItemLongClick += MyListView_ItemLongClick;
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
            var calendar = FindViewById<RelativeLayout>(Resource.Id.deadlineCalendar);
            var buttonCalendarOpen = FindViewById<Button>(Resource.Id.calendarButton);
            buttonCalendarOpen.Click += ButtonCalendarOpen_Click;

            var currentDateTime = DateTime.Now.AddDays(-3);
            
            for (var i = 0; i < calendar.ChildCount; i++)
            {
                // Дата - это TextView в Layout
                var dateView = calendar.GetChildAt(i) as TextView;
                if (dateView == null) return;
                var date = new DatePythonCalendar(currentDateTime, dateView);
                dateView.Text = currentDateTime.Day.ToString();
                datesPythonCalendar.Add(date);
                foreach (var goal in Goals)
                {
                    SetDeadlineDate(goal, date);
                }
                // Изменяем текущий день на следующий
                currentDateTime = currentDateTime.AddDays(1);
            }
        }

        private void ButtonCalendarOpen_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(CalendarActivity));
            intent.PutExtra("goals", Extensions.SerializeGoals(Goals));
            StartActivity(intent);
        }

        private void SetDeadlineDate(Goal goal, DatePythonCalendar date)
        {
            if (date.Deadline.Date == goal.Deadline.Date)
            {
                date.Goal = goal;
                date.View.Tag = date;
                date.View.Background = GetDrawable(Resource.Drawable.deadlineMouse1);
            }
        }
        private void DeleteDeadlineFromDate(Goal goal, DatePythonCalendar date)
        {
            if (goal.Deadline.Date == date.Deadline.Date)
            {
                date.Goal = null;
                date.View.Background = GetDrawable(Resource.Drawable.dateInCalendarWithPython);
            }
        }
    }
}




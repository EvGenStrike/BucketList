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

namespace BucketList
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        public List<Goal> Goals;
        private string userName;
        private Goal currentGoalName;
        private Dictionary<TextView, DateTime> datesPythonCalendar;

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
        }

        private void Initialize()
        {
            //goals = new List<Goal>
            //{
            //    new Goal("Прочесть книгу", new DateTime(2024, 6, 3)),
            //    new Goal("Выучить Java", new DateTime(2024, 7, 3)),
            //    new Goal("Сдать сессию", new DateTime(2024, 8, 3)),
            //};
            datesPythonCalendar = new Dictionary<TextView, DateTime>();
            if (string.IsNullOrEmpty(GoalExtensions.ReadGoals()))
                GoalExtensions.OverwriteGoals(GoalExtensions.SerializeGoals(new List<Goal>()));
            Goals = GoalExtensions.GetSavedGoals();
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

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);

            menu.SetHeaderTitle("Удалить цель?");

            menu.Add(Menu.None, 0, Menu.None, "Да");
            menu.Add(Menu.None, 1, Menu.None, "Нет");
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            if (item.ItemId == 0)
            {
                RemoveGoal(currentGoalName);
                return true;
            }

            return base.OnContextItemSelected(item);
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
            //View view = (View)sender;
            //AddGoal("Пойти спать");
            //Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
            //    .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_camera)
            {
                // Handle the camera action
            }
            else if (id == Resource.Id.nav_gallery)
            {

            }
            else if (id == Resource.Id.nav_slideshow)
            {

            }
            else if (id == Resource.Id.nav_manage)
            {

            }
            else if (id == Resource.Id.nav_share)
            {

            }
            else if (id == Resource.Id.nav_send)
            {

            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
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
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && data != null)
            {
                var newGoal = JsonNet.Deserialize<Goal>(data.GetStringExtra("goal"));
                AddGoal(newGoal);
            }
        }

        private void AddGoal(Goal goal)
        {
            Goals.Add(goal);
            foreach (var date in datesPythonCalendar)
            {
                foreach (var goal1 in Goals)
                {
                    if (date.Value.Date == goal1.Deadline.Date)
                        date.Key.Background = GetDrawable(Resource.Drawable.deadlineMouse1);
                }
            }
            UpdateGoalsView();
        }

        private void RemoveGoal(Goal goal)
        {
            GoalExtensions.DeleteImage(goal.ImagePath);
            Goals.Remove(goal);
            foreach (var date in datesPythonCalendar)
            {
                foreach (var goal1 in Goals)
                {
                    if (date.Value.Date != goal1.Deadline.Date)
                        date.Key.Background = GetDrawable(Resource.Drawable.dateInCalendarWithPython);
                }
            }
            UpdateGoalsView();
        }

        private void UpdateGoalsView()
        {
            var serializedGoals = GoalExtensions.SerializeGoals(Goals);
            GoalExtensions.OverwriteGoals(serializedGoals);
            var listView = FindViewById<ListView>(Resource.Id.goalsListView);
            var adapter = new ArrayAdapter<string>(this, Resource.Layout.all_goals_list_item, Goals.Select(x => x.GoalName).ToList());
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
            userName = Intent.GetStringExtra("username");
            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
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
            //var calendarView = FindViewById<CalendarView>(Resource.Id.allGoalsCalendarView);
            //calendarView.FirstDayOfWeek = 2;
            //calendarView.MinDate = DateTime.Now.GetDateTimeInMillis();
            //calendarView.DateChange += CalendarView_DateChange;
            //calendarView.DateTextAppearance = (Android.Resource.Style.TextAppearanceMedium);

            // Set the highlighted dates
            List<DateTime> highlightedDates = new List<DateTime>()
            {
                new DateTime(2023, 6, 1),
                new DateTime(2023, 6, 10),
                new DateTime(2023, 6, 20)
            };
            //calendarView.SetHighlightedDates(highlightedDates);
            var currentDateTime = DateTime.Now;
            var calendar = FindViewById<RelativeLayout>(Resource.Id.deadlineCalendar);
            for (var i = 0; i < calendar.ChildCount; i++)
            {
                var date = calendar.GetChildAt(i) as TextView;
                if (date == null) return;
                date.Text = currentDateTime.Day.ToString();
                datesPythonCalendar[date] = currentDateTime;
                foreach (var goal in Goals)
                {
                    if (goal.Deadline.Date == datesPythonCalendar[date].Date)
                        date.Background = GetDrawable(Resource.Drawable.deadlineMouse1);
                }
                currentDateTime = currentDateTime.AddDays(1);

            }
        }

        private void CalendarView_DateChange(object sender, CalendarView.DateChangeEventArgs e)
        {
            var calendarView = (CalendarView)sender;

        }
    }
}




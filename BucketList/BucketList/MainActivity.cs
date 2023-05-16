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

namespace BucketList
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private List<string> goals;
        private TextView userName;
        private string currentGoalName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetTitle(Resource.String.empty_string);
            SetContentView(Resource.Layout.activity_main);
            var listView = FindViewById<ListView>(Resource.Id.goalsListView);

            

            string internalStoragePath = Application.Context.FilesDir.AbsolutePath;
            string filePath = System.IO.Path.Combine(internalStoragePath, "myfile.txt");
            var result = "";
            using (StreamReader writer = new StreamReader(filePath))
            {
                result = writer.ReadLine();
            }



            goals = new List<string> { "Прочесть книгу", "Выучить Java", "Получить место работы в Яндексе" };
            var adapter = new ArrayAdapter<string>(this, Resource.Layout.all_goals_list_item, goals);
            listView.Adapter = adapter;
            listView.ItemClick += (sender, e) =>
            {
                listView.Adapter =
                new ArrayAdapter<string>(this, Resource.Layout.all_goals_list_item, goals);
            };

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            // Установка обработчика долгого нажатия
            listView.ItemLongClick += MyListView_ItemLongClick;
            

        }
        
        private void MyListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            // Получите ссылку на ListView
            ListView myListView = sender as ListView;

            // Получите выбранный элемент
            var selectedItem = myListView.GetItemAtPosition(e.Position);
            currentGoalName = (string)selectedItem;

            //// Отобразите контекстное меню
            RegisterForContextMenu(myListView);

            //// Откройте контекстное меню для выбранного элемента
            OpenContextMenu(myListView);
        }
        private void RemoveGoal(string goalName)
        {
            goals.Remove(goalName);
            UpdateGoalsView();
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            // Установите заголовок контекстного меню
            menu.SetHeaderTitle("Удалить цель?");

            // Добавьте пункт меню для удаления элемента
            menu.Add(Menu.None, 1, Menu.None, "Да");

        }
        public override bool OnContextItemSelected(IMenuItem item)
        {
            // Проверьте, выбран ли пункт меню "Delete"
            if (item.ItemId == 1)
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
                base.OnBackPressed();
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
                string newItem = data.GetStringExtra("newItem");
                AddGoal(newItem);
            }
        }

        private void AddGoal(string goal)
        {
            goals.Add(goal);
            UpdateGoalsView();
        }

        private void UpdateGoalsView()
        {
            var listView = FindViewById<ListView>(Resource.Id.goalsListView);
            var adapter = new ArrayAdapter<string>(this, Resource.Layout.all_goals_list_item, goals);
            listView.Adapter = adapter;
        }

        private void InitializeMainScreen()
        {

        }
    }
}


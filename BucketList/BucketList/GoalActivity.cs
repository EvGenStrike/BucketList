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
using System.Text;
using static AndroidX.RecyclerView.Widget.RecyclerView;

namespace BucketList
{
    [Activity(Label = "GoalActivity")]
    public class GoalActivity : Activity
    {
        public List<Goal> Goals { get; private set; }
        public Goal CurrentGoal { get; private set; }
        private Subgoal currentSubgoal;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_goal_screen);
            SetGoals();
            Initialize();
            SetGoalText();
            SetListView();
            SetFabAddSubgoal();
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
            var listView = FindViewById<ListView>(Resource.Id.subgoals_items);
            var adapter = new SubgoalAdapter(this, CurrentGoal.Subgoals, listView);
            listView.Adapter = adapter;
            listView.ItemClick += ListView_ItemClick;
            //listView.ItemLongClick += ListView_ItemLongClick;
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
            var intent = new Intent(this, typeof(AddGoalActivity));
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
            var listView = FindViewById<ListView>(Resource.Id.subgoals_items);
            var adapter = new SubgoalAdapter(this, CurrentGoal.Subgoals, listView);
            listView.Adapter = adapter;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //Toast.MakeText(this, "asdfasf", ToastLength.Long).Show();
            ShowContextMenu(sender, e);
        }

        private void ShowContextMenu(object sender, AdapterView.ItemClickEventArgs e)
        {
            ListView myListView = sender as ListView;

            // Получите выбранный элемент
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
            // Проверьте, выбран ли пункт меню "Delete"
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
    }
}
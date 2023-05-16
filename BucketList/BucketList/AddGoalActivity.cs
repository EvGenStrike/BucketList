using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.DrawerLayout.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BucketList
{
    [Activity(Label = "AddGoalActivity")]
    public class AddGoalActivity : Activity
    {
        Button goalAddButton;
        EditText goalAddNameEditText;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_goal);
            goalAddButton = FindViewById<Button>(Resource.Id.goal_add_button);
            goalAddButton.Click += OnClick;

            goalAddNameEditText = FindViewById<EditText>(Resource.Id.goal_add_name_text);
        }

        private void OnClick(object sender, EventArgs e)
        {
            var intent = new Intent();
            intent.PutExtra("newItem", GetGoalNameText());
            SetResult(Result.Ok, intent);
            Finish();
        }

        private string GetGoalNameText()
        {
            var text = goalAddNameEditText.Text;
            if (text == "") return "Новая цель";
            return text;
        }
    }
}
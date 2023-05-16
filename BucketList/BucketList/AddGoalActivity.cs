using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BucketList
{
    [Activity(Label = "AddGoalActivity")]
    public class AddGoalActivity : Activity
    {
        Button button1;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_goal);
            button1 = FindViewById<Button>(Resource.Id.button1);
            button1.Click += OnClick;
        }

        private void OnClick(object sender, EventArgs e)
        {
            var intent = new Intent();
            intent.PutExtra("newItem", "Новый элемент списка");
            SetResult(Result.Ok, intent);
            Finish();
        }
    }
}
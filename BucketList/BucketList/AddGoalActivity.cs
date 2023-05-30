using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.FloatingActionButton;
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

            var datePickerButton = FindViewById<Button>(Resource.Id.button1);
            datePickerButton.Click += DatePickerButton_Click;
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
        private void DatePickerButton_Click(object sender, EventArgs e)
        {
            var currentDate = DateTime.Now;
            DatePickerDialog datePickerDialog 
                = new DatePickerDialog(
                                    this,
                                    OnDateSet,
                                    currentDate.Year,
                                    currentDate.Month - 1,
                                    currentDate.Day
                                    );
            datePickerDialog.DatePicker.MinDate = DateTime.Now.GetDateTimeInMillis();
            datePickerDialog.Show();
            
        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            // Получите выбранную дату из DatePickerDialog
            int year = e.Year;
            int month = e.Month + 1; // Здесь месяц начинается с 0, поэтому добавляем 1
            int dayOfMonth = e.DayOfMonth;

            // Выполните необходимые действия с выбранной датой
            string selectedDate = $"{dayOfMonth}-{month}-{year}";
            Toast.MakeText(this, $"Выбранная дата: {selectedDate}", ToastLength.Short).Show();
        }
    }
}
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BucketList
{
    [Activity(Label = "AddGoalActivity")]
    public class AddGoalActivity : Activity
    {
        private List<Goal> goals;

        private Button addGoalButton;
        private EditText goalNameEditText;
        private Button chooseImageButton;
        private ImageView goalImage;
        private Button datePickerButton;

        private DateTime selectedDate;
        private string imagePath;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_goal);
            Initialize();
            SetAddGoalButton();
            SetChooseImageButton();
            SetDatePickerButton();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == Constants.PickImageRequestCode) && (resultCode == Result.Ok) && (data != null))
            {
                var imagePath = ImageExtensions.CopyImageToAppFiles(this, data.Data);
                this.imagePath = imagePath;
                goalImage.SetImage(imagePath);
            }
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);

            menu.SetHeaderTitle("Загрузить картинку?");

            menu.AddOption(ContextMenuOptions.Yes);
            menu.AddOption(ContextMenuOptions.No);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var itemId = (ContextMenuOptions)item.ItemId;
            switch (itemId)
            {
                case ContextMenuOptions.Yes:
                    ImageExtensions.UploadImage(this);
                    return true;
                default:
                    return base.OnContextItemSelected(item);
            }
        }

        public override void OnBackPressed()
        {
            ImageExtensions.DeleteImage(imagePath);
            base.OnBackPressed();
        }

        public virtual void InitializeAdditionalData()
        {
            goals = SaveExtensions.GetSavedGoals();
        }

        public virtual bool IsValidGoal(string goalName, DateTime goalDeadline)
        {
            if (goals.Any(x => x.Name == goalName))
            {
                Toast.MakeText(this, "Цель с таким названием уже существует", ToastLength.Short).Show();
                return false;
            }
            if (!GoalExtensions.IsValidDeadline(goalDeadline))
            {
                Toast.MakeText(this, "Неверный дедлайн", ToastLength.Short).Show();
                return false;
            }
            return true;
        }

        public virtual void CalendarSetup(CalendarView calendarView)
        {
            var selectedDate = GetGoalDateTime();
            if (GoalExtensions.IsValidDeadline(selectedDate))
                calendarView.Date = selectedDate.GetDateTimeInMillis();
        }

        public DateTime GetGoalDateTime()
        {
            return selectedDate;
        }

        private void Initialize()
        {
            addGoalButton = FindViewById<Button>(Resource.Id.add_goal_screen_add_goal_button);
            goalNameEditText = FindViewById<EditText>(Resource.Id.add_goal_screen_add_goal_name_edit_text);
            chooseImageButton = FindViewById<Button>(Resource.Id.add_goal_screen_choose_image_button);
            goalImage = FindViewById<ImageView>(Resource.Id.add_goal_screen_goal_image);
            datePickerButton = FindViewById<Button>(Resource.Id.add_goal_screen_add_goal_deadline_button);
            InitializeAdditionalData();
        }        

        private void SetAddGoalButton()
        {
            addGoalButton.Click += AddGoalButton_Click;
        }

        private void SetChooseImageButton()
        {
            chooseImageButton.Click += ChooseImageButton_Click;
        }

        private void SetDatePickerButton()
        {
            datePickerButton.Click += DatePickerButton_Click;
        }

        private void AddGoalButton_Click(object sender, EventArgs e)
        {
            var goalName = GetGoalNameText();
            var goalDeadline = GetGoalDateTime();
            if (!IsValidGoal(goalName, goalDeadline))
                return;
            var goal = new Goal(goalName, goalDeadline, imagePath);
            var serializedGoal = SaveExtensions.SerializeGoal(goal);
            var intent = new Intent();
            intent.PutExtra("goal", serializedGoal);
            SetResult(Result.Ok, intent);
            Finish();
        }

        private void ChooseImageButton_Click(object sender, EventArgs e)
        {
            ContextMenuExtensions.CreateContextMenu(this, chooseImageButton);
        }

        private void DatePickerButton_Click(object sender, EventArgs e)
        {
            var datePickerDialog = CalendarExtensions.GetDatePickerDialog(this, "Выберите дату", OnDateSet, OnDateChange, CalendarSetup);
            datePickerDialog.Show();
        }      

        private string GetGoalNameText()
        {
            var text = goalNameEditText.Text;
            return string.IsNullOrEmpty(text)
                ? "Новая цель"
                : text;
        }

        private void OnDateChange(object sender, CalendarView.DateChangeEventArgs args)
        {
            var selectedDate = new DateTime(args.Year, args.Month + 1, args.DayOfMonth);
            this.selectedDate = selectedDate;
        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            var selectedDate = GetGoalDateTime();
            if (!GoalExtensions.IsValidDeadline(selectedDate))
            {
                Toast.MakeText(this, "Неверный дедлайн", ToastLength.Short).Show();
                return;
            }
            var year = selectedDate.Year;
            var month = selectedDate.Month;
            var dayOfMonth = selectedDate.Day;
            var selectedDateInString
                = $"{string.Format("{0:00}", dayOfMonth)}.{string.Format("{0:00}", month)}.{year}";
            Toast.MakeText(this, $"Выбранная дата: {selectedDateInString}", ToastLength.Short).Show();
        }
    }
}

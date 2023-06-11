using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Activity;
using AndroidX.AppCompat.App;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using Java.Util.Zip;
using Json.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace BucketList
{
    [Activity(Label = "AddGoalActivity")]
    public class AddGoalActivity : Activity
    {
        public static readonly int PickImageId = 1000;

        Button goalAddButton;
        EditText goalAddNameEditText;
        ImageView snake;
        DatePickerDialog datePickerDialog;

        Button chooseImageButton;
        ImageView goalImage;
        string imagePath;

        private CalendarView calendarView;
        private DateTime selectedDate;

        private List<Goal> allGoals;
        private string maxDeadline;
        private string currentGoal;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_goal);
            goalAddButton = FindViewById<Button>(Resource.Id.goal_add_button);
            goalAddButton.Click += OnClick;
            allGoals = Extensions.GetSavedGoals();
            goalAddNameEditText = FindViewById<EditText>(Resource.Id.goal_add_name_text);
            chooseImageButton = FindViewById<Button>(Resource.Id.goal_add_choose_image_button);
            goalImage = FindViewById<ImageView>(Resource.Id.goal_add_image);

            chooseImageButton.Click += ChooseImageButton_Click;
            
            var datePickerButton = FindViewById<Button>(Resource.Id.button1);
            datePickerButton.Click += DatePickerButton_Click;
        }

        private void ChooseImageButton_Click(object sender, EventArgs e)
        {
            ShowContextMenu(sender, e);
        }

        private void ShowContextMenu(object sender, EventArgs e)
        {
            var myListView = sender as Button;

            RegisterForContextMenu(myListView);

            OpenContextMenu(myListView);
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            // Установите заголовок контекстного меню
            menu.SetHeaderTitle("Загрузить картинку?");

            // Добавьте пункт меню для удаления элемента
            menu.Add(Menu.None, 0, Menu.None, "Да");
            menu.Add(Menu.None, 1, Menu.None, "Нет");
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            if (item.ItemId == 0)
            {
                UploadPicture();
            }

            return base.OnContextItemSelected(item);
        }

        private void UploadPicture()
        {
            var intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), PickImageId);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;

                var imagePath = CopyImageToAppFiles(uri);
                this.imagePath = imagePath;

                if (!string.IsNullOrEmpty(imagePath))
                {
                    goalImage.SetImageURI(Android.Net.Uri.FromFile(new Java.IO.File(imagePath)));
                }
            }
        }

        private string CopyImageToAppFiles(Android.Net.Uri sourceUri)
        {
            try
            {
                var sourceStream = ContentResolver.OpenInputStream(sourceUri);

                // Генерация уникального имени файла для копии изображения
                string fileName = $"image_{DateTime.Now.Ticks}.jpg";
                string destinationPath = Path.Combine(FilesDir.AbsolutePath, fileName);

                using (var destinationStream = new FileStream(destinationPath, FileMode.Create))
                {
                    sourceStream.CopyTo(destinationStream);
                }

                return destinationPath;
            }
            catch (Exception ex)
            {
                // Обработка ошибок копирования изображения
                Console.WriteLine("Error copying image: " + ex.Message);
                return null;
            }
        }

        private void OnClick(object sender, EventArgs e)
        {
            var intent = new Intent();
            var goalName = GetGoalNameText();
            var goalDeadline = GetGoalDateTime();
            if (allGoals.Any(x => x.GoalName == goalName))
            {
                Toast.MakeText(this, "Цель с таким названием уже существует", ToastLength.Short).Show();
                return;
            }
            if (DateTime.Compare(GetGoalDateTime(), DateTime.Now) <= 0)
            {
                Toast.MakeText(this, "Вы не выбрали дедлайн", ToastLength.Short).Show();
                return;
            }
            var goal = new Goal(goalName, goalDeadline, imagePath);
            var serializedGoal = Extensions.SerializeGoal(goal);
            intent.PutExtra("goal", serializedGoal);
            SetResult(Result.Ok, intent);
            Finish();
        }

        public override void OnBackPressed()
        {
            Extensions.DeleteImage(imagePath);
            base.OnBackPressed();
        }

        private string GetGoalNameText()
        {
            var text = goalAddNameEditText.Text;
            if (text == "") return "Новая цель";
            return text;
        }

        private DateTime GetGoalDateTime()
        {
            return selectedDate;
        }

        private void DatePickerButton_Click(object sender, EventArgs e)
        {
            var currentDate = DateTime.Now;

            LayoutInflater inflater = LayoutInflater.From(this);
            View datePickerView = inflater.Inflate(Resource.Layout.calendar, null);
            CalendarView calendarView = datePickerView.FindViewById<CalendarView>(Resource.Id.addGoalCalendar);
            this.calendarView = calendarView;
            DatePickerDialog datePickerDialog = new DatePickerDialog(
                                                this,
                                                OnDateSet,
                                                currentDate.Year,
                                                currentDate.Month - 1,
                                                currentDate.Day
                                                );
            datePickerDialog.SetView(datePickerView);
            calendarView.MinDate = currentDate.GetDateTimeInMillis();
            //if (!string.IsNullOrEmpty(maxDeadline))
            //    calendarView.MaxDate = DateTime.Parse(maxDeadline).GetDateTimeInMillis();
            calendarView.FirstDayOfWeek = 2;
            calendarView.DateChange += CalendarView_DateChange;
            datePickerDialog.Show();

            //datePickerDialog
            //    = new DatePickerDialog(
            //                        this,
            //                        OnDateSet,
            //                        currentDate.Year,
            //                        currentDate.Month - 1,
            //                        currentDate.Day
            //                        );
            //datePickerDialog.DatePicker.MinDate = DateTime.Now.GetDateTimeInMillis();
            //datePickerDialog.SetContentView(Resource.Layout.calendar);
            //datePickerDialog.Show();

            //LayoutInflater inflater = LayoutInflater.From(this);
            //View datePickerView = inflater.Inflate(Resource.Layout.calendar, null);
            //DatePickerDialog datePickerDialog = new DatePickerDialog(
            //                                    this,
            //                                    OnDateSet,
            //                                    currentDate.Year,
            //                                    currentDate.Month - 1,
            //                                    currentDate.Day
            //                                    );
            //datePickerDialog.DatePicker.MinDate = DateTime.Now.GetDateTimeInMillis();
            //datePickerDialog.SetView(datePickerView);
            //datePickerDialog.Show();

        }

        private void CalendarView_DateChange(object sender, CalendarView.DateChangeEventArgs args)
        {
            var selectedDate = new DateTime(args.Year, args.Month + 1, args.DayOfMonth);
            this.selectedDate = selectedDate;
        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            var year = selectedDate.Year;
            var month = selectedDate.Month; // Здесь месяц начинается с 0, поэтому добавляем 1
            var dayOfMonth = selectedDate.Day;
            var selectedDateInString
                = $"{string.Format("{0:00}", dayOfMonth)}.{string.Format("{0:00}", month)}.{year}";
            Toast.MakeText(this, $"Выбранная дата: {selectedDateInString}", ToastLength.Short).Show();
            //var calendarView = datePicker.RootView.FindViewById<CalendarView>(Resource.Id.addGoalCalendar);
            //var selectedDate = calendarView.Date.GetDateTimeFromMillis();
            //var year = selectedDate.Year;
            //var month = selectedDate.Month + 1; // Здесь месяц начинается с 0, поэтому добавляем 1
            //var dayOfMonth = selectedDate.Day;
            //var selectedDateInString = $"{dayOfMonth}-{month}-{year}";
            //Toast.MakeText(this, $"Выбранная дата: {selectedDateInString}", ToastLength.Short).Show();


            //// Получите выбранную дату из DatePickerDialog
            //int year = e.Year;
            //int month = e.Month + 1; // Здесь месяц начинается с 0, поэтому добавляем 1
            //int dayOfMonth = e.DayOfMonth;


            //// Выполните необходимые действия с выбранной датой
            //string selectedDate = $"{dayOfMonth}-{month}-{year}";
            //Toast.MakeText(this, $"Выбранная дата: {selectedDate}", ToastLength.Short).Show();
        }
    }
}

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Android.Telephony.CarrierConfigManager;

namespace BucketList
{
    [Activity(Label = "RegistrationActivity", MainLauncher = true)]
    public class RegistrationActivity : Activity
    {
        EditText username;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_registration);
            username = FindViewById<EditText>(Resource.Id.registration_add_name_edittext);

            var name = GetUserName();
            if (!string.IsNullOrEmpty(name))
            {
                StartActivity(typeof(MainActivity));
            }
            else
            {
                var button = FindViewById<Button>(Resource.Id.registration_button);
                button.Click += OnClick;
            }
        }

        public override void OnBackPressed()
        {
            
        }

        private void OnClick(object sender, EventArgs eventHandler)
        {
            var name = username.Text;
            if (!string.IsNullOrEmpty(name))
            {
                WriteUserName(name);
                StartActivity(typeof(MainActivity));
            }
            else
            {
                Toast.MakeText(this, "Пожалуйста, введите ваше имя", ToastLength.Short).Show();
            }
        }

        private string GetUserName()
        {
            try
            {
                string internalStoragePath = Application.Context.FilesDir.AbsolutePath;
                string filePath = Path.Combine(internalStoragePath, "myfile.txt");
                var result = "";
                using (StreamReader writer = new StreamReader(filePath))
                {
                    result = writer.ReadLine();
                }
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private void WriteUserName(string data)
        {
            // Получаем путь к внутреннему хранилищу приложения
            string internalStoragePath = Application.Context.FilesDir.AbsolutePath;

            // Создаем путь к файлу и файловый поток для записи
            string filePath = Path.Combine(internalStoragePath, "myfile.txt");
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Записываем данные в файл
                writer.WriteLine(data);
            }
        }
    }
}
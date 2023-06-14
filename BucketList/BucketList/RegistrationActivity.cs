using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Json.Net;
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
        public static readonly int PickImageId = 1000;

        EditText username;
        private ImageView imageView;
        private string imagePath;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_registration);
            username = FindViewById<EditText>(Resource.Id.registration_add_name_edittext);

            var name = Extensions.ReadUser();
            //var name = "";

            if (!string.IsNullOrEmpty(name))
            {
                var intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("username", "outdated");
                StartActivity(intent);
            }
            else
            {
                var button = FindViewById<Button>(Resource.Id.registration_button);
                button.Click += OnClick;
                imageView = FindViewById<ImageView>(Resource.Id.registration_add_user_photo);
                imageView.Click += Image_OnClick;
            }
        }

        private void Image_OnClick(object sender, EventArgs e)
        {
            ShowContextMenu(sender, e);
        }

        private void ShowContextMenu(object sender, EventArgs e)
        {
            var myImageView = sender as ImageView;

            RegisterForContextMenu(myImageView);

            OpenContextMenu(myImageView);
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
                    imageView.SetImageURI(Android.Net.Uri.FromFile(new Java.IO.File(imagePath)));
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

        public override void OnBackPressed()
        {
            
        }

        private void OnClick(object sender, EventArgs eventHandler)
        {
            var name = username.Text;
            if (!string.IsNullOrEmpty(name))
            {
                var user = new User(name, imagePath);
                var userSerialied = Extensions.SerializeUser(user);
                Extensions.OverwriteUser(userSerialied);
                var intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("username", "outdated");
                StartActivity(intent);
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
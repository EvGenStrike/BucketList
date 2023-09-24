using Android.App;
using Android.Content;
using Android.Widget;
using System;
using System.IO;

namespace BucketList
{
    public static class ImageExtensions
    {
        public static void SetImage(this ImageView imageView, string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                imageView.SetImageURI(Android.Net.Uri.FromFile(new Java.IO.File(imagePath)));
            }
        }

        /// <summary>
        /// Удаляет изображение из файлов приложения
        /// </summary>
        /// <param name="imagePath"></param>
        public static void DeleteImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    File.Delete(imagePath);
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Открывает галерею, где пользователь должен выбрать картинку
        /// </summary>
        /// <param name="activity">Из какой Activity вызвать этот метод (обычно передаётся this - с текущей activity)</param>
        public static void UploadImage(Activity activity)
        {
            var intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            activity.StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), Constants.PickImageRequestCode);
        }

        /// <summary>
        /// Копирует переданное изображение в файлы приложения
        /// </summary>
        /// <param name="activity">Activity, из которой будет сгенерирован поток данных (обычно передаётся this - с текущей activity)</param>
        /// <param name="sourceUri">URI ссылка на изображение</param>
        /// <returns>Путь до скопированного изображения</returns>
        public static string CopyImageToAppFiles(Activity activity, Android.Net.Uri sourceUri)
        {
            try
            {
                var sourceStream = activity.ContentResolver.OpenInputStream(sourceUri);
                var fileName = $"image_{DateTime.Now.Ticks}.jpg";
                var destinationPath = Path.Combine(Application.Context.FilesDir.AbsolutePath, fileName);

                using (var destinationStream = new FileStream(destinationPath, FileMode.Create))
                {
                    sourceStream.CopyTo(destinationStream);
                }

                return destinationPath;
            }
            catch (Exception exception)
            {
                Toast.MakeText(activity, $"Error copying image:\n{exception.Message}", ToastLength.Short).Show();
                return null;
            }
        }
    }
}
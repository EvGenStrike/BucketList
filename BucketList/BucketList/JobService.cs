using Android.App.Job;
using Android.App;
using static Java.Util.Jar.Attributes;
using Android.OS;
using AndroidX.Core.App;

namespace BucketList
{
    [Service(Name = "com.companyname.bucketlist.MyJobService", Permission = "android.permission.BIND_JOB_SERVICE")]
    public class MyJobService : JobService
    {
        public override bool OnStartJob(JobParameters parameters)
        {
            // Логика показа уведомления
            ShowNotification("Загол123123овок", "Текст уведомления");

            // Важно вызвать завершение работы задачи
            JobFinished(parameters, false);

            return false;
        }

        public override bool OnStopJob(JobParameters parameters)
        {
            return false;
        }

        private void ShowNotification(string title, string message)
        {
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                // Создание канала уведомлений
                var channelId = "channel_id";
                var channelName = "Channel Name";
                var channelDescription = "Channel Description";
                var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Default)
                {
                    Description = channelDescription
                };
                notificationManager.CreateNotificationChannel(channel);
            }

            // Создание уведомления
            var notificationBuilder = new NotificationCompat.Builder(this, "channel_id")
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Resource.Drawable.snake_image);

            // Показ уведомления
            var notificationId = 1; // Уникальный идентификатор уведомления
            notificationManager.Notify(notificationId, notificationBuilder.Build());
        }
    }
}
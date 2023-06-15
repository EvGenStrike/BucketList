using Android.App;
using Android.Content;
using Android.OS;
using Android.Service.Notification;
using Android.Support.V4.App;
using AndroidX.Core.App;
using Java.Lang;
using System;

namespace BucketList
{
    [Service]
    public class NotificationService : Service
    {
        private const int NotificationId = 1;
        private const string ChannelId = "my_channel_id";

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var desiredNotificationTime = intent.GetLongExtra("notificationTime", 0);
            var title = intent.GetStringExtra("title");
            var message = intent.GetStringExtra("message");
            var currentTimeMillis = JavaSystem.CurrentTimeMillis();
            
            //var notificationTime = currentTimeMillis + (15 * 1000); // Текущее время + 15 секунд
            var notificationTime = desiredNotificationTime - DateTime.Now.GetDateTimeInMillis();

            // Показываем уведомление через минуту
            ScheduleNotification(notificationTime, title, message);

            #region С фоновой службой (тогда даже при закрытом приложении работает)
            //// Создаем PendingIntent для запуска службы в фоновом режиме
            //var notificationIntent = new Intent(this, typeof(MainActivity));
            //var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);

            //// Создаем уведомление для статусной строки
            //var notificationBuilder = new NotificationCompat.Builder(this, ChannelId)
            //    .SetContentTitle("Уведомление")
            //    .SetContentText("Служба работает в фоновом режиме")
            //    .SetSmallIcon(Resource.Drawable.snake_image)
            //    .SetContentIntent(pendingIntent)
            //    .SetAutoCancel(true);

            //if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            //{
            //    var channel = new NotificationChannel(ChannelId, "My Channel", NotificationImportance.Default);
            //    channel.Description = "My Channel Description";
            //    var notificationManager = GetSystemService(NotificationService) as NotificationManager;
            //    notificationManager.CreateNotificationChannel(channel);
            //}

            //var notification = notificationBuilder.Build();

            //// Запускаем службу в фоновом режиме с помощью StartForeground
            //StartForeground(NotificationId, notification);
            #endregion

            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void ScheduleNotification(long notificationTime, string title, string description)
        {
            var alarmManager = (AlarmManager)GetSystemService(AlarmService);
            var notificationIntent = new Intent(this, typeof(NotificationReceiver));
            notificationIntent.PutExtra("title", title);
            notificationIntent.PutExtra("message", description);
            var pendingIntent = PendingIntent.GetBroadcast(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, notificationTime, pendingIntent);
        }
    }
}

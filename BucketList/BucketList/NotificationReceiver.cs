using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using AndroidX.Core.App;

namespace BucketList
{
    [BroadcastReceiver(Enabled = true)]
    public class NotificationReceiver : BroadcastReceiver
    {
        private const int NotificationId = 1;
        private const string ChannelId = "my_channel_id";

        public override void OnReceive(Context context, Intent intent)
        {
            // Извлекаем данные из интента, если необходимо
            // var data = intent.GetStringExtra("key");

            // Создаем PendingIntent для запуска приложения при нажатии на уведомление
            var notificationIntent = new Intent(context, typeof(MainActivity));
            notificationIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            var pendingIntent = PendingIntent.GetActivity(context, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);

            // Создаем уведомление
            var title = intent.GetStringExtra("title");
            var message = intent.GetStringExtra("message");
            var notificationBuilder = new NotificationCompat.Builder(context, ChannelId)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Resource.Drawable.snake_image)
                .SetContentIntent(pendingIntent)
                .SetAutoCancel(true)
                .SetPriority(NotificationCompat.PriorityDefault);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(ChannelId, "My Channel", NotificationImportance.Default);
                channel.Description = "My Channel Description";
                var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
                notificationManager.CreateNotificationChannel(channel);
            }

            var notification = notificationBuilder.Build();

            // Отображаем уведомление
            var notificationManagerCompat = NotificationManagerCompat.From(context);
            notificationManagerCompat.Notify(NotificationId, notification);
        }
    }
}

using Android.App;
using Android.Content;
using Android.OS;
using Android.Service.Notification;
using Android.Support.V4.App;

namespace BucketList
{
    [Service]
    public class NotificationService : NotificationListenerService
    {
        public override void OnNotificationPosted(StatusBarNotification notification)
        {
            // Обработка события при появлении нового уведомления
        }

        public override void OnNotificationRemoved(StatusBarNotification notification)
        {
            // Обработка события при удалении уведомления
        }

        public override IBinder OnBind(Intent intent)
        {
            return base.OnBind(intent);
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // Отправка уведомления

            return StartCommandResult.Sticky;
        }
    }
}
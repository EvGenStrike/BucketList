using Android.App;
using Android.OS;
using Android.Widget;

namespace BucketList
{
    [Activity(Label = "StatisticsActivity")]
    public class StatisticsActivity : Activity
    {
        private User user;

        ImageView backArrow;
        ImageView userPhoto;

        TextView totalCreatedGoals;
        TextView doneGoals;
        TextView failedGoals;
        TextView deletedGoals;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_statistics_screen);
            SetUser();
            SetBackArrow();
            SetUserPhoto();
            SetStatistics();
        }

        private void SetStatistics()
        {
            totalCreatedGoals = FindViewById<TextView>(Resource.Id.statistics_screen_total_goals_created_count);
            doneGoals = FindViewById<TextView>(Resource.Id.statistics_screen_done_goals_created_count);
            failedGoals = FindViewById<TextView>(Resource.Id.statistics_screen_failed_goals_created_count);
            deletedGoals = FindViewById<TextView>(Resource.Id.statistics_screen_deleted_goals_created_count);

            totalCreatedGoals.Text = user.UserStatistics.GoalsCreatedCount.ToString();
            doneGoals.Text = user.UserStatistics.GoalsDoneCount.ToString();
            failedGoals.Text = user.UserStatistics.GoalsFailedCount.ToString();
            deletedGoals.Text = user.UserStatistics.GoalsDeletedCount.ToString();
        }

        private void SetUser()
        {
            user = SaveExtensions.GetSavedUser();
        }

        private void SetUserPhoto()
        {

            var photoPath = SaveExtensions.GetSavedUser().UserPhotoPath;
            userPhoto = FindViewById<ImageView>(Resource.Id.statistics_screen_user_photo);
            userPhoto.SetImage(photoPath);
        }

        private void SetBackArrow()
        {
            backArrow = FindViewById<ImageView>(Resource.Id.statistics_screen_back_arrow);
            backArrow.Click += (sender, e) => { OnBackPressed(); };
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}
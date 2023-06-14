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

            totalCreatedGoals.Text = user.UserStatistics.
        }

        private void SetUser()
        {
            user = Extensions.GetSavedUser();
        }

        private void SetUserPhoto()
        {

            var photoPath = Extensions.GetSavedUser().UserPhotoPath;
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
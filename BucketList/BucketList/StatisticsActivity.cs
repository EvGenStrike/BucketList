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
        ImageView backArrow;
        ImageView userPhoto;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_statistics_screen);
            SetBackArrow();
            SetUserPhoto();
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
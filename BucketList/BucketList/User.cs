using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BucketList
{
    public class User
    {
        public string UserName { get; set; }
        public string UserPhotoPath { get; set; }
        public Statistics UserStatistics { get; set; }
        public DateTime UserRegistrationDate { get; set; }
        public bool UserAllowNotifications { get; set; }
        public PythonSettings UserPythonSettings { get; set; }

        public User()
        {
            //UserStatistics = new Statistics();
            //UserPythonSettings = new PythonSettings();
        }

        public User(string userName, string userPhotoPath = null)
        {
            UserName = userName;
            UserPhotoPath = userPhotoPath;
            UserStatistics = new Statistics();
            UserPythonSettings = new PythonSettings();
            UserRegistrationDate = DateTime.Now;
        }
    }
}
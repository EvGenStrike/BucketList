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

        public User() { }

        public User(string userName, string userPhotoPath = null)
        {
            UserName = userName;
            UserPhotoPath = userPhotoPath;
        }
    }
}
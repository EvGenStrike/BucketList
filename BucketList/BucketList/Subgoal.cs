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
    public class Subgoal
    {
        public int Id { get; set; }
        public string SubgoalName { get; set; }
        public string Deadline { get; set; }
        
    }
}
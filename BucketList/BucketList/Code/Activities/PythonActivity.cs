using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BucketList
{
    [Activity(Label = "PythonActivity")]
    public class PythonActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.python_screen);
            var progBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            var countDoneGoalsText = FindViewById<TextView>(Resource.Id.pythonDeadlineCountText);

            var countDoneGoals = SaveExtensions.GetSavedUser().UserStatistics.GoalsDoneCount;
            // Увеличить прогресс на заданную величину
            progBar.Progress = countDoneGoals;
            countDoneGoalsText.Text = countDoneGoals.ToString();


            // Create your application here
        }
        public override void OnBackPressed()
        {
                base.OnBackPressed();
        }
    }
}
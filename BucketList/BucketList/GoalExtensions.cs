using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Json.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BucketList
{
    public static class GoalExtensions
    {
        public static void WriteTextFile(string fileName, string text)
        {
            string internalStoragePath = Application.Context.FilesDir.AbsolutePath;
            string filePath = Path.Combine(internalStoragePath, fileName);
            File.WriteAllText(filePath, text);
        }

        public static string ReadTextFile(string fileName)
        {
            string internalStoragePath = Application.Context.FilesDir.AbsolutePath;
            string filePath = Path.Combine(internalStoragePath, fileName);
            try
            {
                var result = "";
                using (StreamReader writer = new StreamReader(filePath))
                {
                    result = writer.ReadLine();
                }
                return result;
            }

            catch (Exception ex)
            {
                return "";
            }
        }

        public static string ReadGoals()
        {
            var data = GoalExtensions.ReadTextFile("goals.txt");
            return data;
        }

        public static void OverwriteGoals(string data)
        {
            GoalExtensions.WriteTextFile("goals.txt", data);
        }

        public static void OverwriteGoals(List<Goal> goals)
        {
            var data = GoalExtensions.SerializeGoals(goals);
            GoalExtensions.WriteTextFile("goals.txt", data);
        }

        public static string SerializeGoals(List<Goal> goals)
        {
            return JsonNet.Serialize(goals);
        }

        public static List<Goal> DeserializeGoals(string goals)
        {
            return JsonNet.Deserialize<List<Goal>>(goals);
        }

        public static List<Goal> GetSavedGoals()
        {
            return GoalExtensions.DeserializeGoals(GoalExtensions.ReadGoals());
        }


        public static string SerializeGoal(Goal goal)
        {
            return JsonNet.Serialize(goal);
        }

        public static Goal DeserializeGoal(string goal)
        {
            return JsonNet.Deserialize<Goal>(goal);
        }

        public static string SerializeSubgoal(Subgoal goal)
        {
            return JsonNet.Serialize(goal);
        }

        public static Subgoal DeserializeSubgoal(string goal)
        {
            return JsonNet.Deserialize<Subgoal>(goal);
        }
    }
}
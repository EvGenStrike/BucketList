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
    public static class SaveExtensions
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

        public static void ClearSaves()
        {
            OverwriteUser("");
            OverwriteGoals(new List<Goal>());
        }

        public static string ReadUser()
        {
            var data = ReadTextFile("user.txt");
            return data;
        }

        public static void OverwriteUser(string data)
        {
            WriteTextFile("user.txt", data);
        }

        public static void OverwriteUser(User user)
        {
            WriteTextFile("user.txt", SerializeUser(user));
        }

        public static string SerializeUser(User user)
        {
            return JsonNet.Serialize(user);
        }

        public static User DeserializeUser(string user)
        {
            return JsonNet.Deserialize<User>(user);
        }

        public static User GetSavedUser()
        {
            return DeserializeUser(ReadUser());
        }

        public static string ReadGoals()
        {
            var data = ReadTextFile("goals.txt");
            return data;
        }

        public static void OverwriteGoals(string data)
        {
            WriteTextFile("goals.txt", data);
        }

        public static void OverwriteGoals(List<Goal> goals)
        {
            var data = SerializeGoals(goals);
            WriteTextFile("goals.txt", data);
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
            return DeserializeGoals(ReadGoals());
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
        public static string SerializeSubgoals(List<Subgoal> subGoals)
        {
            return JsonNet.Serialize(subGoals);
        }

        public static Subgoal DeserializeSubgoal(string goal)
        {
            return JsonNet.Deserialize<Subgoal>(goal);
        }

        public static List<Subgoal> DeserializeSubgoals(string goals)
        {
            return JsonNet.Deserialize<List<Subgoal>>(goals);
        }
    }
}
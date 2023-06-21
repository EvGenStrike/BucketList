using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Util;
using BucketList.Code.Enums;
using System;

namespace BucketList.Code.Extensions
{
    public static class ContextMenuExtensions
    {
        public static void AddOption(this IContextMenu menu, int itemID, string text)
        {
            menu.Add(Menu.None, itemID, Menu.None, text);
        }

        public static void AddOption(this IContextMenu menu, ContextMenuOptions itemID)
        {
            var text = itemID switch
            {
                ContextMenuOptions.Yes => "Да",
                ContextMenuOptions.No => "Нет",
                _ => throw new ArgumentOutOfRangeException(),
            };
            AddOption(menu, itemID, text);
        }

        public static void AddOption(this IContextMenu menu, ContextMenuOptions itemID, string text)
        {
            AddOption(menu, (int)itemID, text);
        }

        public static void CreateContextMenu(this Activity activity, View? view)
        {
            activity.RegisterForContextMenu(view);
            activity.OpenContextMenu(view);
        }
    }
}
using Android.App;
using Android.Views;
using System;

namespace BucketList
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

        /// <summary>
        /// Создаёт контекстное меню
        /// </summary>
        /// <param name="activity">Из какой Activity вызвать этот метод (обычно передаётся this - с текущей activity)</param>
        /// <param name="view">Для какой View создаётся контекстное меню</param>
        public static void CreateContextMenu(Activity activity, View view)
        {
            activity.RegisterForContextMenu(view);
            activity.OpenContextMenu(view);
        }
    }
}
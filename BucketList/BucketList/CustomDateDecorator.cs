using System;
using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using Java.Util;

namespace BucketList
{
    public class CustomCalendarView : CalendarView
    {
        private HashSet<DateTime> highlightedDates;
        private Android.Graphics.Color highlightColor;

        public CustomCalendarView(Context context) : base(context)
        {
            Initialize();
        }

        public CustomCalendarView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();

            // Получение пользовательских атрибутов из XML
            TypedArray attributes = context.ObtainStyledAttributes(attrs, Resource.Styleable.CustomCalendarView);
            int highlightColorResource = attributes.GetResourceId(Resource.Styleable.CustomCalendarView_highlightColor, 0);
            if (highlightColorResource != 0)
            {
                highlightColor = new Android.Graphics.Color(Context.GetColor(highlightColorResource));
            }
            attributes.Recycle();
        }

        private void Initialize()
        {
            highlightedDates = new HashSet<DateTime>();
            highlightColor = Android.Graphics.Color.Red;

            // Установка слушателя событий выбора даты
            DateChange += OnDateChange;
        }

        public void SetHighlightedDates(HashSet<DateTime> dates)
        {
            highlightedDates = dates;

            // Обновление внешнего вида календаря
            Invalidate();
        }

        private void OnDateChange(object sender, DateChangeEventArgs e)
        {
            // Обработка события выбора даты
            //sDateTime selectedDate = e.mo;
            // ... ваш код обработки выбранной даты ...
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            // Выделение выделенных дат цветом
            var paint = new Paint();
            paint.Color = highlightColor;
            paint.SetStyle(Paint.Style.Fill);

            int rowCount = 6; // Количество строк в календаре
            int columnCount = 7; // Количество столбцов в календаре

            int cellWidth = Width / columnCount;
            int cellHeight = Height / rowCount;

            foreach (DateTime date in highlightedDates)
            {
                int day = date.Day;
                int month = date.Month - 1; // Нумерация месяцев начинается с 0

                // Вычисление координат ячейки даты
                int left = (day - 1) * cellWidth;
                int top = month * cellHeight;
                int right = left + cellWidth;
                int bottom = top + cellHeight;

                Rect dayRect = new Rect(left, top, right, bottom);

                canvas.DrawRect(dayRect, paint);
            }
        }


    }
}
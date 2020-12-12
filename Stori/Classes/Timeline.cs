using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stori.Classes
{
    class Timeline
    {
        public TimeSystem timeSystem;
        private ZoomLevel currentZoomLevel;
        public int idealTicksPerPage;

        //public List<CustomDateTime> pageStartDates = new List<CustomDateTime>();
        //public List<CustomDateTime> pageEndDates = new List<CustomDateTime>();
        //public List<string> pageLabels = new List<string>();

        public Timeline(
            TimeSystem timeSystem,
            ZoomLevel currentZoomLevel = ZoomLevel.MonthDay,
            int ticksPerPage = 800)
        {
            this.timeSystem = timeSystem;
            this.currentZoomLevel = currentZoomLevel;
            this.idealTicksPerPage = ticksPerPage;

            //AssignPagesForZoomLevel();
        }

        public enum ZoomLevel
        {
            Auto,
            MinuteSecond,
            HourMinute,
            DayHour,
            MonthDay,
            YearMonth
        }

        public void IterateTickCustomDateTime(CustomDateTime currentDateTime, int count = 1)
        {
            switch (currentZoomLevel)
            {
                case ZoomLevel.MinuteSecond:
                    currentDateTime.AddSeconds(count);
                    break;
                case ZoomLevel.HourMinute:
                    currentDateTime.AddMinutes(count);
                    break;
                case ZoomLevel.DayHour:
                    currentDateTime.AddHours(count);
                    break;
                case ZoomLevel.MonthDay:
                    currentDateTime.AddDays(count);
                    break;
                case ZoomLevel.YearMonth:
                    currentDateTime.AddMonths(count);
                    break;
                default:
                    currentDateTime.AddYears(count);
                    break;
            }
        }

        public bool IsBottomTickRequired(CustomDateTime date)
        {
            switch (currentZoomLevel)
            {
                case ZoomLevel.MinuteSecond:
                    return date.second == 0;
                case ZoomLevel.HourMinute:
                    return date.minute == 0;
                case ZoomLevel.DayHour:
                    return date.hour == 0;
                case ZoomLevel.MonthDay:
                    return date.day == 1;
                case ZoomLevel.YearMonth:
                    return date.month == 1;
                default:
                    return date.year % 10 == 0;
            }
        }

        public string GetTopTickLabel(CustomDateTime date)
        {
            switch (currentZoomLevel)
            {
                case ZoomLevel.MinuteSecond:
                    return "\n" + date.second.ToString("00");
                case ZoomLevel.HourMinute:
                    return "\n" + date.hour.ToString() + ":" + date.minute.ToString("00");
                case ZoomLevel.DayHour:
                    return "\n" + date.hour.ToString() + ":00";
                case ZoomLevel.MonthDay:
                    return date.day.ToString() + "\n(" + date.GetDayName().Substring(0, 3) + ")";
                case ZoomLevel.YearMonth:
                    return "\n" + date.GetMonthName().Substring(0, 3);
                default:
                    return "\n" + date.year.ToString();
            }
        }

        public string GetBottomTickLabel(CustomDateTime date)
        {
            switch (currentZoomLevel)
            {
                case ZoomLevel.MinuteSecond:
                    return date.hour.ToString("00") + ":" + date.minute.ToString("00")
                        + "\n" + date.GetDayName() + ",\n" + date.day.ToString() + " " + date.GetMonthName() + "\n" + date.year.ToString("0000");
                case ZoomLevel.HourMinute:
                    return date.hour.ToString("00") + ":00\n" + date.day.ToString() + " " + date.GetMonthName().Substring(0,3) + " " + date.year.ToString();
                case ZoomLevel.DayHour:
                    return date.GetDayName() + ",\n" + date.day.ToString() + " " + date.GetMonthName() + "\n" + date.year.ToString("0000");
                case ZoomLevel.MonthDay:
                    return date.GetMonthName()
                        + "\n"
                        + date.year.ToString("0000");
                case ZoomLevel.YearMonth:
                    return date.year.ToString("0000");
                default:
                    return date.year.ToString("0000") + "'s";
            }
        }

        public int GetEventWidth(Event myEvent, int tickWidth, int startAndEndPadding)
        {
            int eventWidth = startAndEndPadding;
            CustomDateTime refDateTime = myEvent.startDateTime.GetDateTime();
            while (refDateTime < myEvent.endDateTime)
            {
                IterateTickCustomDateTime(refDateTime);
                if (refDateTime <= myEvent.endDateTime)
                {
                    eventWidth += tickWidth;
                }
            }
            eventWidth += startAndEndPadding;
            return eventWidth;
        }

        public void SetZoomLevel(ZoomLevel zoomLevel)
        {
            this.currentZoomLevel = zoomLevel;
            //AssignPagesForZoomLevel();
        }

        public bool ZoomIn()
        {
            switch (this.currentZoomLevel) 
            {
                case ZoomLevel.MinuteSecond:
                    return false;
                case ZoomLevel.HourMinute:
                    return false;
                    //this.SetZoomLevel(ZoomLevel.MinuteSecond);
                    //break;
                case ZoomLevel.DayHour:
                    return false;
                    //this.SetZoomLevel(ZoomLevel.HourMinute);
                    //break;
                case ZoomLevel.MonthDay:
                    this.SetZoomLevel(ZoomLevel.DayHour);
                    break;
                default:
                    this.SetZoomLevel(ZoomLevel.MonthDay);
                    break;
            }
            return true;
        }

        public bool ZoomOut()
        {
            switch (this.currentZoomLevel)
            {
                case ZoomLevel.MinuteSecond:
                    this.SetZoomLevel(ZoomLevel.HourMinute);
                    break;
                case ZoomLevel.HourMinute:
                    this.SetZoomLevel(ZoomLevel.DayHour);
                    break;
                case ZoomLevel.DayHour:
                    this.SetZoomLevel(ZoomLevel.MonthDay);
                    break;
                case ZoomLevel.MonthDay:
                    this.SetZoomLevel(ZoomLevel.YearMonth);
                    break;
                default:
                    return false;
            }
            return true;
        }

    }
}

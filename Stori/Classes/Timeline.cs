using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stori.Classes
{
    class Timeline
    {
        public CustomDateTime startDateTime, endDateTime;
        public List<Event> events;
        public TimeSystem timeSystem;
        public ZoomLevel defaultZoomLevel;
        private ZoomLevel currentZoomLevel;
        private int ticksPerPage;

        public List<CustomDateTime> pageStartDates = new List<CustomDateTime>();
        public List<CustomDateTime> pageEndDates = new List<CustomDateTime>();
        public List<string> pageLabels = new List<string>();

        public Timeline(
            CustomDateTime startDateTime,
            CustomDateTime endDateTime,
            int firstDayNameIndex,
            TimeSystem timeSystem,
            List<Event> events = null, 
            ZoomLevel defaultZoomLevel = ZoomLevel.DayHour,
            ZoomLevel currentZoomLevel = ZoomLevel.DayHour,
            int ticksPerPage = 800)
        {
            this.startDateTime = startDateTime;
            this.endDateTime = endDateTime;
            this.timeSystem = timeSystem;
            this.events = events;
            this.defaultZoomLevel = defaultZoomLevel;
            this.currentZoomLevel = currentZoomLevel;
            this.ticksPerPage = ticksPerPage;

            AssignPagesForZoomLevel();
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

        public void IterateTickCustomDateTime(CustomDateTime currentDateTime)
        {
            switch (currentZoomLevel)
            {
                case ZoomLevel.MinuteSecond:
                    currentDateTime.AddSeconds(1);
                    break;
                case ZoomLevel.HourMinute:
                    currentDateTime.AddMinutes(1);
                    break;
                case ZoomLevel.DayHour:
                    currentDateTime.AddHours(1);
                    break;
                case ZoomLevel.MonthDay:
                    currentDateTime.AddDays(1);
                    break;
                case ZoomLevel.YearMonth:
                    currentDateTime.AddMonths(1);
                    break;
                default:
                    currentDateTime.AddYears(1);
                    break;
            }
        }

        public bool IsBottomTickRequired(CustomDateTime date)
        {
            switch (currentZoomLevel)
            {
                case ZoomLevel.MinuteSecond:
                    return (date.second == 0);
                case ZoomLevel.HourMinute:
                    return (date.minute == 0);
                case ZoomLevel.DayHour:
                    return (date.hour == 0);
                case ZoomLevel.MonthDay:
                    return (date.day == 0);
                case ZoomLevel.YearMonth:
                    return (date.month) == 0;
                default:
                    return (date.year % 10 == 0);
            }
        }

        public string GetTopTickLabel(CustomDateTime date)
        {
            switch (currentZoomLevel)
            {
                case ZoomLevel.MinuteSecond:
                    return date.second.ToString("00");
                case ZoomLevel.HourMinute:
                    return date.hour.ToString() + ":" + date.minute.ToString("00");
                case ZoomLevel.DayHour:
                    return date.hour.ToString() + ":00";
                case ZoomLevel.MonthDay:
                    return date.day.ToString() + " (" + date.GetDayName().Substring(0, 3) + ")";
                case ZoomLevel.YearMonth:
                    return "(" + date.GetMonthName().Substring(0, 3) + ")";
                default:
                    return date.year.ToString();
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

        public void AssignPagesForZoomLevel()
        {
            CustomDateTime refDateTime = startDateTime.GetDateTime();
            if (pageStartDates != null) 
            {
                pageStartDates.Clear();
            }
            if (pageEndDates != null)
            {
                pageEndDates.Clear();
            }
            if (pageLabels != null)
            {
                pageLabels.Clear();
            }

            int tickCount = 0;
            while (refDateTime <= endDateTime)
            {
                if (tickCount == 0)
                {
                    pageStartDates.Add(refDateTime.GetDateTime());
                }
                tickCount++;
                if (tickCount >= ticksPerPage)
                {
                    pageEndDates.Add(refDateTime.GetDateTime());
                    tickCount = 0;
                }
                IterateTickCustomDateTime(refDateTime);
            }

            //if we didn't get all the way to the last tick of the last page, add last end date
            if (pageEndDates.Last() != endDateTime)
            {
                pageEndDates.Add(endDateTime);
            }
            AssignPageLabelsForZoomLevel();
        }

        public void AssignPageLabelsForZoomLevel()
        {
            for (int i = 0; i < this.pageStartDates.Count; i++)
            {
                string label = "";
                switch (currentZoomLevel)
                {
                    case ZoomLevel.MinuteSecond:
                        label =
                            pageStartDates[i].day + 
                            " " + 
                            pageStartDates[i].GetMonthName().Substring(0,3) + 
                            " " + pageStartDates[i].year.ToString("0000") + 
                            " " + pageStartDates[i].hour.ToString() + 
                            ":" + pageStartDates[i].minute.ToString("00") + 
                            " - " + pageEndDates[i].day + 
                            " " + pageEndDates[i].GetMonthName().Substring(0, 3) +
                            " " + pageEndDates[i].year.ToString("0000") + 
                            " " + pageEndDates[i].hour.ToString() + 
                            ":" + pageEndDates[i].minute.ToString("00") + 
                            ":" + pageEndDates[i].second.ToString("00");
                        break;
                    case ZoomLevel.HourMinute:
                        label =
                            pageStartDates[i].day +
                            " " + pageStartDates[i].GetMonthName().Substring(0, 3) +
                            " " + pageStartDates[i].year.ToString("0000") +
                            " " + pageStartDates[i].hour.ToString() +
                            ":" + pageStartDates[i].minute.ToString("00") +
                            " - " + pageEndDates[i].day +
                            " " + pageEndDates[i].GetMonthName().Substring(0, 3) +
                            " " + pageEndDates[i].year.ToString("0000") +
                            " " + pageEndDates[i].hour.ToString() +
                            ":" + pageEndDates[i].minute.ToString("00");
                        break;
                    case ZoomLevel.DayHour:
                        label =
                            pageStartDates[i].day +
                            " " + pageStartDates[i].GetMonthName().Substring(0, 3) +
                            " " + pageStartDates[i].year.ToString("0000") +
                            " " + pageStartDates[i].hour.ToString() +
                            ":00 - " + pageEndDates[i].day +
                            " " + pageEndDates[i].GetMonthName().Substring(0, 3) +
                            " " + pageEndDates[i].year.ToString("0000") +
                            " " + pageEndDates[i].hour.ToString() +
                            ":00";
                        break;
                    case ZoomLevel.MonthDay:
                        label = 
                            pageStartDates[i].day +
                            " " + pageStartDates[i].GetMonthName() +
                            " " + pageStartDates[i].year.ToString("0000") +
                            " " + pageStartDates[i].hour.ToString() +
                            ":" + pageStartDates[i].minute.ToString("00") +
                            " - " + pageEndDates[i].day +
                            " " + pageEndDates[i].GetMonthName() +
                            " " + pageEndDates[i].year.ToString("0000");
                        break;
                    case ZoomLevel.YearMonth:
                        label =
                            pageStartDates[i].GetMonthName() +
                            " (" + pageStartDates[i].month +
                            ") " + pageStartDates[i].year.ToString("0000") +
                            " - " + pageEndDates[i].GetMonthName() +
                            " (" + pageStartDates[i].month +
                            ") " + pageEndDates[i].year.ToString("0000");
                        break;
                    default:
                        label =
                            pageStartDates[i].year.ToString("0000") +
                            " - " + pageEndDates[i].year.ToString("0000");
                        break;
                }
                pageLabels.Add(label);
            }
        }

        public void SetZoomLevel(ZoomLevel zoomLevel)
        {
            this.currentZoomLevel = zoomLevel;
            AssignPagesForZoomLevel();
        }

        public void SetTicksPerPage(int ticks)
        {
            this.ticksPerPage = ticks;
            AssignPagesForZoomLevel();
        }

        public CustomDateTime GetStartDateForPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex > this.pageStartDates.Count)
            {
                return null;
            }
            return this.pageStartDates[pageIndex];
        }

        public CustomDateTime GetEndDateForPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= this.pageEndDates.Count)
            {
                return null;
            }
            return this.pageEndDates[pageIndex];
        }

        public string GetLabelForPage(int pageIndex)
        {

            if (pageIndex < 0 || pageIndex > this.pageLabels.Count)
            {
                return null;
            }
            return this.pageLabels[pageIndex];
        }

        public int GetPageIndexForCustomDateTime(CustomDateTime date)
        {
            int index = 0;
            //increase index until the ending date is no longer smaller than the desired date
            while (pageEndDates[index] < date)
            { 
                index++; 
            }
            //make sure we're still in the accepted range
            if (index < pageEndDates.Count)
            {
                return index;
            }

            //by default just go to 0
            return 0;
        }

        public bool ZoomIn()
        {
            switch (this.currentZoomLevel) 
            {
                case ZoomLevel.MinuteSecond:
                    return false;
                case ZoomLevel.HourMinute:
                    this.SetZoomLevel(ZoomLevel.MinuteSecond);
                    break;
                case ZoomLevel.DayHour:
                    this.SetZoomLevel(ZoomLevel.HourMinute);
                    break;
                case ZoomLevel.MonthDay:
                    this.SetZoomLevel(ZoomLevel.DayHour);
                    break;
                default:
                    this.SetZoomLevel(ZoomLevel.MonthDay);
                    break;
            }
            return true;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stori.Classes
{
    class TimeSystem
    {
        public string eoYear, eoMonth, eoDay, eoHour, eoMinute, eoSecond, name;
        public int monInYear, hourInDay, minInHour, secInMin, timelineId;
        public bool customTime = true;
        public bool leapYearEnabled = false;
        public List<string> monNames, dayNames;
        public List<int> daysInMonths;

        //constructor
        public TimeSystem(
            string eoYear,
            string eoMonth,
            string eoDay,
            string eoHour,
            string eoMinute,
            string eoSecond,
            int monInYear,
            int hourInDay,
            int minInHour,
            int secInMin,
            bool customTime = true,
            bool leapYearEnabled = false,
            List<string> monNames = null,
            List<string> dayNames = null,
            List<int> daysInMonths = null, 
            string name = "My Timeline") 
        {
            this.eoYear = eoYear;
            this.eoMonth = eoMonth;
            this.eoDay = eoDay;
            this.eoHour = eoHour;
            this.eoMinute = eoMinute;
            this.eoSecond = eoSecond;
            this.monInYear = monInYear;
            this.hourInDay = hourInDay;
            this.minInHour = minInHour;
            this.secInMin = secInMin;
            this.customTime = customTime;
            this.leapYearEnabled = leapYearEnabled;
            this.monNames = monNames;
            this.dayNames = dayNames;
            this.daysInMonths = daysInMonths;
            this.name = name;
        }

        public TimeSystem()
        {
            this.customTime = false;
        }

        public int getAverageDaysInMonths()
        {
            int totalDays = 0;
            for (int i = 0; i < monInYear; i++)
            {
                totalDays += daysInMonths[i];
            }
            return totalDays / monInYear;
        }

        public int getDaysInMonth(int month, int year)
        {
            int days = this.daysInMonths[month - 1];
            if (month == 2 && isLeapYear(year))
            {
                days += 1;
            }
            return days;
        }

        public string GetDaysInMonthText()
        {
            if (daysInMonths == null || daysInMonths.Count <= 0)
            {
                return "invalid";
            }
            string text = "";
            foreach (int days in daysInMonths)
            {
                text += days.ToString();
            }
            return text;
        }

        public bool isLeapYear(int year)
        {
            if (!this.leapYearEnabled)
            {
                return false;
            }
            if (year % 4 == 0)
            {
                if (year % 100 == 0)
                {
                    if (year % 400 == 0)
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
            return false;
           
        }

        public void SetDaysInMonths(List<int> newDaysInMonths)
        {
            if (this.daysInMonths is null)
            {
                this.daysInMonths = new List<int>();
            }
            this.daysInMonths.Clear();
            foreach(int days in newDaysInMonths)
            {
                this.daysInMonths.Add(days);
            }
            //this.daysInMonths = daysInMonths;
        }

        public TimeSystem GetTimeSystem()
        {
            return (TimeSystem)this.MemberwiseClone();
        }

    }
}

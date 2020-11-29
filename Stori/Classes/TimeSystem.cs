using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stori.Classes
{
    class TimeSystem
    {
        public string eoYear, eoMonth, eoDay, eoHour, eoMinute, eoSecond;
        public int monInYear, hourInDay, minInHour, secInMin;
        public bool customTime = true;
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
            List<string> monNames = null,
            List<string> dayNames = null,
            List<int> daysInMonths = null) 
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
            this.monNames = monNames;
            this.dayNames = dayNames;
            this.daysInMonths = daysInMonths;
        }

        public TimeSystem()
        {
            this.customTime = false;
        }
    }
}

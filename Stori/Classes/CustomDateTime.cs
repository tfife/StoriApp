using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stori.Classes
{
    class CustomDateTime
    {
        public int year, month, day, hour, minute, second, dayNameIndex;
        TimeSystem timeSystem;

        public CustomDateTime(
            TimeSystem timeSystem,
            int year, 
            int month, 
            int day, 
            int hour = 0, 
            int minute = 0, 
            int second = 0,
            int dayNameIndex = 0)
        {
            this.timeSystem = timeSystem;
            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.dayNameIndex = dayNameIndex;
        }
        public String GetDate()
        {
            return "datestring";
        }

        public CustomDateTime GetDateTime()
        {
            return (CustomDateTime)this.MemberwiseClone();
        }

        // add time
        public void AddYears(int numYears)
        {
            this.year += numYears;
        }

        public void AddMonths(int numMonths)
        {
            //add months
            this.month += numMonths;
            
            //handle overflowing into a new year
            while(this.month > this.timeSystem.monInYear)
            {
                this.month -= this.timeSystem.monInYear;
                this.AddYears(1);
            }
            //handle months with fewer days than the original date
            if(this.day > this.timeSystem.daysInMonths[this.month - 1])
            {
                this.month += 1;
                if (this.month > this.timeSystem.monInYear)
                {
                    this.month = 1;
                    this.year += 1;
                }
                this.day %= this.timeSystem.daysInMonths[this.month - 1];
            }
        }

        public void AddDays(int numDays)
        {
            this.day += numDays;

            if (dayNameIndex != -1)
            {
                this.dayNameIndex += numDays;
                this.dayNameIndex %= this.timeSystem.dayNames.Count();
            }
            
            //handle overflow into months
            while (this.day > this.timeSystem.daysInMonths[this.month - 1])
            {
                this.day -= this.timeSystem.daysInMonths[this.month - 1];
                this.month += 1;
                //if we've gone over the month limit for a year
                if (this.month > this.timeSystem.monInYear)
                {
                    this.AddYears(this.month / this.timeSystem.monInYear);
                    this.month %= this.timeSystem.monInYear;
                }
            }
            

        }

        public void AddHours(int numHours)
        {
            this.hour += numHours;
            //handle overflow into next day(s)
            this.AddDays(this.hour / this.timeSystem.hourInDay);
            this.hour %= this.timeSystem.hourInDay;
        }

        public void AddMinutes(int numMinutes)
        {
            this.minute += numMinutes;
            //handle overflow into other hour(s)
            this.AddHours(this.minute / this.timeSystem.minInHour);
            this.minute %= this.timeSystem.minInHour;
        }

        public void AddSeconds(int numSeconds)
        {
            this.second += numSeconds;
            //handle overflow into other minute(s)
            this.AddMinutes(this.second / this.timeSystem.secInMin);
            this.second %= this.timeSystem.secInMin;
        }

        public string GetDayName()
        {
            if (dayNameIndex >= 0 && dayNameIndex < this.timeSystem.dayNames.Count())
            {
                return this.timeSystem.dayNames[dayNameIndex];
            }
            return dayNameIndex.ToString();
        }

        public string GetMonthName()
        {
            if (this.month <= this.timeSystem.monNames.Count)
            {
                return this.timeSystem.monNames[this.month - 1];
            }
            return "inv i:" + this.month.ToString();
        }

        //overloaded operators
        public static bool operator ==(CustomDateTime lhs, CustomDateTime rhs)
        {
            if (lhs.year == rhs.year &&
                lhs.month == rhs.month &&
                lhs.day == rhs.day &&
                lhs.hour == rhs.hour &&
                lhs.minute == rhs.minute &&
                lhs.second == rhs.second)
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(CustomDateTime lhs, CustomDateTime rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator <(CustomDateTime lhs, CustomDateTime rhs)
        {
            if (lhs.year > rhs.year)
            {
                return false;
            }
            else if (lhs.year == rhs.year)
            {
                if (lhs.month > rhs.month)
                {
                    return false;
                }
                else if (lhs.month == rhs.month)
                {
                    if (lhs.day > rhs.day)
                    {
                        return false;
                    }
                    else if (lhs.day == rhs.day)
                    {
                        if (lhs.hour > rhs.hour)
                        {
                            return false;
                        }
                        else if (lhs.hour == rhs.hour)
                        {
                            if (lhs.minute > rhs.minute)
                            {
                                return false;
                            }
                            else if (lhs.minute == rhs.minute)
                            {
                                if (lhs.second >= rhs.second)
                                {
                                    return false;
                                }
                            }
                            
                        }
                    }
                }
            }
            return true;
        }

        public static bool operator >(CustomDateTime lhs, CustomDateTime rhs)
        {
            return !(lhs == rhs || lhs < rhs);
        }

        public static bool operator >=(CustomDateTime lhs, CustomDateTime rhs)
        {
            return !(lhs < rhs);
        }

        public static bool operator <=(CustomDateTime lhs, CustomDateTime rhs)
        {
            return !(lhs > rhs);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stori.Classes
{
    class CustomDateTime : IComparable<CustomDateTime>
    {
        public int year, month, day, hour, minute, second, dayNameIndex;
        public TimeSystem timeSystem;

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
        public String GetDate(bool includesMonth, bool includesDay, bool includesHour, bool includesMinute, bool includesSecond)
        {
            string date = "";
            if (includesDay)
            {
                date += this.day.ToString() + " ";
            }
            if (includesMonth)
            {
                date += this.GetMonthName() + " ";
            }
            date += this.year.ToString("0000") + " ";

            if (includesHour)
            {
                date += this.hour.ToString() + ":";
                if (includesMinute)
                {
                    date += this.minute.ToString("00");
                }
                else
                {
                    date += "00";
                }
                if (includesSecond)
                {
                    date += ":" + this.second.ToString("00");
                }
            }
            return date;
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
            if(this.day > this.timeSystem.getDaysInMonth(this.month, this.year))
            {
                this.month += 1;
                if (this.month > this.timeSystem.monInYear)
                {
                    this.month = 1;
                    this.year += 1;
                }
                this.day %= this.timeSystem.getDaysInMonth(this.month, this.year);
            }
        }

        public void AddDays(int numDays)
        {
            handleUnwantedNegatives();
            this.day += numDays;

            if (dayNameIndex != -1)
            {
                this.dayNameIndex += numDays;
                this.dayNameIndex %= this.timeSystem.dayNames.Count();
            }
            
            //handle overflow into months
            while (this.day > this.timeSystem.getDaysInMonth(this.month, this.year))
            {
                this.day -= this.timeSystem.getDaysInMonth(this.month, this.year);
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
            handleUnwantedNegatives();
            this.hour += numHours;
            //handle overflow into next day(s)
            this.AddDays(this.hour / this.timeSystem.hourInDay);
            this.hour %= this.timeSystem.hourInDay;
        }

        public void AddMinutes(int numMinutes)
        {
            handleUnwantedNegatives();
            this.minute += numMinutes;
            //handle overflow into other hour(s)
            this.AddHours(this.minute / this.timeSystem.minInHour);
            this.minute %= this.timeSystem.minInHour;
        }

        public void AddSeconds(int numSeconds)
        {
            handleUnwantedNegatives();
            this.second += numSeconds;
            //handle overflow into other minute(s)
            this.AddMinutes(this.second / this.timeSystem.secInMin);
            this.second %= this.timeSystem.secInMin;
        }

        public string GetDayName()
        {
            handleUnwantedNegatives();
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

        public int CompareTo(CustomDateTime other)
        {
            handleUnwantedNegatives();
            other.handleUnwantedNegatives();

            if (this > other)
            {
                return 1;
            }
            else if (this == other)
            {
                return 0;
            }
            return -1;
        }

        public void handleUnwantedNegatives()
        {
            if (second < 0)
            {
                second = 0;
            }
            if (minute < 0)
            {
                minute = 0;
            }
            if (day < 1)
            {
                day = 1;
            }
            if (month < 1)
            {
                month = 1;
            }
        }

        //overloaded operators
        public static bool operator ==(CustomDateTime lhs, CustomDateTime rhs)
        {
            if (lhs.year == rhs.year &&
                (lhs.month == rhs.month) &&
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

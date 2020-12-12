using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stori.Classes
{
    class Event
    {
        public string title, description;
        public CustomDateTime startDateTime, endDateTime;
        public bool includesMonth = false;
        public bool includesDay = false;
        public bool includesHour = false;
        public bool includesMinute = false;
        public bool includesSecond = false;
        public Timeline timeline;
        public List<string> tags = new List<string>();
        public int eventId;

        public Event(Timeline timeline)
        {
            this.timeline = timeline;
        }
    }
}

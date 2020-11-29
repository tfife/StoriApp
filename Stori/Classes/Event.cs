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
        public bool fullDay = false;
        public List<String> tags;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stori.Classes
{
    class Tag
    {
        public string value;
        public bool active;

        public Tag(string value, bool active)
        {
            this.value = value;
            this.active = active;
        }
    }
}

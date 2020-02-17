using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models
{
    public class PreferredName
    {
        public string First { get; set; }
        public string NormalizedFirst { get; set; }
        public string Middle { get; set; }
        public string NormalizedMiddle { get; set; }
        public string Last { get; set; }
        public string NormalizedLast { get; set; }

        public override string ToString()
        {
            return $"{First} {Middle} {Last}";
        }
    }
}

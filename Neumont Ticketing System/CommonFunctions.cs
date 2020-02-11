using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System
{
    public static class CommonFunctions
    {
        public static string NormalizeString(string input)
        {
            return input.RemoveSpecialCharacters().ToUpper();
        }
    }
}

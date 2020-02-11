using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System
{
    public static class ExtensionMethods
    {
        // https://stackoverflow.com/questions/1120198/most-efficient-way-to-remove-special-characters-from-string
        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        // Add only the items in additional that don't already exist in this list
        // NOTE: Will come with a massive performance cost compared to AddRange()
        public static void AddRangeUnique<T>(this List<T> list, List<T> additional)
        {
            foreach(T thing in additional)
            {
                if (!list.Contains(thing))
                    list.Add(thing);
            }
        }
    }
}

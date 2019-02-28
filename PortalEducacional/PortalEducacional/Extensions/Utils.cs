using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalEducacional.Extensions
{
    public static class Utils
    {
        public static string ApenasNumero(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            var sb = new StringBuilder();
            foreach (char c in str)
            {
                if (c == (char)32 || (c >= '0' && c <= '9') || (c == ','))
                {
                    sb.Append(c);
                }
            }
            return sb.Replace(" ", "").ToString();
        }
    }
}

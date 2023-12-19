using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpargoApp.Utils
{
    internal static class ConsoleExtension
    {
        public static void WriteColorLine(string line, ConsoleColor color)
        {
            ConsoleColor inherit = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ForegroundColor = inherit;
        }
    }
}

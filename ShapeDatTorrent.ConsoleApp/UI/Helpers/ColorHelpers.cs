using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeDatTorrent.ConsoleApp.UI.Helpers
{
    internal static class ColorHelpers
    {
        internal static Color MapConsoleColor(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Red: return System.Drawing.Color.Red;
                case ConsoleColor.Green: return System.Drawing.Color.LimeGreen;
                case ConsoleColor.Yellow: return System.Drawing.Color.Yellow;
                case ConsoleColor.Cyan: return Color.Cyan;
                case ConsoleColor.Gray: return Color.LightSlateGray;

                default: return System.Drawing.Color.Gainsboro;
            }
        }
    }
}

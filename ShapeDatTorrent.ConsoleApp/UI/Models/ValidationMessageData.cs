using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeDatTorrent.ConsoleApp.UI.Models
{
    internal class ValidationMessageData
    {
        internal static Dictionary<ValidationMessageType, Image>
            ValidationImages = new Dictionary<ValidationMessageType, Image>
            { 
                {ValidationMessageType.Info, Properties.Resources.information},
                {ValidationMessageType.Warning, Properties.Resources.exclamation },
                {ValidationMessageType.Error, Properties.Resources.cross_octagon },
                {ValidationMessageType.Success, Properties.Resources.tick_circle } 
            };

    }
}

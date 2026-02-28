using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Library
{
    internal class Journal : Literature, IPeriodic
    {
        
        public String Number { get; set; } = null!;

        public override string GetCard()
        {
            return $"{base.Title} {Number} ({base.Publisher})";
        }

        public string GetPeriod()
        {
            return "Місяць";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Library
{
    internal class Newspaper : Literature, IPeriodic
    {
        public DateOnly Date { get; set; }

        public override string GetCard()
        {
            return $"{base.Title} - {Date} - {base.Publisher}";
        }

        public string GetPeriod()
        {
            return "День";
        }
    }
}

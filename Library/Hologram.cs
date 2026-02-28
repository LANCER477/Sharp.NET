using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Library
{
    internal class Hologram : Literature, INonPrintable
    {
        public String ArtItem { get; set; } = null!;

        public override string GetCard()
        {
            return $"{base.Title}: Hologram of {ArtItem} by {base.Publisher}";
        }
    }
}

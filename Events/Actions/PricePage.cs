using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Events.Actions
{
    internal class PricePage
    {
        public PricePage()
        {
            Console.WriteLine("PricePage shown price " + Subject.Instance.Price);
            Subject.Instance.Subsribe(OnPriceChanged);
        }
        private void OnPriceChanged()
        {
            Console.WriteLine("PricePage got new price " + Subject.Instance.Price);
        }
        ~PricePage()
        {
            Subject.Instance.Unsubsribe(OnPriceChanged);
        }
    }
}

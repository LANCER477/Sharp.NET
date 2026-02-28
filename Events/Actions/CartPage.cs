using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Events.Actions
{
    internal class CartPage
    {
        public CartPage()
        {
            Console.WriteLine("CartPage shown price " + Subject.Instance.Price);
            Subject.Instance.Subsribe(OnPriceChanged);
        }
        private void OnPriceChanged()
        {
            Console.WriteLine("CartPage got new price " + Subject.Instance.Price);
        }
        ~CartPage()
        {
            Subject.Instance.Unsubsribe(OnPriceChanged);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Events.Observer
{
    internal class PriceWidget
    {
        private readonly PriceWidgetSubscriber subscriber;
        public PriceWidget()
        {
            subscriber = new PriceWidgetSubscriber(OnPriceChanged);
            Publisher.Instance.Subscribe(subscriber);

            Console.WriteLine("PriceWidget created with  price " +
                Publisher.Instance.Price);
        }
        public void OnPriceChanged(double price)
        {
            Console.WriteLine("PriceWidget: set new price " + price);
        }

        ~PriceWidget()
        {
            Publisher.Instance.Unsubscribe(subscriber);
        }
    }

    public class PriceWidgetSubscriber(Action<double> action) : ISubscriber
    {
        public void Update(double newPrice)
        {
            action(newPrice);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Events.Observer
{
    internal class CartWidget
    {
        private readonly CartWidgetSubscriber subscriber;
        public CartWidget()
        {
            subscriber = new(OnPriceChanged);
            Publisher.Instance.Subscribe(subscriber);

            Console.WriteLine("CartWidget created with  price " + 
                Publisher.Instance.Price);
        }
        public void OnPriceChanged(double price)
        {
            Console.WriteLine("CartWidget: set new price " + price);
        }

        ~CartWidget()  // finalizer
        {
            // !! Дуже важливо знімати підписки при руйнуванні об'єкту
            // оскільки серед підписників лишається неправильне посилання
            // яке викличе помилку і не дасть викликати подальших підписників
            Publisher.Instance.Unsubscribe(subscriber);
        }
    }

    public class CartWidgetSubscriber(Action<double> action) : ISubscriber
    {

        public void Update(double newPrice)
        {
            action(newPrice);
        }
    }
}

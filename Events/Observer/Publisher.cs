using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Events.Observer
{
    internal class Publisher
    {
        private static Publisher? _instance = null;
        public static Publisher Instance => _instance ??= new();

        private List<ISubscriber> subscribers = [];
        private double _price = 100.0;
        public double Price
        {
            get => _price;
            set
            {
                if(value != _price)
                {
                    _price = value;
                    NotifySubscribers();
                }
            }
        }

        public void Subscribe(ISubscriber subscriber)
        {
            subscribers.Add(subscriber);
        }
        public void Unsubscribe(ISubscriber subscriber)
        {
            subscribers.Remove(subscriber);
        }
        public void NotifySubscribers()
        {
            foreach (var subscriber in subscribers)
            {
                subscriber.Update(_price);
            }
        }
    }
}

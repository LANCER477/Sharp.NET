using SharpKnP321.Events.Actions;
using SharpKnP321.Events.Delegates;
using SharpKnP321.Events.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Events
{
    internal class EventsDemo
    {
        public void Run()
        {
            Console.WriteLine("Events Demo");
            // new ObserverDemo().Run();
            // new ActionsDemo().Run();
            // new DelegatesDemo().Run();
            Emitter emitter = new(initialPrice: 100.0);
            PriceComponent pc = new();
            Console.WriteLine("--------------");
            emitter.Price = 200.0;
        }
    }

    public delegate void EmitterListener(double price);

    class Emitter
    {
        // Event - синтаксичний "цукор" над колекцією підписників
        private event EmitterListener? EmitterEvent;   // типізація за делегатом
        public void Subscribe(EmitterListener listener) => 
            EmitterEvent += listener;  // підписка - оператор "+="
        public void Unsubscribe(EmitterListener listener) => 
            EmitterEvent -= listener;  // відписка - оператор "-+"
        // + автоматичний контроль за NULL значенням - коли колекція 
        // підписників порожніє, подія набуває значення NULL, з 
        // додаванням підписників - утворюється автоматично


        private static Emitter? _instance = null;
        public static Emitter Instance => _instance ??= new(0.0);
        public Emitter(double initialPrice)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("Emitter was instantiated already");
            }
            _price = initialPrice;
            _instance = this;
        }
        #region context
        private double _price;
        public double Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    EmitterEvent?.Invoke(_price);
                }
            }
        }
        #endregion

    }

    class PriceComponent
    {
        private readonly EmitterListener _listener = (price) =>
        {
            Console.WriteLine("PriceComponent got new price " + price);
        };

        public PriceComponent()
        {
            Console.WriteLine("PriceComponent starts with price " + Emitter.Instance.Price);
            Emitter.Instance.Subscribe(_listener);
        }
        ~PriceComponent()
        {
            Emitter.Instance.Unsubscribe(_listener);
        }
    }
}

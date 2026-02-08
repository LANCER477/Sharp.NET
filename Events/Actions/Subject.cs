using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Events.Actions
{
    internal class Subject  // на заміну Publisher
    {
        private static Subject? _instance = null;
        public static Subject Instance => _instance ??= new(0.0);
        public Subject(double initialPrice) {
            if (_instance != null)
            {
                throw new InvalidOperationException("Subject was instantiated already");
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
                    NotifySubscribers();
                }
            }
        }
        #endregion

        #region Notifier
        private readonly List<Action> subscribers = [];
        public void Subsribe(Action action)
        {
            lock (subscribers) { subscribers.Add(action); }
        }
        public void Unsubsribe(Action action)
        {
            lock (subscribers) { subscribers.Remove(action); }
        }
        private void NotifySubscribers()
        {
            lock (subscribers) { subscribers.ForEach(s => s.Invoke()); }
        }
        #endregion
    }
}

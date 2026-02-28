using SharpKnP321.Events.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Events.Delegates
{
    // делегат - тип даних для методів (аналог Action<double>)
    public delegate void StateListener(double price);
    
    internal class State
    {
        private static State? _instance = null;
        public static State Instance => _instance ??= new(0.0);
        public State(double initialPrice)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("State was instantiated already");
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
                    NotifyListeners(_price);
                }
            }
        }
        #endregion

        private readonly List<StateListener> listeners = [];
        public void AddListener(StateListener listener) => listeners.Add(listener); 
        public void RemoveListener(StateListener listener) => listeners.Remove(listener);
        private void NotifyListeners(double price) =>
            listeners.ForEach(listener => listener(price));

    }
}

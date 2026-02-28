using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Events.Delegates
{
    internal class PriceView
    {
        private void _Action(double d) { }

        private readonly StateListener _listener = (price) =>  // implicit new State...
        {
            Console.WriteLine("PriceView got new price " + price);
        };

        public PriceView()
        {
            Console.WriteLine("PriceView starts with price " + State.Instance.Price);
            State.Instance.AddListener(_listener);
            State.Instance.AddListener(_Action);
        }
        ~PriceView()
        {
            State.Instance.RemoveListener(_listener);
        }

    }
}
/* Д.З. Прикласти посилання на репозиторій з підсумковим проєктом
 */
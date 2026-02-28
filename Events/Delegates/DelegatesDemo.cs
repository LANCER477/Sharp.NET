using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Events.Delegates
{
    internal class DelegatesDemo
    {
        public void Run()
        {
            State state = new(100.0);
            new PriceView();
            Console.ReadLine();
            state.Price = 200.0;
        }
    }
}

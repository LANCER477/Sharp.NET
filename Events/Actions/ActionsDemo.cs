using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Events.Actions
{
    internal class ActionsDemo
    {
        public void Run()
        {
            Subject subject = new(initialPrice: 100.0);
            PricePage pricePage = new();
            CartPage cartPage = new();
            Console.WriteLine("--------------");
            subject.Price = 200.0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Library
{
    internal class Booklet : Literature
    {
        public String Author { get; set; } = null!;
        public string Subject { get; set; } = null!;

        public override string GetCard()
        {
            return $"{Author}, {base.Title} - {Subject}";
        }
        public void Print()
        {
            Console.WriteLine("Printing... " + GetCard());
        }

        [ColorPrint(Copies = 2)]  // ending '...Attribute' optional
        public void PrintWithColor(String colorScheme)
        {
            Console.WriteLine($"In colors '{colorScheme}' Printing... " + GetCard());
        }
    }
}

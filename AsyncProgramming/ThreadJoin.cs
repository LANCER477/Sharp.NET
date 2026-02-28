using System;
using System.Collections.Generic;
using System.Text;

namespace SharpKnP321.AsyncProgramming
{
    internal class ThreadJoin
    {
        public void Run()
        {
            Console.WriteLine("{0:F2} Breakfast start", (DateTime.Now.Ticks % (long)1e8) / 1e7);

            Thread makeCoffee = new(MakeCoffee);
            makeCoffee.Start();   // Порядок запуску - спочатку найбільш тривалі

            Thread roastBeacon = new(RoastBeacon);
            roastBeacon.Start();  // 

            Thread makeToast = new(MakeToast);            
            makeToast.Start();    // 
            
            makeToast.Join();     // очікування потоку - блокування потоку виклику (Run) до завершення роботи makeToast
            roastBeacon.Join();   // порядок очікування - за логікою
            makeCoffee.Join();

            Console.WriteLine("{0:F2} Breakfast finish", (DateTime.Now.Ticks % (long)1e8) / 1e7);
        }

        private void MakeToast()
        {
            Console.WriteLine("{0:F2} MakeToast Start", (DateTime.Now.Ticks % (long)1e8) / 1e7);
            Thread.Sleep(100);
            Console.WriteLine("{0:F2} MakeToast Finish", (DateTime.Now.Ticks % (long)1e8) / 1e7);
        }

        private void RoastBeacon()
        {
            Console.WriteLine("{0:F2} RoastBeacon Start", (DateTime.Now.Ticks % (long)1e8) / 1e7);
            Thread.Sleep(300);
            Console.WriteLine("{0:F2} RoastBeacon Finish", (DateTime.Now.Ticks % (long)1e8) / 1e7);
        }

        private void MakeCoffee()
        {
            Console.WriteLine("{0:F2} MakeCoffee Start", (DateTime.Now.Ticks % (long)1e8) / 1e7);
            Thread.Sleep(1000);
            Console.WriteLine("{0:F2} MakeCoffee Finish", (DateTime.Now.Ticks % (long)1e8) / 1e7);
        }
    }
}
/* Очікування потоків
 * MakeToast - 1хв
 * RoastBeacon - 3хв
 * MakeCoffe - 10хв
 * 
 *    Start      Join
 * -------        ---------
 *      \\----/   /
 *       \-------/
 *       
 * Модифікувати попереднє ДЗ (з формуванням масиву)
 * на синхронізацію з головним потоком через очікування:
 * підсумковий результат виводиться у головному потоці
 */
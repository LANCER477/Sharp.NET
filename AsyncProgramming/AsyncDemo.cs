using System;
using System.Collections.Generic;
using System.Text;

namespace SharpKnP321.AsyncProgramming
{
    internal class AsyncDemo
    {
        public void Run()
        {
            Console.WriteLine("AsyncDemo start");
            RunAsync().Wait();  // для переходу від синхронних до async методів
                                // необхідний один "не цукровий" варіант
            Console.WriteLine("AsyncDemo finish");
        }

        private async Task RunAsync()  // за традиціями, імена async-методів завершуються на Async 
        {
            long startTicks = DateTime.Now.Ticks;
            Console.WriteLine("{0:F2} RunAsync start", ((DateTime.Now.Ticks - startTicks) % (long)1e8) / 1e7);
            // Суть синхронний результат при асинхронному виконанні - через те, що 
            //  очікування задач не дає починатись іншим задачам
            // Console.WriteLine("{1:F2} {0}", await GetStringAsync(100), ((DateTime.Now.Ticks - startTicks) % (long)1e8) / 1e7);
            // Console.WriteLine("{1:F2} {0}", await GetStringAsync(50, 500), ((DateTime.Now.Ticks - startTicks) % (long)1e8) / 1e7);
            // Console.WriteLine("{1:F2} {0}", await GetStringAsync(70, 700), ((DateTime.Now.Ticks - startTicks) % (long)1e8) / 1e7);

            // необхідно розділяти запуск задач та їх очікування
            // якщо можливо оцінити тривалість задач, то почати слід з найтриваліших
            Task<String> t100 = GetStringAsync(100);
            Task<String> t70 = GetStringAsync(70, 700);
            var t50 = GetStringAsync(50, 500);
            Console.WriteLine("{1:F2} {0}", await t50,  ((DateTime.Now.Ticks - startTicks) % (long)1e8) / 1e7);
            Console.WriteLine("{1:F2} {0}", await t100, ((DateTime.Now.Ticks - startTicks) % (long)1e8) / 1e7);
            Console.WriteLine("{1:F2} {0}", await t70,  ((DateTime.Now.Ticks - startTicks) % (long)1e8) / 1e7);

        }

        // явно зазначаємо "обгортку" Task<String> ...
        private async Task<String> GetStringAsync(int length, int delay=1000)
        {
            // Task.Delay(1000).Wait();
            await Task.Delay(delay);   // await - аналог .Result але 
            // даний оператор може використовуватись лише в async методах

            // ... проте повертаємо просто String
            return $"The string of length {length}";
        }
    }
}
/* Д.З. Реалізувати попереднє ДЗ (формування масиву)
 * за допомогою async-await синтаксису.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpKnP321.Extensions.Str;
using SharpKnP321.Library;   // для включення розширень using обов'язковий

namespace SharpKnP321.Extensions
{
    internal class ExtensionsDemo
    {
        public void Run()
        {
            Console.WriteLine("Extensions Demo");
            // оголошений окремо клас IntExtension вводить метод .px() для int
            Console.WriteLine("margin: " + 2.px());
            Console.WriteLine("margin: " + 2.percnt());

            // оскільки розширення знаходиться в окремом просторі імен
            // воно стає доступним тільки з оператором using (див. вище)
            Console.WriteLine("the_snake_case_name".SnakeToCamel());

            // Створити розширення для double .ToMoney(), яке буде
            // округлювати число до двох знаків, за потреби дописувати
            // нулі після коми,
            // ** а також додає розділення розрядів:
            // 123412134.5 -> 123 412 134.50

            // Розширення для інтерфейсів:
            var n = new Newspaper
            {
                Title = "Урядовий кур'єр",
                Date = new DateOnly(2025, 10, 23),
                Publisher = "Газета Кабінету Міністрів України"
            };
            // розширення .IsDaily() доступне через інтерфейс IPeriodic
            Console.WriteLine(n.IsDaily() ? "Щоденна" : "Не щоденна");

            var j = new Journal
            {
                Title = "ArgC & ArgV",
                Number = "2(113), 2000",
                Publisher = "https://journals.ua/technology/argc-argv/"
            };
            Console.WriteLine(j.IsDaily() ? "Щоденний" : "Не щоденний");
        }
    }

    public static class PeriodicExtension
    {
        public static bool IsDaily(this IPeriodic lit)
        {
            return lit.GetPeriod() == "День";
        }
    }
}
/* Класи-розширення (Extensions) - інструмент для розширення функціональності
 * інших класів (чи інтерфейсів) через оголошення спеціальних класів-розширень.
 * 
 */
/* Д.З. Створити розширення для типу DateTime, яке буде представляти об'єкт
 * в SQL форматі: 
 * "2025-11-06 11:32:23"
 * Провести випробування, додати скріншоти роботи.
 */
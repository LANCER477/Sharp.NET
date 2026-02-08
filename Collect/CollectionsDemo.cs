using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Collect
{
    internal class CollectionsDemo
    {
        class Manager
        {
            public int Id { get; set; }
            public String Name { get; set; } = String.Empty;
            public override string ToString()
            {
                return $"{Name} (id={Id})";
            }
        }
        class Sale
        {
            public int Id { get; set; }
            public int ManagerId { get; set; }
            public double Price { get; set; }
            public override string ToString()
            {
                return $"Sale(id={Id}, ManagerIg={ManagerId}, Price={Price:F2})";
            }
        }

        public void Run()
        {
            Random random = new();
            List<Manager> managers = [
                new() { Id = 1, Name = "John Doe" },
                new() { Id = 2, Name = "July Smith" },
                new() { Id = 3, Name = "Nik Forest" },
                new() { Id = 4, Name = "Barbara White" },
                new() { Id = 5, Name = "Will Black" },
            ];
            List<Sale> sales = [..Enumerable               // .. - spread operator -
                .Range(1, 100)                             // перетворює вираз у послідовність
                .Select(x => new Sale{                     // (умовно - записує дані через кому)
                    Id = x,                                // 
                    ManagerId = 1 + x % managers.Count,    // 
                    Price = random.NextDouble() * 1000     // 
                })                                         // 
            ];
            // sales.Take(10).ToList().ForEach(Console.WriteLine);

            // Поєднати дані - вивести Менеджер--сума продажу
            var query1 = sales
                .Select(s => new
                {
                    Manager = managers.First(m => m.Id == s.ManagerId),
                    s.Price
                });

            // foreach (var item in query1)
            // {
            //     Console.WriteLine("{0} {1:F2}", item.Manager, item.Price);
            // }

            var query2 = sales              // 
                .Join(                      // Join - поєднання колекцій
                    managers,               // інша колекція
                    s => s.ManagerId,       // вибір зовнішнього ключа
                    m => m.Id,              // вибір внутрішнього ключа
                    (s, m) => new           // правило комбінації пари-вибірки
                    {                       // 
                        Sale = s,           // 
                        Manager = m         // 
                    });                     // 

            // foreach (var item in query2)
            // {
            //     Console.WriteLine("{0} {1}", item.Manager, item.Sale);
            // }

            var query3 = managers               // GroupJoin - поєднання з групуванням
            .GroupJoin(                         // (в одного менеджера є кілька продажів)
                sales,                          // 
                m => m.Id,                      // 
                s => s.ManagerId,               // 
                (m, ss) => new                  // m - один менеджер
                {                               // ss - множина продажів по даному менеджеру
                    Manager = m,                // 
                    Sum = ss.Sum(s => s.Price)  // 
                })                              // Продовження інструкції відбувається з
            .Select(item => new                 // вибіркою {Manager, Sum}
            {                                   // 
                Name = item.Manager.Name,       // У свою чергу Select перетворює вибірку 
                Total = item.Sum                // на {Name, Total}
            })
            .OrderBy(item => item.Total);         

            foreach (var item in query3)
            {
                Console.WriteLine("{0} -- {1:F2}", item.Name, item.Total);
            }
            Console.WriteLine("===============");
            // Визначити найгіршого за продажами
            var theWorst = query3.First();
            Console.WriteLine("Найгірший результат {0:F2} у {1}", theWorst.Total, theWorst.Name);

            Console.WriteLine("===============");
            
            // Визначити трійку найгірших
            foreach (var w in query3.Take(3))
            {
                Console.WriteLine("{0} -- {1:F2}", w.Name, w.Total);
            }
        }
        /* Д.З. Засобами LINQ на базі аудиторної структури (Manager, Sale)
         * створити запити на
         * - підрахунок кількості продажів (Менеджер - шт)
         * - підрахунок кількості продажів суми яких перевищують 500.00
         * - підрахунок кількості продажів суми яких менші за 100.00
         * = кожен запит сортувати за числовим показником
         * - розрахунок премії: за 1 місце (за продажами) - 10% від суми продажів,
         *     2 - 7%, 3 - 5%
         *   вивести трійку кращих з розміром премії (округлити до двох знаків)  
         */

        public void RunL()
        {
            // LINQ Language Integrated Queries - спец.синтаксис для роботи з колекціями
            List<String> strings = [];
            for (int i = 0; i < 10; i++)
            {
                strings.Add("String " + i);
            }
            var query =                       // from - синтаксис
                from s in strings             // застарілий, майже не вживається
                where s[^1] == '2' || s[^1] == '3'
                select s;                     

            // Console.WriteLine(query);      // System.Linq.Enumerable+ListSelectIterator`2[System.String,System.String]
            foreach(var str in query)
            {
                Console.WriteLine(str);
            }
            Console.WriteLine("==============");
            // Method-syntax
            var query2 = strings
                .Where(s => ((int)s[^1] & 1) == 1)   // Where - filter
                .Select(s => s.ToUpper());           // Select - transform
                // .ToList();                        // Перетворювач до колекції

            foreach(String str in query2)
            {
                Console.WriteLine(str);
            }
        }

        public void RunList()
        {
            // Collections
            List<String> strings = [];
            for (int i = 0; i < 10; i++)
            {
                strings.Add("String " + i);
            }
            foreach(String str in strings)
            {
                Console.WriteLine(str);
                // strings.Remove(str);   // InvalidOperationException: Collection was modified
                // strings.Add("New")     // InvalidOperationException: Collection was modified
            }
            strings.Add("New 7");   // поза циклом помилок немає
            Console.WriteLine("----------");
            strings.ForEach(Console.WriteLine);
            Console.WriteLine("----------");
            // Видалити усі елементи, які завершуються на непарне число
            // Під час ітерації видалення призведе до винятку, відповідно для рішення
            // потрібна друга колекція
            List<String> removes = [];
            foreach(var str in strings) // утворюємо цикл по strings та модифікуємо removes
            {
                char c = str[^1];
                if(c <= '9' && c >= '0')
                {
                    int n = (int)c;   // '0'=48, '1'=49, ... - парність числа збігається з парністю ASCII коду
                    if((n & 1) == 1)
                    {
                        removes.Add(str);   // у першому циклі знаходимо ті, що мають 
                        // бути видалені та поміщуємо їх (посилання на них) у другу колекцію
                    }
                }
                // Console.WriteLine(str[^1]);
            }
            // утворюємо цикл по removes та модифікуємо strings
            foreach(String rem in removes)
            {
                strings.Remove(rem);
            }
            Console.WriteLine("----------");
            strings.ForEach(Console.WriteLine);
            Console.WriteLine("----------");
        }
        /* bool: a && b
         * bitwise: x & y - побітовий "ТА"
         * x = 10    1010
         * y = 8     1000
         * x & y        0
         */
        /* 001000 | 001001 = 001001
         */
        /* Slices
         * str = "The string"
         * str[1] = 'h'
         * str[1..4] = "he s"
         * str[..5] == str[0..5] = "The st"
         * str[5..] = "tring"
         * str[^1] = "g" (-1 - перший з кінця)
         * str[..^1] = "The strin"
         */

        public void RunArr()
        {
            Console.WriteLine("Collections Demo");
            /* Масив(класично) - спосіб збереження даних, за якого однотипні дані розміщуються у
             * пам'яті послідовно і мають визначений розмір.
             * У C#.NET масив - об'єкт, який забезпечує управління класичним масивом.
             */
            String[] arr1 = new String[3];
            String[] arr2 = new String[3] { "1", "2", "3" };
            String[] arr3 = { "1", "2", "3" };
            String[] arr4 = [ "1", "2", "3" ];
            arr1[0] = new("Str 1");   // базовий синтаксис роботи з елементами масивів
            arr1[1] = arr2[0];        // забезпечується індексаторами на відміну від С++
                                      // де [n] - це розіменування зі зміщенням: a[n] == *(a + n)
                                      // Dereferencing (Posible null dereferencing -> NullReferenceExc)
            
            arr1[0] = "New Str 1";
            int x = arr1.Length;

            /* На відміну від масивів колекції:
             * дозволяють змінний розмір
             * дозволяють непослідовне збереження
             */
        }
    }
}
/* Garbage Collector
 * [obj1  obj2  obj3 ....]
 * [obj1  ----  obj3 ....] 
 * 
 * 
 *                pointer                      pointer
 *                   |                           |
 * GC: [obj1  ----  obj3 ....] --> [obj1 obj3 ........] 
 *                   |                    |
 *               reference             reference
 * 
 * 
 * [ arr1[0] arr1[1] ...        "Str1" ... "New Str 1" ]
 *      \_x______________________/           /
 *        \_________________________________/
 */

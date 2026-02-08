using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharpKnP321.Library
{
    public class Library
    {
        [JsonInclude]
        private List<Literature> Funds { get; set; } = [];
        
        public Library()
        {
            
        }

        public void Init()
        {
            Funds.Add(new Book
            {
                Author = "D. Knuth",
                Title = "The art of programming",
                Publisher = "Київ, Наукова Думка",
                Year = 2000
            });
            Funds.Add(new Book
            {
                Author = "J. Richter",
                Title = "CLR via C#",
                Publisher = "Microsoft Press",
                Year = 2013
            });
            Funds.Add(new Journal
            {
                Title = "ArgC & ArgV",
                Number = "2(113), 2000",
                Publisher = "https://journals.ua/technology/argc-argv/"
            });
            Funds.Add(new Newspaper
            {
                Title = "Урядовий кур'єр",
                Date = new DateOnly(2025, 10, 23),  // як діє оператор new на структури?
                // оскільки структура - ValueType, то значення замінюється в області
                // змінної шляхом перезапуску конструктора структури
                Publisher = "Газета Кабінету Міністрів України"
            });
            Funds.Add(new Hologram
            {
                Title = "Скіфське мистецтво",
                ArtItem = "Пектораль",
                Publisher = "Студія 'Лазер'"
            });
            Funds.Add(new Booklet
            {
                Title = "New Opening!",
                Subject = "Sweet Restaurant",
                Author = "PopShop"
            });
        }
        public void ShowColorPrintable()
        {
            // Пошук за атрибутами - метаданими, що супроводжують методи
            foreach (Literature literature in Funds)
            {
                foreach(var method in literature.GetType().GetMethods())
                {
                    var attr = method.GetCustomAttribute<ColorPrintAttribute>();
                    if(attr != null)
                    {
                        // вилучення додаткових даних з атрибуту (attr.Copies)
                        for (int i = 0; i < attr.Copies; i++)
                        {
                            method.Invoke(literature, ["RGB"]);
                        }                        
                    }
                }
            }
        }

        public void ShowPrintable()
        {
            // Duck Typing - find objects of any type with Print() method
            foreach (Literature literature in Funds)
            {
                MethodInfo? printMethod = literature.GetType().GetMethod("Print");
                if(printMethod != null)
                {
                    // Method invocation
                    //                  object     args - values for params
                    printMethod.Invoke(literature, null);
                }
            }
        }

        public void PrintCatalog()
        {
            foreach (Literature literature in Funds)
            {
                Console.WriteLine(literature.GetCard());
            }
        }

        public void PrintPeriodic()
        {
            foreach (Literature literature in Funds)
            {
                if (literature is IPeriodic lit)
                {
                    // Прямо через literature метод GetPeriod не доступний
                    // (хоча ми певні, що він там є, бо умова "is IPeriodic" виконана)
                    // Для доступу до методу інтерфейсу необхідно перетворити
                    // тип змінної до інтерфейсного
                    // literature as IPeriodic  -- референсне (м'яке) перетворення
                    // (IPeriodic) literature -- "жорстке" перетворення
                    // pattern matching -- if (literature is IPeriodic lit)
                    Console.Write("Раз у " + lit.GetPeriod() + ": ");
                    Console.WriteLine(literature.GetCard());
                }
            }
        }

        public void PrintNonPeriodic()
        {
            foreach (Literature literature in Funds)
            {
                if (literature is not IPeriodic)
                {
                    Console.WriteLine(literature.GetCard());
                }
            }
        }
    }
}
/*
 [] - оперативна пам'ять
 () - програма (збірка)
 {} - постійна пам'ять (диск, BIOS)

Запуск                   Value Type                Reference Type
{(program), ...}       { ( 32bit ), ...}           { ( *64bit ), ...}   
     |                                            
     v                    x = 10                      x = new(10)
[ (program), ... ]     [ ( 10 ), ... ]             [ ( ref ), ... (10#ref) ]
                                                           \________/
                                                  
                       x = new(20)?                 x = new(20)?
                       перезапуск конструктора      [ ( ref2 ), ... (10#ref1) ... (20#ref2)]
                       без створення додаткових            \___ХX___/                /
                       об'єктів                             \_______________________/
                       [ ( 20 ), ... ]              утворення нового об'єкту і заміна посилання на нього
                       
 
 A = B                 копія значення В до А        утворення другого посилання на В

 B.x = 10              змінюється тільки B.x        змінюється поле "х" єдиного об'єкту,
                                                    тобто як В.х, так і А.х
 */
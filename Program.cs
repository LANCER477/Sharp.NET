
using SharpKnP321.AsyncProgramming;
using SharpKnP321.Collect;
using SharpKnP321.Data;
using SharpKnP321.Dict;
using SharpKnP321.Events;
using SharpKnP321.Exceptions;
using SharpKnP321.Extensions;
using SharpKnP321.Files;
using SharpKnP321.Library;
using SharpKnP321.Networking;
using SharpKnP321.Users;
using SharpKnP321.Vectors;
using System.Reflection;
using System.Runtime.InteropServices;

// ShowReflection();
// new VectorDemo().Run();
try  // рекомендація - оточувати точку входу блоком try-catch
{
    Console.OutputEncoding = System.Text.Encoding.Unicode;
    Console.InputEncoding = System.Text.Encoding.Unicode;
    // new ExceptionsDemo().Run();
    // new FilesDemo().Run();
    // new ExtensionsDemo().Run();
    // new EventsDemo().Run();
    // new CollectionsDemo().Run();
    // new DictDemo().Run();
    // new DataDemo().Run();
    // new AsyncProgramming().Run();

    /*  // Запуск системних задач, які орієнтуються на pointers, краще робити в умовах фіксованих розміщень об'єктів
    var t = new ThreadPooling();
    var h = GCHandle.Alloc(t, GCHandleType.Pinned);   // не дозволяємо GC зміщувати даний об'єкт у пам'яті
    t.Run();
    h.Free();                    // по завершенню знімаємо фіксацію об'єкта у пам'яті
    */
    // new TaskDemo().Run();
    // new AsyncDemo().Run();
    // new Continuations().Run();
    // new NetworkingDemo().Run().Wait();
    //new EmailDemo().Run();
    new UsersDemo().Run();
}
catch (Exception ex)
{
    // Логічних дій з винятком на даному рівні передбачити важко -
    // здійснюється логування (запис даних) про аварійну зупинку програми
    // У режимі розробника це ще може бути Error-Page
    Console.WriteLine("Не оброблений у прогамі виняток: " + ex.ToString());
}

Console.WriteLine("Main finished");




void ShowReflection()
{
    /* Рефлексія (в ООП) - інструментарій мови/платформи, який 
     * дозволяє одержувати відомості про склад типу даних
     */
    Type bookType = typeof(Book);

    FieldInfo[] fields = bookType.GetFields();
    if (fields.Length > 0)
    {
        Console.WriteLine("Type 'Book' has fields:");
        foreach (var field in fields)
        {
            Console.WriteLine(field.Name);
        }
    }
    else
    {
        Console.WriteLine("Type 'Book' has no fields");
    }


    Console.WriteLine("Type 'Book' has props:");
    foreach (var prop in bookType.GetProperties())
    {
        Console.WriteLine("{0}: {1}", prop.Name, prop.PropertyType.Name);
        //Console.WriteLine($"{prop.Name}: {prop.PropertyType.Name}");
    }

    Console.WriteLine("Type 'Book' has methods:");
    foreach(var method in bookType.GetMethods())
    {
        Console.WriteLine(method.Name);
    }

    Console.WriteLine("----------------------- Рефлексія за об'єктом --------------");

    Literature j = new Journal()
    {
        Title = "ArgC & ArgV",
        Number = "2(113), 2000",
        Publisher = "https://journals.ua/technology/argc-argv/"
    };
    Type jType = j.GetType();
    Console.WriteLine(jType.Name);   // Journal --- змінна типізується з об'єктом (а не за оголошенням)
    // як дізнатись чи є у змінній властивість Number та, якщо є, дістатись його значення
    PropertyInfo? propN = jType.GetProperty("Number");
    if (propN != null)
    {
        // prop - відомості про тип даних, а не про об'єкт
        var number = propN.GetValue(j);   // -> j.Number
        Console.WriteLine($"Object has 'Number' property with value '{number}'");
    }
    else
    {
        Console.WriteLine("Object has no 'Number' property");
    }
    // "Качина типізація": якщо щось ходить як качка та видає звуки качки, то це і є качка
    // Програмний прийом за якого визначається не сам тип, а наявність у ньому
    // певних складових (частіше за все - методів)
    // Замість перевірки умовного інтерфейсу IPrintable перевіряється наявність 
    // метода Print()
    Library library = new();
    Console.WriteLine("--------- printable ------------");
    library.ShowPrintable();
    Console.WriteLine("--------- color printable ------------");
    library.ShowColorPrintable();
}

void ShowLibrary()
{
    Library library = new();
    library.PrintCatalog();

    Console.WriteLine("\n--------Periodic-----------");
    library.PrintPeriodic();

    Console.WriteLine("\n--------NonPeriodic-----------");
    library.PrintNonPeriodic();

    Console.WriteLine("-------------------");
}

void Intro()
{
    int[] arr = new int[10];
    foreach (int el in arr)
    {
        Console.WriteLine(el);
    }
    for (int i = 0; i < arr.Length; i++)
    {
        Console.WriteLine(arr[i]);
    }
    arr[0] = default;
    int[][] arr2 = new int[5][];   // jagged - "рвані" масиви
    for (int i = 0; i < 5; i += 1)
    {
        arr2[i] = new int[i + 1];
    }
    foreach (int[] el in arr2)
    {
        foreach (int w in el)
        {
            Console.Write(w + " ");
        }
        Console.WriteLine();
    }
    int[,] arr3 = new int[3, 4];
    for (int i = 0; i < 3; ++i)
    {
        for (int j = 0; j < 4; ++j)
        {
            Console.Write(arr3[i, j]);
        }
        Console.WriteLine();
    }
    String[] strings = { "s1", "s2" };
    String[] s2 = ["s1", "s2"];

    return;

    Console.Write("Enter your name: ");    // Виведення без переводу рядка

    // NULL-safety -- традиція у сучасних мовах програмування, згідно з якою
    // розрізняються типи даних, які дозволяють значення NULL, та ті, 
    // які не дозволяють
    String? name = Console.ReadLine();     // Введення з консолі

    if (String.IsNullOrEmpty(name))
    {
        Console.WriteLine("Bye");
    }
    else
    {
        Console.WriteLine("Hello, " + name);   // Виведення з переводом рядка
    }
    // Типи даних
    // Усі типи даних є нащадками загального типу Object
    // через це вони мають ряд спільних методів: GetType, ToString, GetHashCode, ...
    // Типи даних належать простору імен System, для скорочення інструкцій
    // існють псевдоніми типів:
    int x;  // Псевдонім для System.Int32
    System.Int32 y;
    string s1;
    //System.String s2;
    float f;   // System.Single   -- 32 bit
    double g;  // System.Double   -- 62 bit
               // Nullable - версії
    Nullable<int> v;  // повна форма
    int? a;           // скорочена форма

    Console.Write("How old are you? ");
    String ageInput = Console.ReadLine()!;   // !(наприкінці) - NULL-checker
    int age = int.Parse(ageInput);           // Parsing - відновлення значення з рядка
    Console.WriteLine("Next year you'll be " + (age + 1));

    Console.Write("Previous ages: ");
    for (int i = 0; i < age; i += 1)
    {
        Console.Write(i + " ");
    }
    int rem = 10 % 3;   // залишок від ділення
    Console.WriteLine();
    // String - Immutable - не дозволяє зміни
    char c = ageInput[0];   // дозволено
                            // ageInput[0] = 'A';  - не дозволено
                            // якщо потрібно змінювати рядок, то слід формувати новий
    ageInput = "A" + ageInput.Substring(1);
}
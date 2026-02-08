using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace SharpKnP321.Files
{
    internal class FilesDemo
    {
        public void Run()
        {
            // Serializing
            Library.Library library = new();
            library.Init();
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true
            };
            String lib = System.Text.Json.JsonSerializer.Serialize(library, options);
            Console.WriteLine(lib);
            File.WriteAllText("library.json", lib);

            Library.Library library2;
            try
            {
                library2 = JsonSerializer.Deserialize<Library.Library>(lib)!;
                //    ?? throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            library2.PrintCatalog();

            Vectors.Vector v = new() { X = 10, Y = 20 };
            String j = System.Text.Json.JsonSerializer.Serialize(v);
            Console.WriteLine(j);
            Vectors.Vector v2 = System.Text.Json.JsonSerializer.Deserialize<Vectors.Vector>(j);
            Console.WriteLine(v2);

        }
        public void RunLog()
        {
            String logDir = Directory.GetCurrentDirectory() + "/logs";
            String logFile = "runlogs.txt";
            String logPath = Path.Combine(logDir, logFile);
            #region logging
            if ( ! Directory.Exists(logDir) )
            {
                try
                {
                    Directory.CreateDirectory(logDir);
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Неможна створити директорію логування " + ex.Message);
                    return;
                }
            }
            if ( ! File.Exists(logPath))
            {
                try
                {
                    File.Create(logPath).Dispose();
                    
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Неможна створити файл логування " + ex.Message);
                    return;
                }
            }
            try
            {
                File.AppendAllText(logPath, DateTime.Now.ToString() + "\n");
            }
            catch (IOException ex)
            {
                Console.WriteLine("Помилка логування " + ex.Message);
                return;
            }
            #endregion

            String[] lines = File.ReadAllLines(logPath);
            String times = EndingType(lines.Length) switch
            {
                0 => "разів",
                1 => "раз",
                2 => "рази",
                _ => throw new ArgumentException("Unexpected ending type")
            };
            Console.WriteLine($"Програма запускалась {lines.Length} {times}:");
            for (int i = 0; i < lines.Length; i++)
            {
                Console.Write($"{i+1}. ");
                Console.WriteLine(lines[i]);
            }
        }

        int EndingType(int number)
        {
            number = number % 100;
            if (number >= 5 && number <= 20) return 0;
            number = number % 10;
            if (number == 0 || number >= 5) return 0;
            if (number == 1) return 1;
            return 2;
        }

        public void Run4()
        {
            string dir = Directory.GetCurrentDirectory();
            string filepath = Path.Combine(dir, "file4.txt");
            try
            {
                File.WriteAllText(filepath, "File 4 content");
                File.AppendAllText(filepath, "\nAppend line");
                Console.WriteLine(File.ReadAllText(filepath));
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void Run3()
        {
            try
            {
                using var wtr = new StreamWriter("./file3.txt");
                wtr.Write("x = ");
                wtr.Write(10);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                using var rdr = new StreamReader("./file3.txt");
                string content = rdr.ReadToEnd();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public void Run2()
        {
            Console.WriteLine("Робота з файлами");
            FileStream? fs = null;
            try
            {
                fs = new FileStream("./file1.txt", FileMode.Create, FileAccess.Write);
                fs.Write(Encoding.UTF8.GetBytes("Hello World!"));
                fs.Flush();
                fs.Close();
            }
            catch(IOException ex) 
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                fs?.Close();
            }

            try
            {
                using var rfs = new FileStream("./Files/file2.txt", FileMode.Open, FileAccess.Read | FileAccess.Write);
                byte[] buf = new byte[4096];
                int n = rfs.Read(buf, 0, 4096);
                Console.WriteLine(Encoding.UTF8.GetString(buf, 0, n));
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
/* Робота з файлами в основі має потоки (Stream)
 * Потоки дозволяють маніпулювати з одним байтом, у т.ч. 
 * одразу мати справу з масивом байтів.
 * Для запису (чи читання) інших типів даних необхідно
 * вживати заходи з їх перетворення до бінарного виду.
 * При перетворенні чисел можливі два підходи:
 *  - пряме представлення (32 біти)
 *  - рядкове представлення "2342"
 *  
 * Потоки (файлові) є некерованими ресурсами і вимагають
 * закриття подачею команди (якщо не закрити, то платформа
 * цього зробити не зможе)
 * using - auto disposable - блок з автоматичним закриттям
 * 
 * Пряма робота з потоками незручна при збереженні різних 
 * типів даних. Тому вживаються "обгортки" StreamReader\StreamWriter
 * 
 * Серіалізація (від англ. - послідовний) - спосіб представити 
 * об'єкт у вигляді послідовності, що може збережена чи передана
 * послідовним каналом. Є різні форми серіалізації: бінарна та текстова.
 * Серед текстових форм найбільш поширена - JSON
 * 
 * Д.З. Реалізувати обмеження на розміри файлів-логів. Якщо файл сягає
 * граничного розміру (0.5кБ), то створюється новий файл з додаванням
 * лічильника (runlogs1, runlogs2, ...). Дії повторюються коли наступний
 * файл також сягає граничного розміру
 */

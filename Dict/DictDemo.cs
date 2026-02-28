using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharpKnP321.Dict
{
    internal class DictDemo
    {
        private const String filename = "dictionary.json";

        private Dictionary<String, String> dictionary;

        class MenuItem
        {
            public String Title { get; set; } = null!;
            public char Key { get; set; }
            public Action Action { get; set; } = null!;
            public override string ToString() => $"{Key}. {Title}";
        }
        private MenuItem[] menuItems;

        public DictDemo()
        {
            try {
                dictionary = JsonSerializer.Deserialize<Dictionary<String, String>>(
                    File.ReadAllText(filename))!;
                Console.Write("Завантажено ");
            }
            catch
            {
                dictionary = new()
                {//    Key   Value
                    { "cat", "кіт" },  // Pair
                    { "car", "авто" },
                    { "truck", "авто" },
                    { "catch", "ловити" },
                    { "go", "йти" },
                };
                Console.Write("Закладено ");
            }
            Console.WriteLine(dictionary.Count + " слів");

            menuItems = [
                new(){ Key = '1', Title = "Переклад слова з української до англійської", Action = Uk2En },
                new(){ Key = '2', Title = "Переклад слова з англійської до української", Action = En2Uk },
                new(){ Key = '3', Title = "Додати переклад до словника", Action = AddWord },
                new(){ Key = '4', Title = "Вивести весь словник", Action = PrintDictionary },
                new(){ Key = '0', Title = "Вихід з програми", Action = () => throw new Exception() },
            ];
        }
        
        public void Run()
        {
            try
            {
                while (true) { Menu(); }
            }
            catch { }
            finally
            {
                SaveDictionary();
            }
        }

        private void SaveDictionary()
        {
            File.WriteAllText(filename,
                JsonSerializer.Serialize(dictionary, 
                    new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = true
                    }));
        }

        public void Menu()
        {
            ConsoleKeyInfo key;
            MenuItem? selectedItem;
            do
            {
                Console.WriteLine("Словник");
                foreach(MenuItem item in menuItems)
                {
                    Console.WriteLine(item);
                }
                key = Console.ReadKey();
                selectedItem = menuItems
                    .FirstOrDefault(item => item.Key == key.KeyChar);
                Console.WriteLine();
                if (selectedItem == null) Console.WriteLine("Неправильний вибір");
                else selectedItem!.Action();
            } while (selectedItem == null);
        }

        private void PrintDictionary()
        {
            // Pagination - пагінація - поділ на сторінки
            // Вивести весь словник по N елементів, перехід до наступного - довільна клавіша
            // окрім ESC - переривання виведення (на етапі тестування N = 2-3)
        }

        private void AddWord()
        {
            Console.Write("Слово англійською: ");
            String en = Console.ReadLine()!;
            Console.WriteLine("Переклад українською: ");
            String uk = Console.ReadLine()!;
            try
            {
                dictionary.Add(en, uk);
                Console.WriteLine("Додано успішно");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Виникла помилка: " + ex.Message);
            }
        }

        private void Uk2En()
        {
            Console.Write("Введіть слово українською: ");
            String? word;
            do { word = Console.ReadLine()?.Trim(); }
            while (String.IsNullOrEmpty(word));

            var translations = dictionary
                .Where(pair => pair.Value == word);

            if(translations.Any())
            {
                foreach (var pair in translations)
                {
                    Console.WriteLine("{0} -- {1}", word, pair.Key);
                }
            }
            else
            {
                Console.WriteLine("Переклад не знайдено");
            }
        }

        private void En2Uk()
        {
            Console.Write("Введіть слово англійською: ");
            String? word;
            do { word = Console.ReadLine()?.Trim(); }
            while(String.IsNullOrEmpty(word));
            if (dictionary.ContainsKey(word))
            {
                Console.WriteLine("{0} -- {1}", word, dictionary[word]);
            }
            else
            {
                Console.WriteLine("Переклад не знайдено");
            }
        }
    }
}
/* Розширити роботу проєкту "Словник"
 * - реалізувати редагування наявних записів 
 *      (вводиться слово англійською, дається новий переклад українською)
 * - реалізувати видалення записів
 *  = за англійським словом
 *  = за українським словом
 * ** пошук асоціацій - якщо введене слово не знаходиться у словнику, то 
 *    здійснювати пошук найближчих до нього альтернатив. Рекомендується
 *    використати "відстань Левенштейна"(https://en.wikipedia.org/wiki/Levenshtein_distance)
 */

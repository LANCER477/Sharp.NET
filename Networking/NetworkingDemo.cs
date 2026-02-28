using SharpKnP321.Networking.Api;
using SharpKnP321.Networking.Orm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SharpKnP321.Networking
{
    internal class NetworkingDemo
    {
        public async Task Run()
        {
            Console.WriteLine("Робота з АРІ");
            MoonApi moonApi = new();
            MoonPhase todayPhase = moonApi.TodayPhaseAsync().Result;
            Console.WriteLine("{0} {1}", todayPhase.PhaseName, todayPhase.Lighting);

            MoonPhase tomorrowPhase = moonApi.PhaseByDateAsync(new DateOnly(2026, 2, 24)).Result;
            Console.WriteLine("{0} {1}", tomorrowPhase.PhaseName, tomorrowPhase.Lighting);


            Console.Write("Введіть дату (у форматі ДД.ММ.РРРР): ");
            string inputDate = Console.ReadLine();

            if (DateOnly.TryParseExact(inputDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly userDate))
            {
                MoonPhase userPhase = await moonApi.PhaseByDateAsync(userDate);
                Console.WriteLine($"Фаза на {userDate}: {userPhase.PhaseName}, Освітлення: {userPhase.Lighting}");
            }
            else
            {
                Console.WriteLine("Помилка: Невірний формат дати.");
            }
        }

        public async Task RunNbuHomeworkXmlAsync()
        {
            Console.WriteLine("\n--- ДЗ: Курси валют НБУ (XML) ---");
            Console.Write("Введіть дату (у форматі ДД.ММ.РРРР, наприклад 24.02.2022): ");
            string inputDate = Console.ReadLine();


            if (DateTime.TryParseExact(inputDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {

                if (parsedDate >= DateTime.Today)
                {
                    Console.WriteLine("Помилка: Дата має належати минулому!");
                    return;
                }


                string formattedDate = parsedDate.ToString("yyyyMMdd");
                string apiUrl = $"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date={formattedDate}";

                try
                {
                    using HttpClient client = new();
                    Console.WriteLine("Завантаження XML з НБУ...");
                    string xmlString = await client.GetStringAsync(apiUrl);


                    XDocument xmlDocument = XDocument.Parse(xmlString);

                    List<NbuRate> rates = [..
                        xmlDocument
                        .Descendants("currency")
                        .Select(c => new NbuRate
                        {
                            Txt = c.Element("txt")!.Value,
                            Rate = Convert.ToDouble(c.Element("rate")!.Value, CultureInfo.InvariantCulture),
                            Cc = c.Element("cc")!.Value,
                            R030 = Convert.ToInt32(c.Element("r030")!.Value),
                        })
                    ];

                    if (rates.Count > 0)
                    {
                        Console.WriteLine($"\nКурс НБУ на {parsedDate:dd.MM.yyyy}:");
                        Console.WriteLine("-------------------------------------------------");

                        var popularCodes = new[] { "USD", "EUR", "GBP", "PLN" };
                        foreach (var rate in rates.Where(r => popularCodes.Contains(r.Cc)))
                        {
                            Console.WriteLine($"{rate.Cc} ({rate.Txt}): \t{rate.Rate} грн");
                        }
                        Console.WriteLine("-------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine("НБУ не повернув даних на цю дату.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Помилка: Невірний формат дати.");
            }
        }

        public async Task RunHtmlDownloaderAsync()
        {
            Console.Write("Введіть адресу сайту (наприклад, google.com): ");
            string url = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(url)) return;

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка відкриття браузера: {ex.Message}");
            }

            try
            {
                using HttpClient client = new();
                string htmlCode = await client.GetStringAsync(url);

                Console.WriteLine("\n================ HTML CODE ================");
                if (htmlCode.Length > 1500)
                {
                    Console.WriteLine(htmlCode.Substring(0, 1500));
                    Console.WriteLine($"\n... [Код обрізано. Загальна довжина: {htmlCode.Length} символів] ...");
                }
                else
                {
                    Console.WriteLine(htmlCode);
                }
                Console.WriteLine("===========================================\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження HTML: {ex.Message}");
            }
        }

        public async Task RunXml()  // 
        {
            using HttpClient client = new();
            HttpRequestMessage request = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange"),
            };
            Task<HttpResponseMessage> responseTask =
                client.SendAsync(request);

            Console.WriteLine("Курси валют НБУ, робота з XML");
            Console.Write(DateTime.Now.Ticks / 10000 % 100000); Console.WriteLine(" request start");

            HttpResponseMessage response = await responseTask;
            Task<String> contentTask = response.Content.ReadAsStringAsync();

            Console.Write(DateTime.Now.Ticks / 10000 % 100000); Console.WriteLine(" request finish");
            String xmlString = await contentTask;
            // Console.WriteLine(xmlString);
            XDocument xmlDocument = XDocument.Parse(xmlString);
            // Console.WriteLine(xmlDocument.Descendants("currency").Count());
            List<NbuRate> rates = [..
                xmlDocument
                .Descendants("currency")
                .Select(c =>  new NbuRate
                {
                    Txt = c.Element("txt")!.Value,
                    Rate = Convert.ToDouble(c.Element("rate")!.Value, CultureInfo.InvariantCulture),
                    Cc = c.Element("cc")!.Value,
                    R030 = Convert.ToInt32(c.Element("r030")!.Value),
                })
            ];
            Console.WriteLine(String.Join('\n', rates));
        }

        public async Task RunJson()
        {
            // Курси валют НБУ, робота з JSON

            using HttpClient client = new();
            HttpRequestMessage request = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json"),
            };
            Task<HttpResponseMessage> responseTask =
                client.SendAsync(request);

            Console.WriteLine("Курси валют НБУ, робота з JSON");
            Console.Write(DateTime.Now.Ticks / 10000 % 100000); Console.WriteLine(" request start");

            HttpResponseMessage response = await responseTask;
            Task<String> contentTask = response.Content.ReadAsStringAsync();

            Console.Write(DateTime.Now.Ticks / 10000 % 100000); Console.WriteLine(" request finish");
            String jsonString = await contentTask;

            List<NbuRate> rates = NbuApi.ListFromJsonString(jsonString);

            foreach (NbuRate rate in rates)
            {
                Console.WriteLine(rate);
            }
        }

        public async Task RunStep()
        {
            using HttpClient client = new();
            HttpRequestMessage request = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://itstep.org/"),
            };
            Task<HttpResponseMessage> responseTask = client.SendAsync(request);

            Console.WriteLine("HTTP requests and responses");
            Console.Write(DateTime.Now.Ticks / 10000 % 100000); Console.WriteLine(" request start");

            HttpResponseMessage response = await responseTask;
            Task<String> contentTask = response.Content.ReadAsStringAsync();

            Console.Write(DateTime.Now.Ticks / 10000 % 100000); Console.WriteLine(" request finish");

            Console.WriteLine($"HTTP/{response.Version} {((int)response.StatusCode)} {response.ReasonPhrase}");
            foreach (var header in response.Headers)
            {
                Console.WriteLine("{0}: {1}", header.Key, String.Join(',', header.Value));
            }
            Console.WriteLine();
            Console.WriteLine(await contentTask);
        }

        public void RunBody()
        {
            HttpClient client = new();
            Task<String> getRequest = client.GetStringAsync("https://itstep.org/");  // вилучає тіло
            Console.Write(DateTime.Now.Ticks / 10000 % 100000);
            Console.WriteLine(" Get start");
            String requestBody = getRequest.Result;
            Console.WriteLine(DateTime.Now.Ticks / 10000 % 100000);
            Console.WriteLine(requestBody);
        }
    }
}
/* Д.З. Реалізувати відображення курсів валют на задану дану,
 * що її вводить користувач з клавіатури.
 * Забезпечити перевірку валідності дати, а також те, що вона
 * належить минулому
 * !! Використати формат XML
 * https://bank.gov.ua/ua/open-data/api-dev
 */
/* Д.З. Реалізувати завантажувач кодів сайтів:
 * Користувач вводить адресу сайту,
 * здійснюється запит, виводиться HTML код сайту, а також
 * запускається браузер з переходом на даний сайт (асинхронно)
 */
/* Робота мережею Інтернет
 * Мережа - сукупність вузлів та зв'язків між ними (каналів зв'язку)
 * Вузол (Node) - активний учасник, що перетворює дані (ПК, принтер, телефон, виконавчий пристрій тощо)
 * вузол у мережі відрізняється адресою та/або іменем
 * Зв'язок - спосіб передачі даних між вузлами (дріт, оптоволокно, радіоканал тощо)
 * НТТР - текстовий транспортий протокол
 * запит              відповідь
 * метод шлях         статус-код  фраза
 * заголовки - пари ключ: значення\r\n
 * тіло (довільна інформація), зокрема, JSON - текстовий протокол передачі даних
 * * CONNECT  службові
 * TRACE
 * * HEAD     технологічні
 * OPTIONS
 * * загальні CRUD - Create Read Update Delete
 * GET     одержання даних (читання, Read) -- без модифікації системи (без змін)
 * POST    створення нових елементів (Create)
 * DELETE  
 * PUT     заміна наявних даних на передані
 * PATCH   оновлення частини наявних даних
 * * галузеві стандарти
 * LINK
 * UNLINK
 * PURGE
 * MKCOL
 * * * JSON (JavaScript Object Notation)
 * - primitive: number, "string\u10AF", null, true, false
 * - array: [JSON, JSON, ...]
 * - object: {"key1": JSON, "key2": JSON, ...}
 * * XML:
 * <?xml version="1.0" encoding="utf-8"?>
 * <root>
       <currency>
            <r030>36</r030>
            <txt>Австралійський долар</txt>
            <rate>30.4814</rate>
            <cc>AUD</cc>
            <exchangedate>20.02.2026</exchangedate>
            <special>
            </special>
      </currency>
 * </root>
 * * <?xml version="1.0" encoding="utf-8"?>
 * <root exchangedate="20.02.2026">
        <currency r030="36" txt="Австралійський долар" rate="30.4814"/>
 * </root>
 */
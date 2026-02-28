using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http; // <--- Додано для завантаження HTML
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpKnP321.AsyncProgramming
{
    internal class AsyncProgramming
    {
        // --- Поля для прикладу з інфляцією ---
        private double sum;
        private int threadCnt;
        private readonly object sumLocker = new();
        private readonly object cntLocker = new();

        // --- Поля для Д.З. (Випадкові числа) ---
        private List<int> randomNumbers = new();
        private readonly object randomLocker = new();

        // Поля для Task Д.З.
        private int taskActiveCount;
        private readonly object taskCntLocker = new();

        public void Run()
        {
            ConsoleKeyInfo keyInfo;
            do
            {
                Console.Clear();
                Console.WriteLine("Async Programming: select an action");
                Console.WriteLine("1. Processes list");
                Console.WriteLine("2. Start notepad (Control Demo)");
                Console.WriteLine("3. Edit demo file (Params)");
                Console.WriteLine("4. Thread demo");
                Console.WriteLine("5. Multi Thread demo (Inflation)");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("6. HW: Process Launchers");
                Console.WriteLine("7. HW: MultiThread Random Numbers (With Join)");
                Console.WriteLine("8. HW: Launchers WITH ARGUMENTS");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("9. HW: Tasks Random Numbers (Task TPL)");
                Console.WriteLine("a. HW: Async/Await Random Numbers");
                Console.WriteLine("b. HW: String Tasks Chain (ContinueWith)");
                Console.WriteLine("c. HW: Async HTML Downloader"); // <--- НОВЕ ДЗ
                Console.WriteLine("----------------------------------");
                Console.WriteLine("0. Exit program");

                keyInfo = Console.ReadKey();
                Console.WriteLine();

                switch (keyInfo.KeyChar)
                {
                    case '0': return;
                    case '1': ProcessesDemo(); PressAnyKey(); break;
                    case '2': ProcessControlDemo(); PressAnyKey(); break;
                    case '3': ProcessWithParam(); PressAnyKey(); break;
                    case '4': ThreadsDemo(); PressAnyKey(); break;
                    case '5': MultiThread(); PressAnyKey(); break;
                    case '6': HomeworkProcessLaunchers(); PressAnyKey(); break;
                    case '7': HomeworkRandomThreads(); PressAnyKey(); break;
                    case '8': HomeworkLaunchWithArgs(); PressAnyKey(); break;
                    case '9': HomeworkRandomTasks(); PressAnyKey(); break;
                    case 'a': HomeworkAsyncAwaitLaunch(); PressAnyKey(); break;
                    case 'b': HomeworkStringChain(); PressAnyKey(); break;
                    case 'c': HomeworkHtmlDownloaderLaunch(); PressAnyKey(); break; // <--- Виклик нового методу
                    default: Console.WriteLine("Wrong choice"); PressAnyKey(); break;
                }
            } while (true);
        }

        private void PressAnyKey()
        {
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }

        #region HW: Async HTML Downloader
        /* Д.З. Реалізувати завантажувач кодів сайтів:
         * Користувач вводить адресу сайту, здійснюється запит, виводиться HTML код сайту, 
         * а також запускається браузер з переходом на даний сайт (асинхронно) */
        private void HomeworkHtmlDownloaderLaunch()
        {
            HomeworkHtmlDownloaderAsync().Wait();
        }

        private async Task HomeworkHtmlDownloaderAsync()
        {
            Console.WriteLine("\n--- Homework: HTML Downloader ---");
            Console.Write("Enter website URL (e.g., https://google.com): ");
            string url = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(url))
            {
                url = "https://example.com";
                Console.WriteLine($"Empty input. Using default: {url}");
            }
            else if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url; // Додаємо протокол, якщо користувач забув
            }

            Console.WriteLine("\nFetching HTML and opening browser asynchronously...\n");

            // 1. Асинхронний запуск браузера (ми не використовуємо await, 
            // щоб код пішов виконуватись далі, поки браузер відкривається у фоні)
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
                Console.WriteLine($"[Browser Launch Error]: {ex.Message}");
            }

            // 2. Асинхронний запит до сайту для отримання HTML
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string htmlCode = await client.GetStringAsync(url);

                    Console.WriteLine("================ HTML CODE ================");

                    // Виводимо перші 2000 символів, щоб консоль не "зависла" від гігантських сайтів
                    if (htmlCode.Length > 2000)
                    {
                        Console.WriteLine(htmlCode.Substring(0, 2000));
                        Console.WriteLine($"\n... [HTML truncated for readability. Total length: {htmlCode.Length} chars] ...");
                    }
                    else
                    {
                        Console.WriteLine(htmlCode);
                    }
                    Console.WriteLine("===========================================");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Network Error]: {ex.Message}");
            }
        }
        #endregion

        #region HW: String Tasks Chain (ContinueWith)
        private void HomeworkStringChain()
        {
            Console.WriteLine("\n--- Homework: String Processing Chain ---");
            Console.WriteLine("Enter a sentence to process:");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                input = "Hello world from Async Programming";
                Console.WriteLine($"Empty input. Using default: \"{input}\"");
            }

            Console.WriteLine("\nStarting Task Chain...");

            Task<string> task1 = Task.Run(() =>
            {
                string result = InvertWords(input);
                Console.WriteLine($"[Task 1 - Invert]: {result}");
                return result;
            });

            Task<string> task2 = task1.ContinueWith(prevTask =>
            {
                string result = CaesarCipher(prevTask.Result, 3);
                Console.WriteLine($"[Task 2 - Caesar]: {result}");
                return result;
            });

            Task<string> task3 = task2.ContinueWith(prevTask =>
            {
                string result = HideCharacters(prevTask.Result, '*');
                Console.WriteLine($"[Task 3 - Hide]  : {result}");
                return result;
            });

            task3.Wait();

            Console.WriteLine("\nChain finished.");
        }

        private string InvertWords(string text)
        {
            var words = text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                char[] charArray = words[i].ToCharArray();
                Array.Reverse(charArray);
                words[i] = new string(charArray);
            }
            return string.Join(" ", words);
        }

        private string CaesarCipher(string text, int shift)
        {
            char[] buffer = text.ToCharArray();
            for (int i = 0; i < buffer.Length; i++)
            {
                char letter = buffer[i];
                if (char.IsLetter(letter))
                {
                    char offset = char.IsUpper(letter) ? 'A' : 'a';
                    buffer[i] = (char)((letter + shift - offset) % 26 + offset);
                }
            }
            return new string(buffer);
        }

        private string HideCharacters(string text, char hideSymbol)
        {
            var words = text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                string w = words[i];
                if (w.Length >= 3)
                {
                    words[i] = w[0] + new string(hideSymbol, w.Length - 2) + w[w.Length - 1];
                }
            }
            return string.Join(" ", words);
        }
        #endregion

        #region HW: Async/Await Random Numbers
        private void HomeworkAsyncAwaitLaunch()
        {
            HomeworkRandomAsync().Wait();
        }

        private async Task HomeworkRandomAsync()
        {
            Console.WriteLine("\n--- Homework: Async/Await Numbers ---");
            Console.Write("Enter count of numbers to generate: ");

            if (int.TryParse(Console.ReadLine(), out int count) && count > 0)
            {
                randomNumbers = new List<int>();
                var tasks = new List<Task>();

                Console.WriteLine("Starting Async Tasks...");

                for (int i = 0; i < count; i++)
                {
                    tasks.Add(RandomAsyncWorker());
                }

                await Task.WhenAll(tasks);

                Console.WriteLine("\n--------------------------------");
                Console.WriteLine("All async tasks finished.");
                Console.WriteLine($"FINAL RESULT: [{string.Join(", ", randomNumbers)}]");
                Console.WriteLine("--------------------------------");
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }

        private async Task RandomAsyncWorker()
        {
            var rnd = new Random();
            int delay = rnd.Next(500, 2000);

            await Task.Delay(delay);

            int number = rnd.Next(10, 100);

            lock (randomLocker)
            {
                randomNumbers.Add(number);
                Console.WriteLine($"Async Task {Task.CurrentId} added {number}. List: [{string.Join(", ", randomNumbers)}]");
            }
        }
        #endregion

        #region HW: Tasks Random Numbers (Task TPL)
        private void HomeworkRandomTasks()
        {
            Console.WriteLine("\n--- Homework: Random Numbers via Tasks ---");
            Console.Write("Enter count of numbers to generate: ");

            if (int.TryParse(Console.ReadLine(), out int count) && count > 0)
            {
                randomNumbers = new List<int>();
                taskActiveCount = count;

                Console.WriteLine("Starting Tasks...");

                for (int i = 0; i < count; i++)
                {
                    Task.Run(RandomTaskWorker);
                }
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }

        private void RandomTaskWorker()
        {
            var rnd = new Random();
            int delay = rnd.Next(500, 2000);
            Thread.Sleep(delay);

            int number = rnd.Next(10, 100);
            bool isLast = false;

            lock (randomLocker)
            {
                randomNumbers.Add(number);
                Console.WriteLine($"Task {Task.CurrentId} added {number}. List: [{string.Join(", ", randomNumbers)}]");
            }

            lock (taskCntLocker)
            {
                taskActiveCount--;
                if (taskActiveCount == 0)
                {
                    isLast = true;
                }
            }

            if (isLast)
            {
                Console.WriteLine("\n--------------------------------");
                Console.WriteLine("Last Task Reporting:");
                Console.WriteLine($"FINAL RESULT: [{string.Join(", ", randomNumbers)}]");
                Console.WriteLine("--------------------------------");
                Console.WriteLine("Press any key to refresh menu...");
            }
        }
        #endregion

        #region HW: Launchers WITH ARGUMENTS
        private void HomeworkLaunchWithArgs()
        {
            Console.WriteLine("\n--- Homework: Launch With Arguments ---");
            Console.WriteLine("1. Notepad (Open specific file)");
            Console.WriteLine("2. Browser (Open specific search query)");
            Console.WriteLine("3. Media Player (Open specific file)");
            Console.Write("Select action: ");

            var key = Console.ReadKey().KeyChar;
            Console.WriteLine("\n");

            try
            {
                switch (key)
                {
                    case '1':
                        string fileName = "homework_args.txt";
                        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

                        if (!File.Exists(fullPath))
                        {
                            File.WriteAllText(fullPath, "Цей файл було створено автоматично та відкрито через аргументи процесу.");
                            Console.WriteLine($"[Created] File {fileName} created.");
                        }

                        Console.WriteLine($"Launching Notepad with argument: {fileName}");
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "notepad.exe",
                            Arguments = fullPath,
                            UseShellExecute = true
                        });
                        break;

                    case '2':
                        string searchQuery = "C# Task Parallel Library";
                        string targetUrl = $"https://www.google.com/search?q={searchQuery.Replace(" ", "+")}";

                        var browsers = Process.GetProcessesByName("chrome")
                            .Concat(Process.GetProcessesByName("msedge"))
                            .Concat(Process.GetProcessesByName("firefox")).ToArray();

                        if (browsers.Length > 0)
                            Console.WriteLine($"[Info] Found {browsers.Length} browser processes running.");
                        else
                            Console.WriteLine("[Info] No browser found. Starting new instance.");

                        Console.WriteLine($"Opening URL: {targetUrl}");
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = targetUrl,
                            UseShellExecute = true
                        });
                        break;

                    case '3':
                        Console.WriteLine("Enter full path to a music/video file (or press Enter for empty):");
                        Console.Write("> ");
                        string mediaPath = Console.ReadLine()?.Trim('"');

                        string playerExecutable = "wmplayer.exe";

                        if (string.IsNullOrWhiteSpace(mediaPath))
                        {
                            Process.Start(playerExecutable);
                        }
                        else
                        {
                            if (File.Exists(mediaPath))
                            {
                                Console.WriteLine($"Launching {playerExecutable} with file: {mediaPath}");
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = playerExecutable,
                                    Arguments = $"\"{mediaPath}\"",
                                    UseShellExecute = true
                                });
                            }
                            else
                            {
                                Console.WriteLine("File not found! Launching player anyway.");
                                Process.Start(playerExecutable);
                            }
                        }
                        break;

                    default:
                        Console.WriteLine("Unknown selection.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        #endregion

        #region HW: Process Launchers
        private void HomeworkProcessLaunchers()
        {
            Console.WriteLine("\n--- Homework: Launch Applications ---");
            Console.WriteLine("1. Notepad");
            Console.WriteLine("2. Browser (google.com)");
            Console.WriteLine("3. Calculator");
            Console.Write("Select app: ");

            var key = Console.ReadKey().KeyChar;
            Console.WriteLine();

            try
            {
                switch (key)
                {
                    case '1':
                        Process.Start("notepad.exe");
                        Console.WriteLine("Notepad launched.");
                        break;
                    case '2':
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://www.google.com",
                            UseShellExecute = true
                        });
                        break;
                    case '3':
                        Process.Start("calc.exe");
                        Console.WriteLine("Calculator launched.");
                        break;
                    default:
                        Console.WriteLine("Unknown app selection.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error launching process: {ex.Message}");
            }
        }
        #endregion

        #region HW: Random Numbers Collection (Thread with Join)
        private void HomeworkRandomThreads()
        {
            Console.Write("\nEnter count of numbers to generate: ");
            if (int.TryParse(Console.ReadLine(), out int count) && count > 0)
            {
                randomNumbers = new List<int>();
                List<Thread> threads = new List<Thread>();

                Console.WriteLine("Starting threads...");

                for (int i = 0; i < count; i++)
                {
                    var t = new Thread(RandomNumberThreadWorker);
                    threads.Add(t);
                    t.Start();
                }

                foreach (var t in threads)
                {
                    t.Join();
                }

                Console.WriteLine("\n--------------------------------");
                Console.WriteLine("All threads finished. Main thread reporting:");
                Console.WriteLine($"FINAL RESULT: [{string.Join(", ", randomNumbers)}]");
                Console.WriteLine("--------------------------------");
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }

        private void RandomNumberThreadWorker()
        {
            var rnd = new Random();
            int delay = rnd.Next(500, 2000);
            Thread.Sleep(delay);

            int number = rnd.Next(10, 100);

            lock (randomLocker)
            {
                randomNumbers.Add(number);
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} added {number}. List: [{string.Join(", ", randomNumbers)}]");
            }
        }
        #endregion

        #region Original Code
        private void MultiThread()
        {
            sum = 100.0;
            threadCnt = 12;
            Console.WriteLine($"Start Sum: {sum}");
            for (int i = 0; i < 12; i++)
            {
                new Thread(CalcMonth).Start(i + 1);
            }
        }

        private void CalcMonth(Object? month)
        {
            int m = (int)month!;
            Thread.Sleep(1000);
            double percent = m;
            double k = (1.0 + percent / 100.0);
            lock (sumLocker) sum *= k;
            bool isLast;
            lock (cntLocker)
            {
                threadCnt -= 1;
                isLast = threadCnt == 0;
            }
            if (isLast) Console.WriteLine($"Inflation Result for year: {sum:F2}");
        }

        private void ThreadsDemo()
        {
            Console.WriteLine("Thread created");
            var t = new Thread(ThreadActivity);
            Console.WriteLine("Thread start");
            t.Start();
        }

        private void ThreadActivity()
        {
            Console.WriteLine("ThreadActivity start");
            Thread.Sleep(3000);
            Console.WriteLine("ThreadActivity stop");
        }

        private void ProcessWithParam()
        {
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "demo.txt");
                if (!File.Exists(filePath)) File.WriteAllText(filePath, "Hello Async World!");
                var p = Process.Start(new ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    Arguments = filePath,
                    UseShellExecute = true
                });
                Console.WriteLine("Notepad with params started. Press any key to kill it...");
                Console.ReadKey();
                if (p != null && !p.HasExited)
                {
                    p.CloseMainWindow();
                    p.Kill();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void ProcessControlDemo()
        {
            try
            {
                Console.WriteLine("Starting Notepad...");
                Process process = Process.Start("notepad.exe");
                Console.WriteLine("Press any key to close Notepad...");
                Console.ReadKey();
                if (!process.HasExited)
                {
                    process.CloseMainWindow();
                    process.Kill(true);
                    process.WaitForExit();
                    process.Dispose();
                    Console.WriteLine("Notepad closed.");
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        }

        private void ProcessesDemo()
        {
            Process[] processes = Process.GetProcesses();
            Dictionary<String, int> proc = new Dictionary<string, int>();
            foreach (var process in processes)
            {
                if (proc.ContainsKey(process.ProcessName)) proc[process.ProcessName]++;
                else proc[process.ProcessName] = 1;
            }
            Console.WriteLine("Top 10 Processes by count:");
            foreach (var pair in proc.OrderByDescending(p => p.Value).ThenBy(p => p.Key).Take(10))
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
        }
        #endregion
    }
}
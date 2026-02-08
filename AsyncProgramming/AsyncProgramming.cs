using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SharpKnP321.AsyncProgramming
{
    internal class AsyncProgramming
    {
        // --- Поля для прикладу з інфляцією (з оригінального коду) ---
        private double sum;
        private int threadCnt;
        private readonly object sumLocker = new();
        private readonly object cntLocker = new();

        // --- Поля для Д.З. (Випадкові числа) ---
        private List<int> randomNumbers = new();
        private int randomThreadCount;
        private readonly object randomLocker = new();

        public void Run()
        {
            ConsoleKeyInfo keyInfo;
            do
            {
                Console.Clear(); // Трохи очистимо консоль для зручності
                Console.WriteLine("Async Programming: select an action");
                Console.WriteLine("1. Processes list");
                Console.WriteLine("2. Start notepad (Control Demo)");
                Console.WriteLine("3. Edit demo file (Params)");
                Console.WriteLine("4. Thread demo");
                Console.WriteLine("5. Multi Thread demo (Inflation)");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("6. HW: Process Launchers (Notepad, Browser, Calc)");
                Console.WriteLine("7. HW: MultiThread Random Numbers");
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
                    // Нові пункти Д.З.
                    case '6': HomeworkProcessLaunchers(); PressAnyKey(); break;
                    case '7': HomeworkRandomThreads(); PressAnyKey(); break;
                    default: Console.WriteLine("Wrong choice"); PressAnyKey(); break;
                }
            } while (true);
        }

        private void PressAnyKey()
        {
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }

        #region HW: Process Launchers
        /* Д.З. Реалізувати запуск процесів 
         * - блокнот 
         * - браузер (* з пошуком наявного) 
         * - калькулятор */
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
                        // Перевірка наявних процесів браузера
                        var chromeProcs = Process.GetProcessesByName("chrome");
                        var edgeProcs = Process.GetProcessesByName("msedge");
                        int totalBrowsers = chromeProcs.Length + edgeProcs.Length;

                        if (totalBrowsers > 0)
                            Console.WriteLine($"[Info] Found {totalBrowsers} running browser processes.");
                        else
                            Console.WriteLine("[Info] No active browser found. Starting new...");

                        // Запуск URL
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://www.google.com",
                            UseShellExecute = true // Важливо для .NET Core/5+ щоб відкрити URL
                        });
                        break;

                    case '3':
                        // Калькулятор (calc.exe працює як shim для UWP калькулятора)
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

        #region HW: Random Numbers Collection
        /* Д.З. Колекція випадкових чисел.
         * Користувач вводить кількість -> запуск потоків -> збір у колекцію -> вивід проміжних результатів.
         * Останній потік виводить фінал. */
        private void HomeworkRandomThreads()
        {
            Console.Write("\nEnter count of numbers to generate: ");
            if (int.TryParse(Console.ReadLine(), out int count) && count > 0)
            {
                // Ініціалізація змінних
                randomNumbers = new List<int>();
                randomThreadCount = count;

                Console.WriteLine("Starting threads...");

                for (int i = 0; i < count; i++)
                {
                    // Запускаємо потоки
                    new Thread(RandomNumberWorker).Start();
                }
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }

        private void RandomNumberWorker()
        {
           
            var rnd = new Random();
            int delay = rnd.Next(500, 2000); // 0.5 - 2 сек
            Thread.Sleep(delay);

            int number = rnd.Next(10, 100); // саме число

            bool isLast = false;

            // Критична секція для запису в список та зміни лічильника
            lock (randomLocker)
            {
                randomNumbers.Add(number);
                randomThreadCount--;

                // Вивід поточного стану колекції
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} added {number}. List: [{string.Join(", ", randomNumbers)}]");

                if (randomThreadCount == 0)
                {
                    isLast = true;
                }
            }

            
            if (isLast)
            {
                Console.WriteLine("\n--------------------------------");
                Console.WriteLine($"FINAL RESULT: [{string.Join(", ", randomNumbers)}]");
                Console.WriteLine("--------------------------------");
            }
        }
        #endregion

        #region Original Code (Inflation & Demos)

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
            // Console.WriteLine($"Request sent for month {m}");
            Thread.Sleep(1000);   // імітація АРІ-запиту
            double percent = m;
            double k = (1.0 + percent / 100.0);

            double tempSum;
            lock (sumLocker)
            {
                sum *= k;
                tempSum = sum;
            }
            // Console.WriteLine($"Response got for month {m}. Current Sum ~ {tempSum:F2}");

            bool isLast;
            lock (cntLocker)
            {
                threadCnt -= 1;
                isLast = threadCnt == 0;
            }
            if (isLast)
            {
                Console.WriteLine($"Inflation Result for year: {sum:F2}");
            }
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
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, "Hello Async World!");
                }

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void ProcessesDemo()
        {
            Process[] processes = Process.GetProcesses();
            Dictionary<String, int> proc = new Dictionary<string, int>();
            foreach (var process in processes)
            {
                if (proc.ContainsKey(process.ProcessName))
                {
                    proc[process.ProcessName]++;
                }
                else
                {
                    proc[process.ProcessName] = 1;
                }
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
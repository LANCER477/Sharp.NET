using System;
using System.Collections.Generic;
using System.Text;

namespace SharpKnP321.AsyncProgramming
{
    internal class Threading
    {
        public void Run()
        {
            Thread t10;
            CancellationTokenSource cts = new();   // об'єкт управління токенами
            try
            {
                t10 = new Thread(ThreadActivity);
                t10.Start(new ThreadData(10, cts.Token));

                new Thread(ThreadActivity).Start("Str");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Press a key");
            Console.ReadKey();
            // t10.Abort(); стара схема скасування потоків - знята з підтримки
            cts.Cancel();   // подати сигнал скасування на всі токени даного джерела
                            // Сам сигнал не зупиняє потоки, лише переводить токен до 
                            // скасованого стану. Потоки мають перевіряти це в середині себе
        }

        private void ThreadActivity(Object? arg)
        {
            try
            {
                if (arg is ThreadData data)   // Pattern matching
                {
                    String res = "";
                    StringBuilder sb = new();
                    for (int i = 0; i < data.N; i += 1)
                    {
                        Thread.Sleep(1000);   // імітація тривалих розрахунків
                        // res += i;          // !! Поєднання рядків у циклі
                        sb.Append(i);
                        Console.WriteLine("proceeded " + i);

                        // контрольована перевірка скасування потоку
                        data.CancellationToken.ThrowIfCancellationRequested();
                    }
                    res = sb.ToString();
                    Console.WriteLine(res);
                }
                else
                {
                    // throw new ArgumentException("arg should be int");  // НЕ потрапить до catch у Run
                    Console.WriteLine("Wrong call: arg should be int");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Виняток у потоці: " + ex.Message);
                // відкат - аварійне завершення роботи
            }
        }
    }

    internal record ThreadData(int N, CancellationToken CancellationToken) { }
}
/* Багатопоточність: управління виконанням
 * 
 *                                    поза catch
 * ---Run---try{}catch{}-|               [|]      Винятки існують у межах одного потоку
 *              \                         |       до інших потоків не передаються
 *               \------thread(10)--cw-|  |
 *                \------thread("Str")--throw-|
 *                
 * Скасування потоків
 * Старий підхід (на зараз не підтимується у C#)
 *  - thread.Cancel() виклик якого створював у потоці виняток
 *    Закладено традиції - потокові функції/методи повністю оточувати try-catch
 * Новий підхід - токени скасування   
 */

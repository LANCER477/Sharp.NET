using System;
using System.Collections.Generic;
using System.Text;

namespace SharpKnP321.AsyncProgramming
{
    internal class TaskDemo
    {
        public void Run()
        {
            Console.WriteLine("TaskDemo start");
            Task task = Task.Run(TaskAction);   // Запускає задачу асинхронно та повертає її об'єкт

            Task<String> taskString =           // Задачі дозволяють повертати значення з "обгорткою" Task<>
                // Task.Run(TaskStringAction);  // варіант для методу без параметрів
                Task.Run(() => TaskStringAction(2000));  // З метою передачі параметрів вживає лямбда
                                                         // кількість та тип параметрів не обмежується
            Task.Delay(500).Wait();
            Console.WriteLine("TaskDemo Wait finish");
            task.Wait();                        // Аналог Thread.Join()

            // Одержання результату задачі - .Result, при цьому також включається очікування
            Console.WriteLine($"taskString.Result = {taskString.Result}");

            var action = () => TaskStringAction(-1);
            try
            {
                Task.Run(action);
                // виняток у задачі не передається до точки виклику, проте
                // не спричинює аварійний стан (характерно для фонових потоків)
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Task.Delay(100).Wait();

            string action2() => TaskStringAction(-2);
            try
            {
                // Task.Run(() => TaskStringAction(-1)).Wait();
                Console.WriteLine( Task.Run(action2).Result );
                // Але якщо у try-catch додається очікування (або Wait() або Result)
                // то виняток у цей блок передається
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("TaskDemo finish");
        }

        private void TaskAction()
        {
            Console.WriteLine("TaskAction start");
            Task.Delay(1000).Wait();                 // для задач краще не вживати Thread.xxx методи
            Console.WriteLine("TaskAction finish");
        }

        private String TaskStringAction(int timeout)
        {
            if(timeout <= 0)
            {
                throw new ArgumentOutOfRangeException("timeout must be positive");
            }
            Console.WriteLine($"TaskStringAction {timeout} start");
            Task.Delay(timeout).Wait();
            Console.WriteLine($"TaskStringAction {timeout} finish");
            return "TaskStringAction";
        }
    }
}
/* Задачі - інструменти асинхронності, реалізовані на керованому рівні мови/платформи
 * = задачі зупиняються із зупинкою головного потоку
 * 
 * "Сніданок" у формалізмі задач
 * - три задачі, кожна повертає рядок "Subject ready" + мітку часу
 * - організувати правильний порядок запуску, очікування та виведення результатів задач
 * очікуваний результат:         неоптимальний синхронний:
 * 0.0 start                      0.0 start
 * 0.3 toast ready                0.3 toast ready
 * 0.5 beacon ready               0.8 beacon ready
 * 1.0 coffee ready               1.8 coffee ready
 * 
 */

/* Д.З. Повторити реалізацію попереднього ДЗ у формалізмі задач (Task)
 * 
 * Реалізувати формування колекції випадкових чисел.
 * Користувач вводить з консолі бажану кількість.
 * Здійснюється запуск паралельних задач, кожна з яких
 * "запитує" в сервісі випадкових чисел відповідну величину
 * (імітуємо затримкою в роботі та викликом вбудованого генератора вип. чисел)
 * та додає її до спільної колекції та виводить проміжний результат
 * (поточний стан колекції чисел).
 * Задача, що виконується останнім, виводить сформовану колекцію на екран.
 * 
 * Приблизний вигляд консолі:
 * > введіть кількість чисел для генерування: 3
 * [12, 21]
 * [12]
 * [12, 21, 45]
 * Результат: [12, 21, 45]
 */
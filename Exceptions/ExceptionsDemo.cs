using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Exceptions
{
    internal class ExceptionsDemo
    {
        public ExceptionsDemo()
        {
            // конструктори - методи без повернення
            return;  // переривання конструктора не перериває побудову об'єкта
            // єдиний спосіб зупинити побудову об'єкта - створити виняткову ситуацію
        }
        public void Run()
        {
            Console.WriteLine("Виняткові ситуації. Винятки. Exceptions");
            try
            {
                /* Д.З. Модифікувати роботу методу ExceptionsDemo::Run()
                 * реалізувати введення даних користувачем з консолі
                 * та виведення повідомлень про виняткові ситуації 
                 * з пропозицією повторного введення даних.
                 */
                String str = "2";
                Console.WriteLine("Sqrt of {0} = {1}", str, SqrtFromString(str));
            }
            catch(ArgumentOutOfRangeException)
            {
                Console.WriteLine("Аргумент не може бути перетворений до числа");
            } // варіант з блоком для декількох типів винятків
            catch(Exception ex) when (ex is ArgumentNullException || ex is ArgumentException)
            {
                Console.WriteLine("Зафіксовано NULL або порожній рядок в аргументі");
            }
            catch(InvalidOperationException)
            {
                Console.WriteLine("Від'ємні числа не підтримуються");
            }
        }
        public void Run1()
        {
            Console.WriteLine("Виняткові ситуації. Винятки. Exceptions");
            // this.ThrowableCode();   // 1. У .NET дозволяється лишати виклик
            // коду з винятками без обробників (у деяких мовах - ні)
            // 2. У такій формі виняток завершує роботу всієї програми
            try
            {
                this.ThrowableCode();
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine("Виникла виняткова ситуація " + ex.Message);
                return;
            }
            catch (IOException) // блоків catch може бути довільна кількість,
            {                   // перевірка сумісності іде з гори до низу
                Console.WriteLine("Unexpected exception");
            }
            catch   // Всі види винятків з ігноруванням об'єкту-винятку
            {
                // якщо виняток не обробляється, то рекомендується його передати далі
                throw;
            }
            finally   // блок finally виконується у будь-якому разі
            {         // навіть якщо було викликано return
                Console.WriteLine("Finally actions");
            }
        }

        private void ThrowableCode()
        {
            // створення виняткової ситуації
            // throw new ApplicationException("Launch of ThrowableCode");
            throw new LiteratureParseException("Unrecognized literature type");
        }

        private double SqrtFromString(string str)
        {
            ArgumentNullException.ThrowIfNull(str);  // скорочена форма наступного виразу
            // if (str == null)
            // {
            //     throw new ArgumentNullException(nameof(str));
            // }
            str = str.Trim();
            if(str == String.Empty)
            {
                throw new ArgumentException("Blank or empty data passed");
            }
            double result;
            try { result = Double.Parse(str); }
            catch { throw new ArgumentOutOfRangeException(nameof(str), "Argument must be valid float number"); }
            if(result < 0)
            {
                throw new InvalidOperationException("Negative values unsupported");
            }
            return Math.Sqrt(result);
        }
    }
}
/* Винятки (Exceptions)
 * Спосіб організації процесу виконання коду за якого процес може бути 
 * зупинений та переведений до режиму оброблення винятку.
 * 
 * У ранніх мовах програмування винятків не було, перевірка помилок
 * здійснювалась через запит спец. функцій на кшталт last_error()
 * 
 * В ООП з'являються ситуації, які неможна зупинити засобами return - 
 * конструктори об'єктів є методами без повернення. Вимагається 
 * інший спосіб переривання роботи.
 * 
 * Механізм:
 * - виняткова ситуація утворюється командою throw, яка приймає 
 *    об'єкт-виняток, який передається обробникам
 * - поява винятку зупиняє виконання коду з місця команди throw
 *    і відбувається пошук найближчого обробника. Якщо він знайдений,
 *    то управління передається до нього, якщо не знайдений, то
 *    весь процес (застосунок) зупиняється а аварійному режимі.
 * - обробники формуються блоками catch або finally   
 * 
 * Рекомендації:
 * - використовувати максимально конкретні реалізації винятків,
 *    не використовувати загальні на кшталт Exception, SystemException
 * - використовувати блок finally для закриття ресурсів
 * - розподіляти логіку, не перевантажувати один блок try великою
 *    кількістю задач
 * - розподіляти логіку обробників винятків - обробляти лише ті з них,
 *    які належать до "відповідальності" даного логічного блоку.
 *    method(){ date -----> get_by_date(date){query} }
 *                                              |
 *                                           обробляємо винятки по query
 *                                           не обробляємо по date
 * - якщо у програмі є точка входу (на кшталт Main), то бажано її 
 *    оточити обробником для непередбачених ситуацій
 * - не слід використовувати винятки для організації логіки алгоритмів,
 *    наприклад для визначення сусідних елементів масиву (у крайніх ел-тів
 *    лише по одному сусіду, в інших - по два)
 *    for(i=0,...
 *      try{ arr[i-1] }
 *      catch(IndexOutOfRangeException){ ignore }
 *   семантика винятку - це проблема, яку не можна вирішити.   
 *   
 * - якщо ми розробляємо бібліотеку або великий сервіс, то є сенс 
 *    створювати власні винятки замість стандартних системних
 */

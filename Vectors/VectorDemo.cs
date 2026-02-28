using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Vectors
{
    internal class VectorDemo
    {
        public void Run()
        {
            Console.WriteLine("Vectors Demo");
            Vector v1 = new() { X = 10.0, Y = 20.0 };
            Vector v2 = v1;       // перевірка семантики 
            v1.X = 30.0;          // чи відіб'ється зміна v1 на v2 ?
            Console.WriteLine(    // v1 = (30,0000;20,0000), v2 = (10,0000;20,0000) -- ні, об'єкти різні
                "v1 = {0}, v2 = {1}", v1, v2
            );
            // повторюємо експеримент для референсного типу
            RefVector r1 = new() { X = 10.0, Y = 20.0 };
            RefVector r2 = r1;
            r1.X = 30.0;
            Console.WriteLine(    // r1.x = 30, r2.x = 30  -- зміни синхронізовані - це один об'єкт
                "r1.x = {0}, r2.x = {1}", r1.X, r2.X
            );
            // оператори з новим типом
            Console.WriteLine("+v1 = {0}", +v1);              // +v1 = (30,0000;20,0000)
            Console.WriteLine("v1 + v2 = {0}", v1 + v2);      // v1 + v2 = (40,0000;40,0000)
            Console.WriteLine("-v1 = {0}", -v1);              // -v1 = (-30,0000;-20,0000)
            Console.WriteLine("v1 - v2 = {0}", v1 - v2);      // v1 - v2 = (20,0000;0,0000)
            Vector zero = new(0.0, 0.0);
            // оператори true, false викликаються тоді, коли об'єкт є умовою, зокрема, у тернарному виразі
            Console.WriteLine("zero vector is " + (zero ? "true" : "false")); // zero vector is false
            Console.WriteLine("v1 vector is " + (v1 ? "true" : "false"));     // v1 vector is true
            // Оператор ! визначається окремо
            Console.WriteLine("!v1 vector is " + (!v1 ? "true" : "false"));   // !v1 vector is false
            // Оператор інверсії (~) - визначений як відбиття від осі Y
            Console.WriteLine("~v1 = {0}", ~v1);              //  (-30,0000;20,0000)
            v1++;
            Console.WriteLine("v1++ => {0}", v1);             // v1++ => (31,0000;21,0000)
            v1--;
            Console.WriteLine("v1-- => {0}", v1);             // v1-- => (30,0000;20,0000)
            v1 += v2;    // до С# 14 скорочені оператори не перевантажуються, а створюються автоматично
            Console.WriteLine("v1 += v2 => {0}", v1);         // v1 += v2 => (40,0000;40,0000)
            v1 -= v2;
            Console.WriteLine("v1 * v2 = {0}", v1 * v2);      // v1 * v2 = 700
            Console.WriteLine("v1 / v2 = {0}", v1 / v2);      // v1 / v2 = 100

            Console.WriteLine("v1[0] = {0}, v1[1] = {1}", v1[0], v1[1]);      // v1[0] = 30, v1[1] = 20
            Console.WriteLine("v1[x] = {0}, v1[y] = {1}", v1["x"], v1["Y"]);  // v1[x] = 30, v1[y] = 20

        }
    }
}
/* Особливості роботи з Value Types
 * - базовими типами з семантикою "за значенням" є struct та record struct
 */

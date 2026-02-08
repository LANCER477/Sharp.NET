using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Vectors
{
    internal struct Vector   // struct - базовий тип для Value Type
    {
        public double X { get; set; }
        public double Y { get; set; }

        // Параметризований конструктор
        public Vector(double x, double y)
        {
            X = x; 
            Y = y;
        }

        public override string? ToString()   // задає рядкове подання - для конкатенації чи виведення
        {
            return $"({X:F4};{Y:F4})";
        }

        #region Operators
        // Унарні
        public static Vector operator +(Vector a) => new()
        {
            X = a.X,
            Y = a.Y,
        };
        public static Vector operator -(Vector a) => new()
        {
            X = -a.X,
            Y = -a.Y,
        };
        // оператори true, false викликаються тоді, коли об'єкт є умовою, зокрема, у тернарному виразі
        public static bool operator true(Vector a) => a.X != 0.0 || a.Y != 0.0;
        public static bool operator false(Vector a) => a.X == 0.0 && a.Y == 0.0;

        public static bool operator !(Vector a) => a.X == 0.0 && a.Y == 0.0;

        /// <summary>
        /// Mirrowing vector in Y line
        /// </summary>
        /// <param name="a">Original vector</param>
        /// <returns>Reflected vector</returns>
        public static Vector operator ~(Vector a) => new() { X = -a.X, Y = a.Y };

        public static Vector operator ++(Vector a) => new() { X = a.X + 1, Y = a.Y + 1 };
        public static Vector operator --(Vector a) => new() { X = a.X - 1, Y = a.Y - 1 };

        // нові (від C# 14) скорочені оператори: означається для об'єктів (не static)
        // public void operator +=(Vector a) => (X, Y) = (X + a.X, Y + a.Y);

        // окремим видом унарного оператора є індексатор []
        public double this[int index]
        {
            // switch-expression -- з поверненням значення
            get => index switch { 0 => X, 1 => Y, _ => throw new IndexOutOfRangeException("0 or 1 only") };
            set
            {   // switch-statement -- без повернення значення
                switch (index)
                {
                    case 0: X = value; break; 
                    case 1: Y = value; break;
                    default: throw new IndexOutOfRangeException("0 or 1 only");
                }
            }
        }
        // індексатори можуть працювати не тільки по числових індексах
        public double this[String index]
        {
            // switch-expression -- з поверненням значення
            get => index.ToLower() switch { "x" => X, "y" => Y, 
                _ => throw new IndexOutOfRangeException("x or y only") };
            set
            {   // switch-statement -- без повернення значення
                switch (index.ToLower())
                {
                    case "x": X = value; break;
                    case "y": Y = value; break;
                    default: throw new IndexOutOfRangeException("x or y only");
                }
            }
        }


        // Бінарні (не плутати з бітовими Bitwise)
        public static Vector operator +(Vector a, Vector b) => new()
        {
            X = a.X + b.X,
            Y = a.Y + b.Y,
        };
        public static Vector operator -(Vector a, Vector b) => new()
        {
            X = a.X - b.X,
            Y = a.Y - b.Y,
        };

        // Приклад зміни типу оператора - скалярний добуток
        public static double operator *(Vector a, Vector b) => 
            a.X * b.X + a.Y * b.Y;

        // Ділення вважатимемо множенням на обернений елемент: a/b = a * ~b
        public static double operator /(Vector a, Vector b) => a * ~b;

        // Залишок ділення - по компонентах
        public static Vector operator %(Vector a, Vector b) => new()
        {
            X = a.X % b.X,
            Y = a.Y % b.Y,
        };

        // Оператори бітового зсуву: << - ліворуч,  >> - праворуч,  >>> - циклічний
        public static Vector operator <<(Vector a, int n) => new()
        {
            X = a.X - n,
            Y = a.Y,
        };
        public static Vector operator >>(Vector a, int n) => new()
        {
            X = a.X + n,
            Y = a.Y,
        };
        public static Vector operator >>>(Vector a, int n) => new()
        {
            X = a.X + n,
            Y = -a.Y,
        };

        // Бінарні бітові оператори   & (and)   | (or)   ^ (xor)
        // не плутати з логічними (&&, ||) - вони не перевантажуються
        public static Vector operator &(Vector a, Vector b) => new()
        {
            X = Math.Max(a.X, b.X),
            Y = Math.Max(a.Y, b.Y),
        };
        public static Vector operator |(Vector a, Vector b) => new()
        {
            X = Math.Min(a.X, b.X),
            Y = Math.Min(a.Y, b.Y),
        };
        public static Vector operator ^(Vector a, Vector b) => new()
        {
            X = Math.Abs(Math.Min(a.X, b.X)),
            Y = Math.Abs(Math.Min(a.Y, b.Y)),
        };

        // Оператори порівняння та відношення - мають оголошуватись парами (< >), (==, !=), (<=, >=)
        public static bool operator ==(Vector a, Vector b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Vector a, Vector b) => !(a == b);

        #endregion
    }
}
/* Д.З. Створити структуру для роботи з дробами (чисельник, знаменник)
перевантажити необхідні оператори для арифметики з ними.
До звіту додати скріншот результатів виконання програми.
 */
using Microsoft.Data.SqlClient;
using SharpKnP321.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Data
{
    internal class DataDemo
    {
        public void Run()
        {
            DataAccessor dataAccessor = new();
            // Створити метод у DataAccessor, який виводитиме три товари, що 
            // є лідерами продажів за сьогодні (дата залежить від моменту запуску)
            // - за кількістю чеків
            // - за кількістю штук
            // - за сумою продажів
            Console.WriteLine("----------ByMoney-----------");
            foreach (var m in dataAccessor.Top3DailyProducts(CompareMode.ByMoney))
            {
                Console.WriteLine(m);
            }
            Console.WriteLine("---------ByChecks------------");

            foreach (var m in dataAccessor.Top3DailyProducts(CompareMode.ByChecks))
            {
                Console.WriteLine(m);
            }
            Console.WriteLine("--------ByQuantity-------------");

            foreach (var m in dataAccessor.Top3DailyProducts(CompareMode.ByQuantity))
            {
                Console.WriteLine(m);
            }
            Console.WriteLine("---------------------");

            foreach (var s in dataAccessor.EnumSales(10))
            {
                Console.WriteLine(s);
            }
            List<Sale> sales = [.. dataAccessor.EnumSales(10)];   // spread operator (..)
        }

        public void Run5()   // генератори - переваги та недоліки
        {
            DataAccessor dataAccessor = new();
            foreach (var dep in dataAccessor.EnumDepartments())
            {
                Console.WriteLine(dep);

                // System.InvalidOperationException:
                // There is already an open DataReader associated with this Connection
                // which must be closed first.
                String sql = $"SELECT * FROM Managers M WHERE M.DepartmentId = '{dep.Id}'";
                using SqlCommand cmd = new(sql, dataAccessor.connection);
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(dataAccessor.FromReader<Manager>(reader));
                }
            }
        }

        private long Fact(uint n)
        {
            if (n < 2) return 1;
            uint m = n - 1;
            return n * Fact(m);
        }

        public void Run4()
        {
            DataAccessor dataAccessor = new();
            List<Department> departments = dataAccessor.GetAll<Department>();
            List<Manager> managers = dataAccessor.GetAll<Manager>();

            // LINQ - робота з колекціями за аналогією з БД
            foreach(var name in departments.Select(d => d.Name))
            {
                Console.WriteLine(name);
            }
            Console.WriteLine(
                String.Join("\n",
                departments
                .GroupJoin(
                    managers,
                    d => d.Id,
                    m => m.DepartmentId,
                    (d, mans) => new
                    {
                        d.Name,
                        Cnt = mans.Count(),
                        Employee = String.Join("; ", mans.Select(m => m.Name))
                    })
                .OrderByDescending(item => item.Cnt)
                .Select(item => String.Format("{0} ({1} empl): {2}", item.Name, item.Cnt, item.Employee))
            ));
        }
        /* Д.З. Створити сутність Access(Id, ManagerId, Login, Salt, Dk)
         * Реалізувати відповідні методи Install, Seed
         * Забезпечити роботу GetAll<Access>() за умови, що таблиця матиме назву Accesses
         * Створити запит LINQ та відобразити: ім'я менеджера - його логін
         */

        public void Run3()
        {
            DataAccessor dataAccessor = new();
            // dataAccessor.Install();
            // dataAccessor.Seed();

            // dataAccessor.GetProducts().ForEach(Console.WriteLine);
            dataAccessor.GetAll<Product>().ForEach(Console.WriteLine);
            Console.WriteLine("---------------");

            //dataAccessor.GetDepartments().ForEach(Console.WriteLine);
            dataAccessor.GetAll<Department>().ForEach(Console.WriteLine);
            Console.WriteLine("---------------");

            //dataAccessor.GetManagers().ForEach(Console.WriteLine);
            dataAccessor.GetAll<Manager>().ForEach(Console.WriteLine);
            Console.WriteLine("---------------");

            // dataAccessor.GetNews().ForEach(Console.WriteLine);
            dataAccessor.GetAll<News>().ForEach(Console.WriteLine);
            Console.WriteLine("---------------");

        }

        public void Run2()
        {            
            DataAccessor dataAccessor = new();

            dataAccessor.MonthlySalesByManagersSql(year:2025, month: 1);
            Console.WriteLine("---------------");
            dataAccessor.MonthlySalesByManagersOrm(1).ForEach(Console.WriteLine);
            /*
             Д.З. Реалізувати вибірки статистики місячних продажів за товарами
            Назва товару -- кількість продажів
            Номер місяця (та рік) передавати параметрами до методу.
            Забезпечити реалізацію через ORM - створити модель (не використовувати
            модель, створену для менеджерів).
            Провести випробування, порівняти з безпосереднім виконанням SQL запиту
             */

            // Console.WriteLine(dataAccessor.RandomProduct());
            // Console.WriteLine(dataAccessor.RandomDepartment());
            // dataAccessor.GetProducts().ForEach(Console.WriteLine);

            // Console.Write("Кількість продажів за місяць (1-12): ");
            // String? input = Console.ReadLine();
            // // int value;
            // if(int.TryParse(input, out int value))   // side effect - зміна "оточення" - змінних поза тілом ф-ції
            // {
            //     try
            //     {
            //         Console.WriteLine(dataAccessor.GetSalesCountByMonth(value));
            //     }
            //     catch
            //     {
            //         Console.WriteLine("Введене значення не було оброблене");
            //     }
            // }
            // else
            // {
            //     Console.WriteLine("Введене значення не розпізнано як число");
            // }

            // dataAccessor.Install();
            // dataAccessor.Seed();
            // dataAccessor.FillSales();

            // List<Product> products = dataAccessor.GetProducts();

            // Вивести товари: назва -- ціна
            // - за зростанням ціни
            // - за спаданням ціни
            // - за абеткою
            // Вивести результати:
            //  - 3 найдорожчі товари
            //  - 3 найдешевші товари
            //  - 3 випадкові товари (з кожним запуском різні)

            // foreach(var p in products.OrderBy(p => p.Price))
            // {
            //     Console.WriteLine("{0} -- {1:F2}", p.Name, p.Price);
            // }

        }
        public void Run1()
        {
            Console.WriteLine("Data Demo");
            // Робота з БД проводиться у кілька етапів
            // І. Підключення.        raw string  
            String connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\samoylenko_d\Source\Repos\SharpKnP321\Database1.mdf;Integrated Security=True";
            
            // ADO.NET - інструментарій (технологія) доступу до даних у .NET
            SqlConnection connection = new(connectionString);
            // Особливість - утворення об'єкту не відкиває підключення
            try
            {
                connection.Open();   // підключення необхідно відкривати окремою командою
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Connection failed: " + ex.Message);
                return;
            }

            // II. Формування та виконання команди (SQL)
            String sql = "SELECT CURRENT_TIMESTAMP";
            // using (у даному контексті) - блок з автоматичним руйнуванням (AutoDisposable)
            using SqlCommand cmd = new(sql, connection);
            Object scalar;
            try
            {
                scalar = cmd.ExecuteScalar();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Command failed: {0}\n{1}", ex.Message, sql);
                return;
            }

            // III. Передача та оброблення даних від БД
            DateTime timestamp;
            timestamp = Convert.ToDateTime(scalar);
            Console.WriteLine("Res: {0}", timestamp);

            // IV. Закриття підключення, перевірка, що всі дані передані
            connection.Close();
        }
    }
}
/* Робота з даними на прикладі БД
 * БД зазвичай є відокремленим від проєкту сервісом, що вимагає окремого 
 * підключення та специфічної взаємодії. У Студії є спрощений сервіс БД
 * LocalDB  New Item -- Service Based DB -- Create
 * Знаходимо рядок підключення до БД через її властивості у Server Explorer
 * 
 * NuGet - система управління підключеними додатковими модулями (бібліотеками)
 *  проєкту C#.NET: Tools -- NuGet Package Manager -- Manage...
 * Знаходимо та встановлюємо Microsoft.Data.SqlClient - додаткові інструменти
 *  для взаємодії з СУБД MS SQLServer, у т.ч. LocalDB
 * 
 * ORM - Object Relation Mapping - відображення даних та їх зв'язків на об'єкти
 * (мови програмування) та їх зв'язки.
 * DTO - Data Transfer Object (Entity) - об'єкти (класи) для представлення даних
 * DAO - Data Access Object - об'єкти для оперування з DTO
 * 
 * 
 * Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\samoylenko_d\Source\Repos\SharpKnP321\Database1.mdf;Integrated Security=True
 */

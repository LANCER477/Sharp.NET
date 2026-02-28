using Microsoft.Data.SqlClient;
using SharpKnP321.Data.Attributes;
using SharpKnP321.Data.Dto;
using SharpKnP321.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Data
{
    public enum CompareMode
    {
        ByChecks,
        ByQuantity,
        ByMoney
    }

    internal class DataAccessor
    {
        public SqlConnection connection { get; private set; }

        #region About Properties
        // повністю автоматична
        public int Prop0 { get; set; }

        // тільки для читання - розрахункові дані на кшталт довжини вектора чи поточного часу
        public int Prop1 { get => 10; }

        // з різними аксесорами
        public int Prop3 { get; private set; }
        public int Prop4 { get; init; }

        // не автоматичнa, а визначенa користувачем
        private int _prop5;
        public int Prop5 {
            get { return _prop5; } 
            set
            {
                if(value != _prop5)   // Prop5 = 10 --> set(value=10)
                {
                    _prop5 = value;
                }
            } 
        }

        // тільки для запису - сідування, виведення (запуск процесів через присвоєння)
        private int _prop2;
        public int Prop2 { 
            set { _prop2 = value; } 
        }
        #endregion

        public DataAccessor()
        {
            // String connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\samoylenko_d\Source\Repos\SharpKnP321\Database1.mdf;Integrated Security=True";
            String connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Lector\source\repos\SharpKnP321\Database1.mdf;Integrated Security=True";
            this.connection = new(connectionString);
            try
            {
                this.connection.Open();   // підключення необхідно відкривати окремою командою
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Connection failed: " + ex.Message);
            }
        }

        public IEnumerable<ProdSaleModel> Top3DailyProducts(CompareMode compareMode)
        {
            return null;
        }

        public List<SaleModel> MonthlySalesByManagersOrm(int month, int year = 2025)
        {
            return ExecuteList<SaleModel>(@$"
            SELECT 
	            MAX(M.Name) AS [{nameof(SaleModel.ManagerName)}],
	            COUNT(S.ID) AS [{nameof(SaleModel.Sales)}]
            FROM 
	            Sales S
	            JOIN Managers M ON S.ManagerId = M.Id
            WHERE 
	            Moment BETWEEN @date AND DATEADD(MONTH, 1, @date)
            GROUP BY
	            M.Id
            ORDER BY 
	            2 DESC
            ", new() { ["@date"] = new DateTime(year, month, 1) });
        }

        public void MonthlySalesByManagersSql(int month, int year = 2025)
        {
            String sql = @"
            SELECT 
	            MAX(M.Name),
	            COUNT(S.ID) 
            FROM 
	            Sales S
	            JOIN Managers M ON S.ManagerId = M.Id
            WHERE 
	            Moment BETWEEN @date AND DATEADD(MONTH, 1, @date)
            GROUP BY
	            M.Id
            ORDER BY 
	            2 DESC
            ";
            using SqlCommand cmd = new(sql, connection);
            // String str = "Some Garbage";
            // str = null;
            // GC.Collect();
            cmd.Parameters.AddWithValue("@date", new DateTime(year, month, 1));
            try
            {
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("{0} -- {1}", reader[0], reader[1]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed: {0}\n{1}", ex.Message, sql);
            }
        }

        public int GetSalesCountByMonth(int month, int year = 2025)
        {
            // Параметризовані запити - з відокремленням параметрів від SQL тексту
            String sql = "SELECT COUNT(*) FROM Sales WHERE Moment BETWEEN @date AND DATEADD(MONTH, 1, @date)";
            using SqlCommand cmd = new(sql, connection);
            cmd.Parameters.AddWithValue("@date", new DateTime(year, month, 1));
            try
            {            
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch { throw; }
            /*
            Д.З. Створити метод, який видає порівняльну характеристику
            продажів за місяць (що вводиться параметром) на поточний рік 
            у порівнянні з попереднім роком. Поточний рік визначати з 
            реальної дати.
            Для повернення подвійного значення можна використати кортежі
            public (int, int) GetSalesInfoByMonth(int month)
            {
                .......
                return (1, 2);
            }
            ...
            var (m1, m2) = GetSalesInfoByMonth(1);
             */
        }

        public (int, int) GetSalesInfoByMonth(int month)
        {
            return (1, 2);
        }

        public T FromReader<T>(SqlDataReader reader)
        {
            // Generic - узагальнене програмування / на заміну шаблонному програмуванню (template)
            var t = typeof(T);                                       // Дані про тип, серед яких конструктори, властивості тощо
            var ctr = t.GetConstructor([]);                          // Знаходимо конструктор без параметрів ([])
            T res = (T) ctr!.Invoke(null);                           // Викликаємо конструктор, будуємо об'єкт
            foreach (var prop in t.GetProperties())                  // Перебираємо усі властивості об'єкту (типу)
            {                                                        // та намагаємось зчитати з даних (reader)
                try                                                  // таке ж ім'я поля 
                {                                                    // 
                    Object data = reader.GetValue(prop.Name);        // Зчитуємо дані, якщо їх немає, буде виняток
                    if(data.GetType() == typeof(decimal))            // Якщо тип даних decimal, то перетворюмо
                    {                                                // до double (для внутрішньої сумісності)
                        prop.SetValue(res, Convert.ToDouble(data));  // 
                    }                                                // 
                    else                                             // 
                    {                                                // 
                        prop.SetValue(res, data);                    // Для інших випадків - переносимо дані до 
                    }                                                // підсумкового об'єкту res

                }
                catch { }
            }
            return res;
        }

        private T ExecuteScalar<T>(String sql, Dictionary<String, Object>? sqlParams = null)
        {
            using SqlCommand cmd = new(sql, connection);
            foreach (var param in sqlParams ?? [])
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }
            try
            {
                using SqlDataReader reader = cmd.ExecuteReader();   // Reader - ресурс для передачі даних від БД до програми
                reader.Read();
                return FromReader<T>(reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed: {0}\n{1}", ex.Message, sql);
                throw;
            }
        }

        private List<T> ExecuteList<T>(String sql, Dictionary<String, Object>? sqlParams = null)
        {
            List<T> res = [];
            using SqlCommand cmd = new(sql, connection);
            foreach (var param in sqlParams ?? [])
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }
            try
            {
                using SqlDataReader reader = cmd.ExecuteReader();   // Reader - ресурс для передачі даних від БД до програми
                while (reader.Read())   // читаємо по одному рядку доки є результати
                {
                    res.Add(FromReader<T>(reader));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed: {0}\n{1}", ex.Message, sql);
                throw;
            }
            return res;
        }

        public Product RandomProduct()
        {            
            return ExecuteScalar<Product>(
                "SELECT TOP 1 * FROM Products ORDER BY NEWID()"
            );            
        }

        public Department RandomDepartment()
        {            
            return ExecuteScalar<Department>(
                "SELECT TOP 1 * FROM Departments ORDER BY NEWID()"
            );            
        }
        
        // Реалізувати метод вибору випадкового менеджера, провести випробування

        public List<Product> GetProducts()
        {
            return ExecuteList<Product>(
                "SELECT * FROM Products"
            );
        }

        public List<Department> GetDepartments()
        {
            return ExecuteList<Department>(
                "SELECT * FROM Departments"
            );
        }

        // варіант з генератором
        public IEnumerable<Department> EnumDepartments()
        {
            // генератори - спосіб одержання результатів "по одному"
            // без необхідності формування масиву чи колекції
            // генератори дозволяють одну операцію - одержання наступного елементу
            // через яку можуть ітеруватись, зокрема циклом foreach
            String sql = "SELECT * FROM Departments";
            using SqlCommand cmd = new(sql, connection);
            SqlDataReader? reader;
            try { reader = cmd.ExecuteReader(); }
            catch (Exception ex)
            {
                Console.WriteLine("Failed: {0}\n{1}", ex.Message, sql);
                throw;
            }
            while (reader.Read())   // читаємо по одному рядку доки є результати
            {
                yield return FromReader<Department>(reader);
            }
            reader.Dispose();
        }

        // Поєднати (за логікою) усі методи-генератори, створити узагальнений
        // EnumAll<T>(), у т.ч. для News

        public IEnumerable<Sale> EnumSales(int limit = 100)
        {
            String sql = "SELECT * FROM Sales ";
            using SqlCommand cmd = new(sql, connection);
            SqlDataReader? reader;
            try { reader = cmd.ExecuteReader(); }
            catch (Exception ex)
            {
                Console.WriteLine("Failed: {0}\n{1}", ex.Message, sql);
                throw;
            }

            try
            {
                while (reader.Read())   // читаємо по одному рядку доки є результати
                {
                    yield return FromReader<Sale>(reader);

                    // код відновлюється з місця, на якому був зупинений, тобто після yield return
                    limit -= 1;
                    if (limit == 0)
                    {
                        yield break;   // оператор переривання (зупинки) генератора
                    }
                }
            }
            finally
            {
                reader.Dispose();   // ?? Як поведеться reader при закритті за наявності непереданих даних?
                                    // ! закривається без проблем
            }
            
        }


        public List<Manager> GetManagers()
        {
            return ExecuteList<Manager>(
                "SELECT * FROM Managers"
            );
        }

        public List<News> GetNews()
        {
            return ExecuteList<News>(
                "SELECT * FROM News"
            );
        }

        // Створити узагальнений метод GetAll<T>(), що "поєднує" GetProducts, GetDepartments, GetManagers 
        public List<T> GetAll<T>()
        {
            var t = typeof(T);
            // перевіряємо чи є у типу Т атрибут, що визначає специфічне ім'я для таблиці
            var attr = t.GetCustomAttribute<TableNameAttribute>();
            String tableName = attr?.Value  ??  t.Name + "s";
            return ExecuteList<T>(
                $"SELECT * FROM {tableName}"
            );
        }


        public void Install()
        {
            InstallProducts();
            InstallDepartments();
            InstallManagers();
            InstallSales();
            InstallNews();
        }
        private void InstallSales()
        {
            String sql = "CREATE TABLE Sales(" +
                "Id        UNIQUEIDENTIFIER PRIMARY KEY," +
                "ManagerId UNIQUEIDENTIFIER NOT NULL," +
                "ProductId UNIQUEIDENTIFIER NOT NULL," +
                "Quantity  INT              NOT NULL  DEFAULT 1," +
                "Moment    DATETIME2        NOT NULL  DEFAULT CURRENT_TIMESTAMP)";
            using SqlCommand cmd = new(sql, connection);
            try
            {
                cmd.ExecuteNonQuery();   // без зворотнього результату
            }
            catch (Exception ex)
            {
                Console.WriteLine("Command failed: {0}\n{1}", ex.Message, sql);
            }
        }
        private void InstallManagers()
        {
            String sql = "CREATE TABLE Managers(" +
                "Id           UNIQUEIDENTIFIER PRIMARY KEY," +
                "DepartmentId UNIQUEIDENTIFIER NOT NULL," +
                "Name         NVARCHAR(64)     NOT NULL," +
                "WorksFrom    DATETIME2        NOT NULL  DEFAULT CURRENT_TIMESTAMP)";
            using SqlCommand cmd = new(sql, connection);
            try
            {
                cmd.ExecuteNonQuery();   // без зворотнього результату
            }
            catch (Exception ex)
            {
                Console.WriteLine("Command failed: {0}\n{1}", ex.Message, sql);
            }
        }
        private void InstallDepartments()
        {
            String sql = "CREATE TABLE Departments(" +
                "Id    UNIQUEIDENTIFIER PRIMARY KEY," +
                "Name  NVARCHAR(64)     NOT NULL)";
            using SqlCommand cmd = new(sql, connection);
            try
            {
                cmd.ExecuteNonQuery();   // без зворотнього результату
            }
            catch (Exception ex)
            {
                Console.WriteLine("Command failed: {0}\n{1}", ex.Message, sql);
            }
        }
        private void InstallProducts()
        {
            String sql = "CREATE TABLE Products(" +
                "Id    UNIQUEIDENTIFIER PRIMARY KEY," +
                "Name  NVARCHAR(64)     NOT NULL," +
                "Price DECIMAL(14,2)    NOT NULL)";
            using SqlCommand cmd = new(sql, connection);
            try
            {
                cmd.ExecuteNonQuery();   // без зворотнього результату
            }
            catch (Exception ex)
            {
                Console.WriteLine("Command failed: {0}\n{1}", ex.Message, sql);
            }
        }
        private void InstallNews()
        {
            String sql = "CREATE TABLE News(" +
                "Id       UNIQUEIDENTIFIER PRIMARY KEY," +
                "AuthorId UNIQUEIDENTIFIER NOT NULL," +
                "Title    NVARCHAR(256)    NOT NULL," +
                "Content  NVARCHAR(MAX)    NOT NULL," +
                "Moment   DATETIME2        NOT NULL  DEFAULT CURRENT_TIMESTAMP)";
            using SqlCommand cmd = new(sql, connection);
            try
            {
                cmd.ExecuteNonQuery();   // без зворотнього результату
            }
            catch (Exception ex)
            {
                Console.WriteLine("Command failed: {0}\n{1}", ex.Message, sql);
            }
        }


        public void Seed()
        {
            SeedProducts();
            SeedDepartments();
            SeedManagers();
            SeedNews();
        }
        public void SeedManagers()
        {
            String sql = "INSERT INTO Managers VALUES" +
                "('303AB470-798A-423B-9701-D94DD8A5B65A', 'C7727779-9EE3-4127-988E-F7E93A780204', N'Havrylova, Mykola', '2001-01-01')," +
                "('F533D6BE-3C59-4B0E-9747-075840B93F18', 'C7727779-9EE3-4127-988E-F7E93A780204', N'Bodnarov, Svyatoslav', '2002-01-01')," +
                "('5A5AE21A-4A1A-4D54-87C8-4AFF8000F7F8', 'C7727779-9EE3-4127-988E-F7E93A780204', N'Stepanenko, Hanna', '2003-01-01')," +
                "('6CBBFCE2-43B6-4ACB-BA25-E93E15A478F0', 'C7727779-9EE3-4127-988E-F7E93A780204', N'Lyubchenko, Anatoliy', '2004-01-01')," +
                "('A95B1703-B4B8-4962-83E9-A5B19F0A68C1', 'C7727779-9EE3-4127-988E-F7E93A780204', N'Havrylenko, Olena', '2005-01-01')," +
                "('BD8B9C14-F11C-497C-B3BE-3AA88BF107B8', '451DD3B1-2287-4881-B66A-F5B3849B677C', N'Pavlenko, Hryhoriy', '2006-01-01')," +
                "('1D581863-4E37-4EE9-BE72-03A612F83C82', '451DD3B1-2287-4881-B66A-F5B3849B677C', N'Savenko, Mikola', '2007-01-01')," +
                "('86A9FC19-4F47-4BD8-B07F-CEBB6E51D8C7', '451DD3B1-2287-4881-B66A-F5B3849B677C', N'Pavlenko, Yana', '2008-01-01')," +
                "('36726ECC-120C-491B-AD53-2AC3C0FFD752', '451DD3B1-2287-4881-B66A-F5B3849B677C', N'Chernyshov, Kateryna', '2009-01-01')," +
                "('4B388C74-87F8-4F54-85B3-41E13035F0B1', '451DD3B1-2287-4881-B66A-F5B3849B677C', N'Ivanenko, Liudmyla', '2010-01-01')," +
                "('93FE7CA7-A98D-4A0A-89C2-F2C78B1B6C5B', '451DD3B1-2287-4881-B66A-F5B3849B677C', N'Pavlenko, Iryna', '2011-01-01')," +
                "('4B537970-D83A-4F56-AF04-335FEBC472FA', '8C51535C-26E3-4B8A-9F7C-7D669C4672AE', N'Ivanenko, Dmytro', '2012-01-01')," +
                "('4036121A-918E-43C6-8DD3-9DC9CADA8246', '8C51535C-26E3-4B8A-9F7C-7D669C4672AE', N'Stepanenko, Mariya', '2013-01-01')," +
                "('92904E45-1EF8-4E84-94C9-6DB06B2897E4', '8C51535C-26E3-4B8A-9F7C-7D669C4672AE', N'Maksimov, Oleh', '2014-01-01')," +
                "('794E6929-198D-4E6A-BF3E-C03B9DC82C8F', '8C51535C-26E3-4B8A-9F7C-7D669C4672AE', N'Kovalenko, Oksanna', '2015-01-01')," +
                "('54F6094F-3995-45A7-8DB7-A0FEF3FC9B3C', '8C51535C-26E3-4B8A-9F7C-7D669C4672AE', N'Koval, Andriy', '2016-01-01')," +
                "('DDF6691A-1EF1-4B38-B9A9-1374D11E1201', '8C51535C-26E3-4B8A-9F7C-7D669C4672AE', N'Kovalenko, Ihor', '2017-01-01')," +
                "('C2ED99C7-7B8B-42A3-8B79-0D685B107D19', 'B4C174CC-8C18-46DF-B8B4-F9E6F51EDCEA', N'Havrylov, Mikola', '2018-01-01')," +
                "('E2FF1A07-20A5-463D-A885-5EFF83E97873', 'B4C174CC-8C18-46DF-B8B4-F9E6F51EDCEA', N'Stepanenko, Kostyantyn', '2019-01-01')," +
                "('3536B5C8-65CA-49D1-9871-3A5EBBF67287', 'B4C174CC-8C18-46DF-B8B4-F9E6F51EDCEA', N'Bodnarova, Hryhoriy', '2020-01-01')," +
                "('376BC68A-74AC-477C-8392-8EC22CD8322B', 'B4C174CC-8C18-46DF-B8B4-F9E6F51EDCEA', N'Chernyshova, Iuliya', '2019-01-01')," +
                "('24CCDE5A-6334-461E-B9C7-A3E1D60B38F7', 'B4C174CC-8C18-46DF-B8B4-F9E6F51EDCEA', N'Lyubchenko, Anatoliy', '2018-01-01')," +
                "('2E9B856A-B14A-4257-B68D-4F43D1EA6A4C', 'B4C174CC-8C18-46DF-B8B4-F9E6F51EDCEA', N'Chernyshov, Taras', '2017-01-01')," +
                "('ABA637A2-F523-4985-9BD0-B3B2C31F0164', 'B4C174CC-8C18-46DF-B8B4-F9E6F51EDCEA', N'Havrylov, Oleksandr', '2016-01-01')," +
                "('E2F3E4AD-C43B-43C9-81B0-8BE13457A70C', 'B4C174CC-8C18-46DF-B8B4-F9E6F51EDCEA', N'Lyubchenko, Aleh', '2015-01-01')," +
                "('1560BD42-A722-4535-A151-BD3BD868C10D', 'B4C174CC-8C18-46DF-B8B4-F9E6F51EDCEA', N'Pavlenko, Volodymyr', '2014-01-01')," +
                "('E76C8F99-D4E5-4203-B4EA-AA37C806D8E0', 'B471180C-B4C0-4DF3-9290-D7DE881C94C7', N'Babych, Liudmyla', '2013-01-01')," +
                "('23A6B3F0-9782-4710-87EC-1B210CB2C019', 'B471180C-B4C0-4DF3-9290-D7DE881C94C7', N'Bodnar, Anastasiya', '2012-01-01')";
            using SqlCommand cmd = new(sql, connection);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Command failed: {0}\n{1}", ex.Message, sql);
            }
        }
        public void SeedDepartments()
        {
            String sql = "INSERT INTO Departments VALUES" +
                "('C7727779-9EE3-4127-988E-F7E93A780204', N'Відділ маркетингу')," +
                "('451DD3B1-2287-4881-B66A-F5B3849B677C', N'Відділ реклами')," +
                "('8C51535C-26E3-4B8A-9F7C-7D669C4672AE', N'Відділ продажів')," +
                "('B4C174CC-8C18-46DF-B8B4-F9E6F51EDCEA', N'ІТ відділ')," +
                "('B471180C-B4C0-4DF3-9290-D7DE881C94C7', N'Служба безпеки')";
            using SqlCommand cmd = new(sql, connection);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Command failed: {0}\n{1}", ex.Message, sql);
            }
        }
        public void SeedProducts()
        {
            String sql = "INSERT INTO Products VALUES" +
                "('1C87B849-12F8-43A8-802F-89578FF1E6DC', N'Samsung Galaxy S24 Ultra', 20000.00)," +
                "('86352131-DB1F-4DCC-98EE-25752269AB62', N'Google Pixel 8 Pro', 10000.00)," +
                "('2E64C765-79EB-44BB-AC8A-0673864BDDD0', N'iPhone 15 Pro', 30000.00)," +
                "('71EEA200-E5A6-48CC-A1C8-98FA1989DD72', N'OnePlus 12', 15000.00)," +
                "('02B4C962-2342-471E-9FC0-393ACD01B90F', N'Samsung Galaxy Note 10 Lite', 9000.00)," +
                "('4E654CB6-23DA-452E-BE2E-E290ECCA61FE', N'Xiaomi Poco X3 NFC', 11000.00)," +
                "('B53CC6C7-CE42-4409-9A5B-BB359AF0AE34', N'ASUS Rog Phone 3', 18000.00)," +
                "('71238977-8557-46C1-A169-525A48A2D671', N'OnePlus 8 Pro', 7000.00)," +
                "('76B32D09-AFAF-4069-B0B1-6E6FDC96FADB', N'Xiaomi Redmi Note 8 Pro', 13000.00)," +
                "('F835776E-D45F-41DA-AD6F-60BA9F7835AF', N'Apple iPhone 12', 15000.00)," +
                "('106DCBF7-3E1C-46A0-BBD5-260CE332039E', N'OUKITEL WP5', 11000.00)," +
                "('012DD183-2E42-49EF-BB02-4CC0F5A60F59', N'Samsung Galaxy S10', 16000.00)," +
                "('3D01E555-6B5D-4EDF-B00D-2270F51EFBDC', N'Apple iPhone 11', 12500.00)," +
                "('ED94CE4B-183D-416D-BD0F-CF876603EEA1', N'Samsung Galaxy A51', 11100.00)," +
                "('725AE0FC-9D91-47D4-8F10-DA09E26D5F62', N'Nokia 5.1 16GB Android One', 16700.00)," +
                "('84F206BB-B3EC-4333-8EA2-9874B3F801A8', N'ZTE Blade A3 (2020) NFC', 12100.00)," +
                "('968CE4DF-21D2-4E55-81C7-0311F19F3E47', N'HUAWEI P40 Pro', 14000.00)";
            using SqlCommand cmd = new(sql, connection);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Command failed: {0}\n{1}", ex.Message, sql);
            }
        }
        public void SeedNews()
        {
            String sql = "INSERT INTO News VALUES" +
            "('488CD481-E5FF-4594-8F67-27AF6B723834', '36726ECC-120C-491B-AD53-2AC3C0FFD752', N'What happens to your body if you only eat fruit?', N'When it comes to dieting, there is no shortage of options to try, from plant-based to keto. Occasionally, people try the fruitarian diet, which involves eating primarily fruits. Even Apple founder Steve Jobs dabbled with this way of eating. But what happens to your body if you only eat fruit? While only eating whole, natural foods from the earth sounds healthy, this diet can cause many health problems.', '2025-11-22')," +
            "('7D1AB47C-BDFD-49E9-B8FF-6E982E10A32F', '4B388C74-87F8-4F54-85B3-41E13035F0B1', N'Wife wanted!',                                     N'Aristocrat, 79, launches bid to find a Lady who is 20 years younger and can fire a gun', '2025-11-23')," +
            "('0E7B6B39-84A3-4B54-A90B-B7EFBE7DA44C', '93FE7CA7-A98D-4A0A-89C2-F2C78B1B6C5B', N'The most beautiful cars ever made',                N'We now have more than 120 years’ worth of cars to enjoy now – some memorably ugly ones too – and that, very obviously, makes it ever harder for new models to break into this list which we produce every few years', '2025-11-24')";
            using SqlCommand cmd = new(sql, connection);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Command failed: {0}\n{1}", ex.Message, sql);
            }
        }
        public void FillSales()
        {
            String sql = "INSERT INTO Sales(Id, ManagerId, ProductId, Quantity, Moment) VALUES" +
                "( NEWID(), " +
                "  ( SELECT TOP 1 Id FROM Managers ORDER BY NEWID() ), " +
                "  ( SELECT TOP 1 Id FROM Products ORDER BY NEWID() ), " +
                "  ( SELECT 1 + ABS(CHECKSUM(NEWID())) % 10 ), " +
                "  ( SELECT DATEADD(MINUTE, ABS(CHECKSUM(NEWID())) % 525600, '2025-01-01') )  " +
                ")";
            using SqlCommand cmd = new(sql, connection);
            try
            {
                for (int i = 0; i < 1e6; i++)
                {
                    cmd.ExecuteNonQuery();
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Command failed: {0}\n{1}", ex.Message, sql);
            }
        }
    }
}



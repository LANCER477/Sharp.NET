using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Data.Dto
{
    internal class Product
    {
        public Guid Id { get; set; }
        public String Name { get; set; } = null!;
        public double Price { get; set; }

        public static Product FromReader(SqlDataReader reader)
        {
            return new()
            {
                Id = reader.GetGuid("Id"),   // using System.Data;
                Name = reader.GetString("Name"),
                Price = Convert.ToDouble(reader.GetDecimal("Price"))
            };
        }

        public override string ToString()
        {
            return $"{Id.ToString()[..3]}... {Name} {Price:F2}";
        }
    }
}
/* DTO - Data Transfer Object (Entity) - об'єкти (класи) для представлення даних
 * Відображення рядка таблиці БД (Products)
 */
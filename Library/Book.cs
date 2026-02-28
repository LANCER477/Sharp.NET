using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharpKnP321.Library
{
    public class Book : Literature
    {
        [JsonInclude]
        // auto-property - повністю реалізується автоматично
        public String Author { get; set; } = null!;

        #region property - з явною реалізацією
        private int _year;
        public int Year {
            get => _year;
            set
            {
                if (_year != value)   // value - значення, що приходить на заміну
                {                     // в інструкції book.Year = 2000
                    _year = value;    // (value = 2000)
                }
            }
        }
        #endregion

        public override string GetCard()
        {
            return $"{this.Author}. {base.Title} - {base.Publisher} - {this.Year}";
        }
    }
}
/* 
Modelling:         Generalization:     Relations:

Book         \ 
Journal      -----> Literature <----<>  Library
Newspaper    /

Encapsulation ----  Abstraction ------- Polymorphism      
           Inheritance
           Extension

Encapsulation -- 2
Book          Journal       Newspaper 
  Author        Publisher     Publisher
  Title         Title         Title
  Year          Number        Date
  Publisher

            Literature
              Publisher
              Title
              GetCard()
        /       |         \ 
   Book     Journal     Newspaper
     Author   Number      Date
     Year


Д.З. Додати до переліку літератури об'єкт "Booklet"
- вивчити предметну область
- виділити необхідні властивості
- описати клас, додати до "фондів" бібліотеки
- переконатись у правильній роботі
Додати скріншот до звіту з ДЗ
 */

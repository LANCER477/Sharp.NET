using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Library
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class ColorPrintAttribute : Attribute
    {
        public int Copies { get; set; } = 1;
        
    }
    
}
/* Створити атрибут ApaStyle який зазначає метод, який 
 * виводить картку у стилі APA
 * Автор (рік) Назва. Видавець
 * 
 * Створити метод у класі Book, додавши атрибут
 * 
 * Створити метод у Library, який виводить всі АРА картки
 * 
 * Модифікувати атрибут: створити CiteStyleAttribute до 
 * якого додати властивість зі значенням стилю.
 * Розширити методи з різними стилями цитування
 * 
 * [CiteStyle("APA")]
 * public .... 
 * 
 * [CiteStyle("IEEE")]
 * public .... 
 * 
 * https://instr.iastate.libguides.com/standards/citing
 * 
 * Створити методи у Library, які виводять картки різних стилей 
 * PrintApaCards()
 * PrintIeeeCards()
 */
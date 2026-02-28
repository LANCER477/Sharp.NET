using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharpKnP321.Library
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(Book),      typeDiscriminator: "Book")]
    [JsonDerivedType(typeof(Booklet),   typeDiscriminator: "Booklet")]
    [JsonDerivedType(typeof(Hologram),  typeDiscriminator: "Hologram")]
    [JsonDerivedType(typeof(Journal),   typeDiscriminator: "Journal")]
    [JsonDerivedType(typeof(Newspaper), typeDiscriminator: "Newspaper")]
    public abstract class Literature
    {
        public String Publisher { get; set; } = String.Empty;
        public String Title { get; set; } = null!;
        public abstract String GetCard();
    }
}
/* NULL-safety - прямий розподіл типів, які дозволяють значення NULL,
 * та тих, які не дозволяють.
 * У цій галузі є додаткові оператори:
 * 
 * obj?.prop - null-propagation / null-forgiving - якщо obj не NULL, то 
 *   здійснюється доступ до поля, інакше - результат виразу NULL
 *   можуть використовуватись каскадно: obj?.prop1?.prop2?.method1()?....
 *   
 * obj!.prop - null-checking - якщо obj NULL, то створюється виняток  
 * 
 * obj ?? def  - null-coalescence якщо obj не NULL, то береться його 
 *   значення, інакше - def. Також можливий каскад: obj ?? def1 ?? def2 ?? ...
 *   
 * obj ??= def -  скорочена форма для присвоювання: якщо obj NULL, то 
 *   присвоювання виконується, інакше ігнорується
 */
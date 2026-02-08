using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Extensions
{
    // клас-розширення, який додає функціональність типу int (Int32)
    public static class IntExtension   // клас має бути public static
    {
        // метод-розширення також public static з позначкою типу на який
        // він поширюється з модифікатором this
        public static String px(this Int32 value)
        {
            return value + "px";
        }

        // ТЗ: створити розширення .percnt() яке дописує "%"
        public static String percnt(this Int32 value)
        {
            return $"{value}%";
        }
    }
}

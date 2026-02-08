using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class TableNameAttribute(String value) : Attribute
    {
        public String Value { get; init; } = value;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Data.Dto
{
    internal class Department
    {
        public Guid Id { get; set; }
        public String Name { get; set; } = null!;

        public override string ToString()
        {
            return $"{Id.ToString()[..3]}... {Name}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Data.Dto
{
    internal class Manager
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public String Name { get; set; } = null!;
        public DateTime WorksFrom { get; set; }

        public override string ToString()
        {
            return $"{Id.ToString()[..3]}... {Name}";
        }
    }
}

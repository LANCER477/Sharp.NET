using SharpKnP321.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Data.Models
{
    internal class ProdSaleModel
    {
        public Product Product { get; set; } = null!;
        public int Checks { get; set; }
        public int Quantity { get; set; }
        public double Money { get; set; }
    }
}

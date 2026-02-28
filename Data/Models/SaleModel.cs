namespace SharpKnP321.Data.Models
{
    internal class SaleModel
    {
        public String ManagerName { get; set; } = null!;
        public int Sales { get; set; }

        public override string ToString()
        {
            return $"{ManagerName} --- {Sales}";
        }
    }
}

namespace FlexiCore.Models
{
    public class Sale
    {
        public int Id { get; set; }

        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int BranchId { get; set; }
        public int BusinessId { get; set; }
        public Branch Branch { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public List<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }

    public class SaleItem
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public Sale Sale { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public int? ServiceId { get; set; }
        public Service Service { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

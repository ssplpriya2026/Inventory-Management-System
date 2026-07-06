namespace Inventory_Management_System.ViewModel
{
    public class StockValueReportViewModel
    {
        public List<CategoryStockValueViewModel> Categories { get; set; } 
        public decimal GrandTotal { get; set; }
    }
}

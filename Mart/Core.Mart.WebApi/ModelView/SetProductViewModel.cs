using Core.EF.Models;

namespace Core.Mart.WebApi.ModelView
{
    public class SetProductViewModel
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string? ImagePath { get; set; }
        public double UnitPrice { get; set; }
        public string ShopCode { get; set; }
        public string CategoryName { get; set; }

    }
}

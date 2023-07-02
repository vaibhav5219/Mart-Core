namespace Core.Mart.WebApi.ModelView
{
    public class GetProductsModelView
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; }= string.Empty;
        public string? ImagePath { get; set; } = string.Empty;
        public float UnitPrice { get; set; } 
        public int? CategoryID { get; set; }
        public string ShopCode { get; set; } = string.Empty;

    }
}

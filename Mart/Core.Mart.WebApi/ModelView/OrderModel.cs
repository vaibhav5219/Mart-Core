using System.ComponentModel.DataAnnotations;

namespace Core.Mart.WebApi.ModelView
{
    public class OrderModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImagePath { get; set; } = string.Empty;
        public float UnitPrice { get; set; }
        public int? CategoryID { get; set; }
        public string ShopCode { get; set; } = string.Empty;
        public int Quantity { get; set; }

        //[Required]
        //public int productId { get; set; }
        //public string productName { get; set; }
        //public string description { get; set; }
        //public string? imagePath { get; set; }

        //[Required]
        //public int unitPrice { get; set; }
        //public string categoryId { get; set; }

        //[Required]
        //public string shopCode { get; set; }

        //[Required]
        //public string cartItems { get; set; }

        //[Required]
        //public string category { get; set;}
        //public string orders { get; set; }
        //public string shopCodeNavigation { get; set; }
        //public string id { get; set; }

        //[Required]
        //public int quantity { get; set; }
    }
}

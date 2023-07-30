using Core.EF.Models;

namespace Core.Mart.WebApi.ModelView
{
    public class SetProductViewModel
    {
        //public string Shop_Code { get; set; }

        public int CategoryID { get; set; }

        public Product Product { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string UnitPrice { get; set; }
        public string ShopCode { get; set; }
        public string CategoryId { get; set; }

    }
}

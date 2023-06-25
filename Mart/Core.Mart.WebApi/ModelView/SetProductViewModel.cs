using Core.EF.Models;

namespace Core.Mart.WebApi.ModelView
{
    public class SetProductViewModel
    {
        //public string Shop_Code { get; set; }

        public int CategoryID { get; set; }

        public Product Product { get; set; }
    }
}

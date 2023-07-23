namespace Core.Mart.WebApi.ModelView
{
    public class ShopDetailsModel
    {
        public string ShopCode { get; set; } = null!;

        public string ShopName { get; set; } = null!;

        public string ShopKeeperName { get; set; } = null!;

        public string Mobile { get; set; } = null!;

        public int Address { get; set; }

        public string ShopDomainName { get; set; } = null!;

        public string PinCode { get; set; } = null!;

        public string? AspNetUsersId { get; set; } = null!;
    }
}

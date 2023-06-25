using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.WebApi.Mart.Auth
{
    public class CartDbContext : IdentityDbContext<IdentityUser>
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

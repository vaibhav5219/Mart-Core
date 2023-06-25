using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Core.EF.Models;

public partial class CartDbcoreContext : DbContext
{
    public CartDbcoreContext()
    {
    }

    public CartDbcoreContext(DbContextOptions<CartDbcoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderStatusTbl> OrderStatusTbls { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ShopDetail> ShopDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-9C0AVE1\\SQLEXPRESS;database=cartDBCore;uid=sa;password=Vaibhav123@;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("Address");

            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.CustomerUserName).HasMaxLength(50);
            entity.Property(e => e.PostalCode)
                .HasMaxLength(50)
                .HasColumnName("Postal_Code");
            entity.Property(e => e.Remarks).HasMaxLength(100);
            entity.Property(e => e.ShopCode)
                .HasMaxLength(50)
                .HasColumnName("Shop_Code");
            entity.Property(e => e.StreetNumber).HasMaxLength(50);
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK_dbo.CartItems");

            entity.Property(e => e.CartId)
                .HasMaxLength(128)
                .HasColumnName("Cart_Id");
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.ItemId).HasColumnName("Item_Id");
            entity.Property(e => e.ShopCode)
                .HasMaxLength(50)
                .HasColumnName("Shop_Code");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItems_Products");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_dbo.Categories");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.ShopCode)
                .HasMaxLength(50)
                .HasColumnName("Shop_Code");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.HasIndex(e => e.CustomerId, "Unique_Email").IsUnique();

            entity.HasIndex(e => e.CustomerId, "Unique_Mobile").IsUnique();

            entity.HasIndex(e => e.CustomerId, "Unique_UserName").IsUnique();

            entity.Property(e => e.AspNetUserId).HasMaxLength(50);
            entity.Property(e => e.CustomerName).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Mobile)
                .HasMaxLength(15)
                .IsFixedLength();
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.OrderId).HasColumnName("Order_Id");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .HasColumnName("Customer_Id");
            entity.Property(e => e.DeliveredDate)
                .HasColumnType("date")
                .HasColumnName("Delivered_Date");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("Order_Date");
            entity.Property(e => e.OrderQuantity).HasColumnName("Order_Quantity");
            entity.Property(e => e.OrderStatus).HasColumnName("Order_Status");
            entity.Property(e => e.OrderTotal).HasColumnName("Order_Total");
            entity.Property(e => e.ProductId).HasColumnName("Product_Id");
            entity.Property(e => e.ShopCode)
                .HasMaxLength(50)
                .HasColumnName("Shop_Code");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Product_ID");

            entity.HasOne(d => d.ShopCodeNavigation).WithMany(p => p.Orders)
                .HasPrincipalKey(p => p.ShopCode)
                .HasForeignKey(d => d.ShopCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_ShopDetails");
        });

        modelBuilder.Entity<OrderStatusTbl>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId);

            entity.ToTable("OrderStatus_tbl");

            entity.Property(e => e.OrderStatusId).HasColumnName("OrderStatus_Id");
            entity.Property(e => e.OrderStatus).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK_dbo.Products");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.ShopCode)
                .HasMaxLength(50)
                .HasColumnName("Shop_Code");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Products_Categories");

            entity.HasOne(d => d.ShopCodeNavigation).WithMany(p => p.Products)
                .HasPrincipalKey(p => p.ShopCode)
                .HasForeignKey(d => d.ShopCode)
                .HasConstraintName("FK_Products_ShopDetails");
        });

        modelBuilder.Entity<ShopDetail>(entity =>
        {
            entity.HasKey(e => e.ShopId);

            entity.HasIndex(e => e.ShopCode, "Unique_Shop_Code").IsUnique();

            entity.Property(e => e.ShopId)
                .HasMaxLength(50)
                .HasColumnName("Shop_Id");
            entity.Property(e => e.AspNetUsersId).HasMaxLength(50);
            entity.Property(e => e.Mobile)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.PinCode)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Pin_Code");
            entity.Property(e => e.ShopCode)
                .HasMaxLength(50)
                .HasColumnName("Shop_Code");
            entity.Property(e => e.ShopDomainName)
                .HasMaxLength(50)
                .HasColumnName("Shop_Domain_Name");
            entity.Property(e => e.ShopKeeperName).HasMaxLength(50);
            entity.Property(e => e.ShopName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ClotheStore.Models;

public partial class ClothesStoreContext : DbContext
{
    public ClothesStoreContext()
    {
    }

    public ClothesStoreContext(DbContextOptions<ClothesStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Page> Pages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductSize> ProductSizes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Ship> Ships { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<TransactStatus> TransactStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-2S8FR9T\\SQLSERVER1;Initial Catalog=ClothesStore;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Salt)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Accounts_Roles");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CatId).HasName("PK_Types");

            entity.Property(e => e.CatId).HasColumnName("CatID");
            entity.Property(e => e.CatName).HasMaxLength(250);
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsEmailConfirmed).HasDefaultValue(false);
            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.Salt)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.Location).WithMany(p => p.Customers)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK_Customers_Locations");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.NameWithType).HasMaxLength(255);
            entity.Property(e => e.PathWithType).HasMaxLength(255);
            entity.Property(e => e.Slug).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(20);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.ShipDate).HasColumnType("datetime");
            entity.Property(e => e.TransactStatusId).HasColumnName("TransactStatusID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Customers");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Payment");

            entity.HasOne(d => d.TransactStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TransactStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_TransactStatus");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ShipDate).HasColumnType("datetime");
            entity.Property(e => e.Size).HasMaxLength(10);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderDetails_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Products");
        });

        modelBuilder.Entity<Page>(entity =>
        {
            entity.Property(e => e.PageId).HasColumnName("PageID");
            entity.Property(e => e.Alias).HasMaxLength(250);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.MetaDesc).HasMaxLength(250);
            entity.Property(e => e.MetaKey).HasMaxLength(250);
            entity.Property(e => e.PageName).HasMaxLength(250);
            entity.Property(e => e.Thumb).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(250);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Payment1)
                .HasMaxLength(50)
                .HasColumnName("Payment");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Alias).HasMaxLength(250);
            entity.Property(e => e.CatId).HasColumnName("CatID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.ProductName).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(250);

            entity.HasOne(d => d.Cat).WithMany(p => p.Products)
                .HasForeignKey(d => d.CatId)
                .HasConstraintName("FK_Products_Types");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ImageId);

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductImages_Products");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.ToTable("ProductSize");

            entity.Property(e => e.ProductSizeId)
                .ValueGeneratedNever()
                .HasColumnName("ProductSizeID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SizeId).HasColumnName("SizeID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductSizes)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductSize_Products");

            entity.HasOne(d => d.Size).WithMany(p => p.ProductSizes)
                .HasForeignKey(d => d.SizeId)
                .HasConstraintName("FK_ProductSize_Size");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Ship>(entity =>
        {
            entity.HasKey(e => e.ShipperId);

            entity.Property(e => e.ShipperId).HasColumnName("ShipperID");
            entity.Property(e => e.Company).HasMaxLength(150);
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsFixedLength();
            entity.Property(e => e.ShipDate).HasColumnType("datetime");
            entity.Property(e => e.ShipperName).HasMaxLength(150);
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.ToTable("Size");

            entity.Property(e => e.SizeId).HasColumnName("SizeID");
            entity.Property(e => e.SizeName)
                .HasMaxLength(5)
                .IsFixedLength();
        });

        modelBuilder.Entity<TransactStatus>(entity =>
        {
            entity.ToTable("TransactStatus");

            entity.Property(e => e.TransactStatusId).HasColumnName("TransactStatusID");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

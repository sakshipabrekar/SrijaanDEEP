using Microsoft.EntityFrameworkCore;
using srijaanDEEP.Models;
using SrijanDEEP.API.Entities;

namespace SrijanDEEP.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<SupplyOrder> SupplyOrders => Set<SupplyOrder>();
    public DbSet<SyncHistory> SyncHistory => Set<SyncHistory>();
    public DbSet<UploadHistory> UploadHistory => Set<UploadHistory>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<DefencePlatformMaster> DefencePlatformMasters => Set<DefencePlatformMaster>();
    public DbSet<ProductTypeMaster> ProductTypeMasters => Set<ProductTypeMaster>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---------------- Users ----------------
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.UserId);
            e.Property(x => x.Username).IsRequired().HasMaxLength(100);
            e.Property(x => x.Email).IsRequired().HasMaxLength(256);
            e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(512);
            e.Property(x => x.Role).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Username).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
        });

        // ---------------- RefreshTokens ----------------
        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.ToTable("RefreshTokens");
            e.HasKey(x => x.RefreshTokenId);
            e.Property(x => x.Token).IsRequired().HasMaxLength(512);
            e.HasIndex(x => x.Token);
            e.HasOne(x => x.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Ignore(x => x.IsExpired);
            e.Ignore(x => x.IsActive);
        });

        // ---------------- Vendors ----------------
        modelBuilder.Entity<Vendor>(e =>
        {
            e.ToTable("Vendor_Master");
            e.HasKey(x => x.URN_No);
            e.Property(x => x.URN_No).IsRequired().HasMaxLength(100);
            e.Property(x => x.Vendor_Org_Name).IsRequired().HasMaxLength(250);
            e.Property(x => x.CIN_No).HasMaxLength(50);
            e.Property(x => x.MSME_No).HasMaxLength(50);
            e.Property(x => x.GST_No).HasMaxLength(50);
            e.Property(x => x.PAN_No).HasMaxLength(20);
            e.Property(x => x.Vendor_Org_Email).HasMaxLength(256);
            e.Property(x => x.Nodal_Officer_Name).HasMaxLength(150);
            e.Property(x => x.Nodal_Officer_Email).HasMaxLength(256);
            e.Property(x => x.Nodal_Officer_Mobile).HasMaxLength(20);
            e.Property(x => x.Nodal_Officer_Designation).HasMaxLength(150);
            e.Property(x => x.Vendor_Code_assigned_by_MDL).HasMaxLength(50);
            e.Property(x => x.Uploaded_by).HasMaxLength(100);
            e.Property(x => x.Upload_IP).HasMaxLength(50);
            e.Property(x => x.Last_Modified_by).HasMaxLength(100);
            e.Property(x => x.Modify_IP).HasMaxLength(50);

            e.HasIndex(x => x.Vendor_Org_Name);
            e.HasIndex(x => x.GST_No);
            e.HasIndex(x => x.PAN_No);
            e.HasIndex(x => x.MSME_No);
        });

        // ---------------- Products ----------------
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.HasKey(x => x.Id);
            e.Property(x => x.URN_No).IsRequired().HasMaxLength(100);
            e.Property(x => x.Product_Name).IsRequired().HasMaxLength(250);
            e.Property(x => x.Part_No).HasMaxLength(100);
            e.Property(x => x.Defence_Platform).HasMaxLength(200);
            e.Property(x => x.Product_Type).HasMaxLength(200);
            e.Property(x => x.SO_No).HasMaxLength(100);
            e.Property(x => x.Uploaded_by).HasMaxLength(100);
            e.Property(x => x.Upload_IP).HasMaxLength(50);
            e.Property(x => x.Last_Modified_by).HasMaxLength(100);
            e.Property(x => x.Modify_IP).HasMaxLength(50);

            e.HasIndex(x => x.URN_No);
            e.HasIndex(x => x.Product_Name);
            e.HasIndex(x => x.Defence_Platform);

            e.HasOne(x => x.Vendor)
                .WithMany(v => v.Products)
                .HasForeignKey(x => x.URN_No)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------------- SupplyOrders ----------------
        modelBuilder.Entity<SupplyOrder>(e =>
        {
            e.ToTable("Supply_Orders");
            e.HasKey(x => x.Id);
            e.Property(x => x.PO_No).IsRequired().HasMaxLength(100);
            e.Property(x => x.PRO_No).HasMaxLength(100);
            e.Property(x => x.URN_No).IsRequired().HasMaxLength(100);
            e.Property(x => x.Qty_Ordered).HasColumnType("decimal(18,2)");
            e.Property(x => x.Unit_Rate).HasColumnType("decimal(18,2)");
            e.Property(x => x.Total_line_value).HasColumnType("decimal(18,2)");
            e.Property(x => x.Qty_Supplied).HasColumnType("decimal(18,2)");
            e.Property(x => x.Uploaded_by).HasMaxLength(100);
            e.Property(x => x.Upload_IP).HasMaxLength(50);
            e.Property(x => x.Last_Modified_by).HasMaxLength(100);
            e.Property(x => x.Modify_IP).HasMaxLength(50);

            e.HasIndex(x => x.ProductId);
            e.HasIndex(x => x.PO_No);
            e.HasIndex(x => x.URN_No);

            e.HasOne(x => x.Product)
                .WithMany(p => p.SupplyOrders)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Vendor)
                .WithMany(v => v.SupplyOrders)
                .HasForeignKey(x => x.URN_No)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------------- SyncHistory ----------------
        modelBuilder.Entity<SyncHistory>(e =>
        {
            e.ToTable("SyncHistory");
            e.HasKey(x => x.SyncHistoryId);
            e.Property(x => x.EntityType).IsRequired().HasMaxLength(50);
            e.Property(x => x.Status).IsRequired().HasMaxLength(50);
            e.HasIndex(x => new { x.EntityType, x.Status });
        });

        // ---------------- UploadHistory ----------------
        modelBuilder.Entity<UploadHistory>(e =>
        {
            e.ToTable("UploadHistory");
            e.HasKey(x => x.UploadHistoryId);
            e.Property(x => x.UploadType).IsRequired().HasMaxLength(20);
            e.Property(x => x.EntityType).IsRequired().HasMaxLength(50);
            e.Property(x => x.FileName).IsRequired().HasMaxLength(300);
            e.Property(x => x.Status).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.EntityType);
            e.HasOne(x => x.UploadedByUser)
                .WithMany()
                .HasForeignKey(x => x.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------------- DefencePlatformMaster ----------------
        modelBuilder.Entity<DefencePlatformMaster>(e =>
        {
            e.ToTable("DefencePlatformMaster");
            e.HasKey(x => x.Id);
        });

        // ---------------- ProductTypeMaster ----------------
        modelBuilder.Entity<ProductTypeMaster>(e =>
        {
            e.ToTable("ProductTypeMaster");
            e.HasKey(x => x.Id);
        });
    }

    /// <summary>
    /// Auto-stamps date/time audit columns on every save. "By"/"IP" fields are
    /// request-context dependent and must be set by the calling service before
    /// SaveChanges is invoked.
    /// </summary>
    public override int SaveChanges()
    {
        ApplyAuditTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditTimestamps()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            switch (entry.Entity)
            {
                case User user:
                    if (entry.State == EntityState.Added) user.CreatedDate = utcNow;
                    if (entry.State is EntityState.Added or EntityState.Modified) user.ModifiedDate = utcNow;
                    break;

                case Vendor vendor:
                    if (entry.State == EntityState.Added) vendor.Uploaded_DateTime = utcNow;
                    if (entry.State is EntityState.Added or EntityState.Modified) vendor.Last_Modified_DateTime = utcNow;
                    break;

                case Product product:
                    if (entry.State == EntityState.Added) product.Uploaded_DateTime = utcNow;
                    if (entry.State is EntityState.Added or EntityState.Modified) product.Last_Modified_DateTime = utcNow;
                    break;

                case SupplyOrder supplyOrder:
                    if (entry.State == EntityState.Added) supplyOrder.Uploaded_DateTime = utcNow;
                    if (entry.State is EntityState.Added or EntityState.Modified) supplyOrder.Last_Modified_DateTime = utcNow;
                    supplyOrder.Total_line_value = supplyOrder.Qty_Ordered * supplyOrder.Unit_Rate;
                    break;

                case SyncHistory syncHistory:
                    if (entry.State == EntityState.Added) syncHistory.CreatedDate = utcNow;
                    if (entry.State is EntityState.Added or EntityState.Modified) syncHistory.LastModifiedDate = utcNow;
                    break;

                case UploadHistory uploadHistory:
                    if (entry.State == EntityState.Added) uploadHistory.CreatedDate = utcNow;
                    if (entry.State is EntityState.Added or EntityState.Modified) uploadHistory.LastModifiedDate = utcNow;
                    break;

                case RefreshToken refreshToken:
                    if (entry.State == EntityState.Added) refreshToken.CreatedDate = utcNow;
                    if (entry.State is EntityState.Added or EntityState.Modified) refreshToken.LastModifiedDate = utcNow;
                    break;
            }
        }
    }
}
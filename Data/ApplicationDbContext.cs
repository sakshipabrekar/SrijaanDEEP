using Microsoft.EntityFrameworkCore;
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
            e.ToTable("Vendors");
            e.HasKey(x => x.VendorId);
            e.Property(x => x.VendorOrganisationName).IsRequired().HasMaxLength(250);
            e.Property(x => x.URNNumber).HasMaxLength(50);
            e.Property(x => x.CINNumber).HasMaxLength(50);
            e.Property(x => x.MSMENumber).HasMaxLength(50);
            e.Property(x => x.GSTNumber).HasMaxLength(50);
            e.Property(x => x.PANNumber).HasMaxLength(20);
            e.Property(x => x.VendorOrganisationEmail).HasMaxLength(256);
            e.Property(x => x.NodalOfficerName).HasMaxLength(150);
            e.Property(x => x.NodalOfficerEmail).HasMaxLength(256);
            e.Property(x => x.NodalOfficerMobile).HasMaxLength(20);
            e.Property(x => x.NodalOfficerDesignation).HasMaxLength(150);
            e.Property(x => x.VendorCodeAssignedByDPSU).HasMaxLength(50);
            e.HasIndex(x => x.URNNumber).IsUnique().HasFilter("[URNNumber] IS NOT NULL");
            e.HasIndex(x => x.VendorOrganisationName);
            e.HasIndex(x => x.GSTNumber);
            e.HasIndex(x => x.PANNumber);
        });

        // ---------------- Products ----------------
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.HasKey(x => x.ProductId);
            e.Property(x => x.UniqueReferenceNumber).IsRequired().HasMaxLength(100);
            e.Property(x => x.ProductName).IsRequired().HasMaxLength(250);
            e.Property(x => x.CompanyName).HasMaxLength(250);
            e.Property(x => x.DefencePlatform).HasMaxLength(150);
            e.Property(x => x.ProductType).HasMaxLength(100);
            e.Property(x => x.PartNumber).HasMaxLength(100);
            e.Property(x => x.SupplyOrderNumber).HasMaxLength(100);
            e.HasIndex(x => x.UniqueReferenceNumber).IsUnique();
            e.HasIndex(x => x.VendorId);
            e.HasIndex(x => x.ProductName);
            e.HasIndex(x => x.DefencePlatform);
            e.HasOne(x => x.Vendor)
                .WithMany(v => v.Products)
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------------- SupplyOrders ----------------
        modelBuilder.Entity<SupplyOrder>(e =>
        {
            e.ToTable("SupplyOrders");
            e.HasKey(x => x.SupplyOrderId);
            e.Property(x => x.PurchaseOrderNumber).IsRequired().HasMaxLength(100);
            e.Property(x => x.VendorName).HasMaxLength(250);
            e.Property(x => x.URNNumber).HasMaxLength(50);
            e.Property(x => x.QuantityOrdered).HasColumnType("decimal(18,2)");
            e.Property(x => x.UnitRate).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalLineValue).HasColumnType("decimal(18,2)");
            e.Property(x => x.QuantitySupplied).HasColumnType("decimal(18,2)");
            e.HasIndex(x => x.ProductId);
            e.HasIndex(x => x.PurchaseOrderNumber);
            e.HasOne(x => x.Product)
                .WithMany(p => p.SupplyOrders)
                .HasForeignKey(x => x.ProductId)
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
    }

    /// <summary>
    /// Auto-stamps CreatedDate/LastModifiedDate (or ModifiedDate for User) on every save,
    /// so individual services never have to remember to do it.
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
                    if (entry.State == EntityState.Added) vendor.CreatedDate = utcNow;
                    if (entry.State is EntityState.Added or EntityState.Modified) vendor.LastModifiedDate = utcNow;
                    break;
                case Product product:
                    if (entry.State == EntityState.Added) product.CreatedDate = utcNow;
                    if (entry.State is EntityState.Added or EntityState.Modified) product.LastModifiedDate = utcNow;
                    break;
                case SupplyOrder supplyOrder:
                    if (entry.State == EntityState.Added) supplyOrder.CreatedDate = utcNow;
                    if (entry.State is EntityState.Added or EntityState.Modified) supplyOrder.LastModifiedDate = utcNow;
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
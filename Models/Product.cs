namespace SrijanDEEP.API.Entities;

public class Product
{
    public int ProductId { get; set; }
    public string UniqueReferenceNumber { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? DefencePlatform { get; set; }
    public string? ProductType { get; set; }
    public string? PartNumber { get; set; }
    public bool IsItemSupplied { get; set; }
    public string? SupplyOrderNumber { get; set; }
    public DateTime? SupplyOrderDate { get; set; }

    public int VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public ICollection<SupplyOrder> SupplyOrders { get; set; } = new List<SupplyOrder>();
}
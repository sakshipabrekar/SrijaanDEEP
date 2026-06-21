namespace SrijanDEEP.API.Entities;

public class SupplyOrder
{
    public int SupplyOrderId { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public DateTime? PurchaseOrderDate { get; set; }
    public string? VendorName { get; set; }
    public string? URNNumber { get; set; }
    public bool IsVendorMSME { get; set; }
    public decimal QuantityOrdered { get; set; }
    public decimal UnitRate { get; set; }
    public decimal TotalLineValue { get; set; }
    public decimal QuantitySupplied { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
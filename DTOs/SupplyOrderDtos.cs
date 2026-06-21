using SrijanDEEP.API.Common;

namespace SrijanDEEP.API.DTOs;

public class SupplyOrderResponseDto
{
    public int SupplyOrderId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public DateTime? PurchaseOrderDate { get; set; }
    public string? VendorName { get; set; }
    public string? URNNumber { get; set; }
    public bool IsVendorMSME { get; set; }
    public decimal QuantityOrdered { get; set; }
    public decimal UnitRate { get; set; }
    public decimal TotalLineValue { get; set; }
    public decimal QuantitySupplied { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateSupplyOrderDto
{
    public int ProductId { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public DateTime? PurchaseOrderDate { get; set; }
    public string? VendorName { get; set; }
    public string? URNNumber { get; set; }
    public bool IsVendorMSME { get; set; }
    public decimal QuantityOrdered { get; set; }
    public decimal UnitRate { get; set; }
    public decimal QuantitySupplied { get; set; }
}

public class UpdateSupplyOrderDto
{
    public int ProductId { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public DateTime? PurchaseOrderDate { get; set; }
    public string? VendorName { get; set; }
    public string? URNNumber { get; set; }
    public bool IsVendorMSME { get; set; }
    public decimal QuantityOrdered { get; set; }
    public decimal UnitRate { get; set; }
    public decimal QuantitySupplied { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SupplyOrderFilterParams : PaginationParams
{
    public int? ProductId { get; set; }
    public bool? IsVendorMSME { get; set; }
    public DateTime? PurchaseOrderDateFrom { get; set; }
    public DateTime? PurchaseOrderDateTo { get; set; }
    public bool? IsActive { get; set; }
}
using SrijanDEEP.API.Common;

namespace SrijanDEEP.API.DTOs;

public class ProductResponseDto
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
    public string? VendorOrganisationName { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateProductDto
{
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
}

public class UpdateProductDto
{
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
    public bool IsActive { get; set; } = true;
}

public class ProductFilterParams : PaginationParams
{
    public int? VendorId { get; set; }
    public string? DefencePlatform { get; set; }
    public string? ProductType { get; set; }
    public bool? IsItemSupplied { get; set; }
    public bool? IsActive { get; set; }
}
using SrijanDEEP.API.Common;

namespace SrijanDEEP.API.DTOs;

// ─── Response ─────────────────────────────────────────────────────────────────

public class ProductResponseDto
{
    public int Id { get; set; }
    public string URN_No { get; set; } = string.Empty;       // FK to Vendor_Master
    public string? Part_No { get; set; }
    public string? Defence_Platform { get; set; }
    public string? Product_Type { get; set; }
    public string Product_Name { get; set; } = string.Empty;
    public bool Is_Item_Supplied { get; set; }
    public string? SO_No { get; set; }
    public DateTime? SO_Date { get; set; }
    public bool IsActive { get; set; }

    // Vendor snapshot (from navigation)
    public string? Vendor_Org_Name { get; set; }

    // Audit — read-only in responses
    public bool Manual_Bulk_upload { get; set; }
    public string? Uploaded_by { get; set; }
    public DateTime? Uploaded_DateTime { get; set; }
    public string? Upload_IP { get; set; }
    public string? Last_Modified_by { get; set; }
    public DateTime? Last_Modified_DateTime { get; set; }
    public string? Modify_IP { get; set; }
}

// ─── Create ───────────────────────────────────────────────────────────────────

public class CreateProductDto
{
    /// <summary>FK — must match an existing Vendor_Master.URN_No.</summary>
    public string URN_No { get; set; } = string.Empty;

    public string? Part_No { get; set; }
    public string? Defence_Platform { get; set; }
    public string? Product_Type { get; set; }
    public string Product_Name { get; set; } = string.Empty;
    public bool Is_Item_Supplied { get; set; }
    public string? SO_No { get; set; }
    public DateTime? SO_Date { get; set; }
    public bool Manual_Bulk_upload { get; set; } = false;
}

// ─── Update ───────────────────────────────────────────────────────────────────

public class UpdateProductDto
{
    /// <summary>FK — must match an existing Vendor_Master.URN_No.</summary>
    public string URN_No { get; set; } = string.Empty;

    public string? Part_No { get; set; }
    public string? Defence_Platform { get; set; }
    public string? Product_Type { get; set; }
    public string Product_Name { get; set; } = string.Empty;
    public bool Is_Item_Supplied { get; set; }
    public string? SO_No { get; set; }
    public DateTime? SO_Date { get; set; }
    public bool Manual_Bulk_upload { get; set; }

    /// <summary>Set false to soft-deactivate without deleting.</summary>
    public bool IsActive { get; set; } = true;
}

// ─── Filter ───────────────────────────────────────────────────────────────────

public class ProductFilterParams : PaginationParams
{
    public string? URN_No { get; set; }
    public string? Defence_Platform { get; set; }
    public string? Product_Type { get; set; }
    public bool? Is_Item_Supplied { get; set; }
    public bool? IsActive { get; set; }
}
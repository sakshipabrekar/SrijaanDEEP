using SrijanDEEP.API.Common;

namespace SrijanDEEP.API.DTOs;

// ─── Response ─────────────────────────────────────────────────────────────────

public class SupplyOrderResponseDto
{
    public int Id { get; set; }
    public string? PRO_No { get; set; }
    public string PO_No { get; set; } = string.Empty;
    public DateTime? PO_Date { get; set; }
    public string URN_No { get; set; } = string.Empty;    // FK to Vendor_Master
    public bool Whether_MSME { get; set; }
    public decimal Qty_Ordered { get; set; }
    public decimal Unit_Rate { get; set; }
    public decimal Total_line_value { get; set; }          // server-computed
    public decimal Qty_Supplied { get; set; }
    public bool IsActive { get; set; }

    // Linked product snapshot
    public int ProductId { get; set; }
    public string? Product_Name { get; set; }

    // Vendor snapshot
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

public class CreateSupplyOrderDto
{
    public int ProductId { get; set; }

    /// <summary>FK — must match an existing Vendor_Master.URN_No.</summary>
    public string URN_No { get; set; } = string.Empty;

    public string? PRO_No { get; set; }
    public string PO_No { get; set; } = string.Empty;
    public DateTime? PO_Date { get; set; }
    public bool Whether_MSME { get; set; }
    public decimal Qty_Ordered { get; set; }
    public decimal Unit_Rate { get; set; }
    public decimal Qty_Supplied { get; set; }
    public bool Manual_Bulk_upload { get; set; } = false;
}

// ─── Update ───────────────────────────────────────────────────────────────────

public class UpdateSupplyOrderDto
{
    public int ProductId { get; set; }

    /// <summary>FK — must match an existing Vendor_Master.URN_No.</summary>
    public string URN_No { get; set; } = string.Empty;

    public string? PRO_No { get; set; }
    public string PO_No { get; set; } = string.Empty;
    public DateTime? PO_Date { get; set; }
    public bool Whether_MSME { get; set; }
    public decimal Qty_Ordered { get; set; }
    public decimal Unit_Rate { get; set; }
    public decimal Qty_Supplied { get; set; }
    public bool Manual_Bulk_upload { get; set; }

    /// <summary>Set false to soft-deactivate without deleting.</summary>
    public bool IsActive { get; set; } = true;
}

// ─── Filter ───────────────────────────────────────────────────────────────────

public class SupplyOrderFilterParams : PaginationParams
{
    public int? ProductId { get; set; }
    public string? URN_No { get; set; }
    public bool? Whether_MSME { get; set; }
    public DateTime? PO_DateFrom { get; set; }
    public DateTime? PO_DateTo { get; set; }
    public bool? IsActive { get; set; }
}
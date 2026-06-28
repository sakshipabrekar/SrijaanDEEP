using SrijanDEEP.API.Common;

namespace SrijanDEEP.API.DTOs;

// ─── Response ─────────────────────────────────────────────────────────────────

public class VendorResponseDto
{
    public string URN_No { get; set; } = string.Empty;
    public string? CIN_No { get; set; }
    public string? MSME_No { get; set; }
    public string? GST_No { get; set; }
    public string? PAN_No { get; set; }
    public string Vendor_Org_Name { get; set; } = string.Empty;
    public string? Vendor_Org_Email { get; set; }
    public string? Nodal_Officer_Name { get; set; }
    public string? Nodal_Officer_Email { get; set; }
    public string? Nodal_Officer_Mobile { get; set; }
    public string? Nodal_Officer_Designation { get; set; }
    public string? Vendor_Code_assigned_by_MDL { get; set; }
    public bool IsActive { get; set; }

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

public class CreateVendorDto
{
    /// <summary>URN_No is the PK — must be provided on creation.</summary>
    public string URN_No { get; set; } = string.Empty;

    public string? CIN_No { get; set; }
    public string? MSME_No { get; set; }
    public string? GST_No { get; set; }
    public string? PAN_No { get; set; }
    public string Vendor_Org_Name { get; set; } = string.Empty;
    public string? Vendor_Org_Email { get; set; }
    public string? Nodal_Officer_Name { get; set; }
    public string? Nodal_Officer_Email { get; set; }
    public string? Nodal_Officer_Mobile { get; set; }
    public string? Nodal_Officer_Designation { get; set; }
    public string? Vendor_Code_assigned_by_MDL { get; set; }

    /// <summary>True when record comes from a bulk-upload file rather than the UI form.</summary>
    public bool Manual_Bulk_upload { get; set; } = false;
}

// ─── Update ───────────────────────────────────────────────────────────────────

public class UpdateVendorDto
{
    public string? CIN_No { get; set; }
    public string? MSME_No { get; set; }
    public string? GST_No { get; set; }
    public string? PAN_No { get; set; }
    public string Vendor_Org_Name { get; set; } = string.Empty;
    public string? Vendor_Org_Email { get; set; }
    public string? Nodal_Officer_Name { get; set; }
    public string? Nodal_Officer_Email { get; set; }
    public string? Nodal_Officer_Mobile { get; set; }
    public string? Nodal_Officer_Designation { get; set; }
    public string? Vendor_Code_assigned_by_MDL { get; set; }
    public bool Manual_Bulk_upload { get; set; }

    /// <summary>Set to false to soft-deactivate; there is no hard-delete endpoint.</summary>
    public bool IsActive { get; set; } = true;
}

// ─── Filter / search params ───────────────────────────────────────────────────

public class VendorFilterParams : PaginationParams
{
    public string? GST_No { get; set; }
    public string? PAN_No { get; set; }
    public string? MSME_No { get; set; }
    public string? URN_No { get; set; }
    public bool? IsActive { get; set; }
}
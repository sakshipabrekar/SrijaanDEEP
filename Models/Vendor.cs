using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SrijanDEEP.API.Entities;

[Table("Vendor_Master")]
public class Vendor
{
    [Key]
    [Column("URN_No")]
    [StringLength(100)]
    public string URN_No { get; set; } = string.Empty;

    [Column("CIN_No")]
    [StringLength(21)]
    public string? CIN_No { get; set; }

    [Column("MSME_No")]
    [StringLength(50)]
    public string? MSME_No { get; set; }

    [Column("GST_No")]
    [StringLength(15)]
    public string? GST_No { get; set; }

    [Column("PAN_No")]
    [StringLength(10)]
    public string? PAN_No { get; set; }

    [Required]
    [Column("Vendor_Org_Name")]
    [StringLength(250)]
    public string Vendor_Org_Name { get; set; } = string.Empty;

    [Column("Vendor_Org_Email")]
    [StringLength(150)]
    public string? Vendor_Org_Email { get; set; }

    [Column("Nodal_Officer_Name")]
    [StringLength(150)]
    public string? Nodal_Officer_Name { get; set; }

    [Column("Nodal_Officer_Email")]
    [StringLength(150)]
    public string? Nodal_Officer_Email { get; set; }

    [Column("Nodal_Officer_Mobile")]
    [StringLength(15)]
    public string? Nodal_Officer_Mobile { get; set; }

    [Column("Nodal_Officer_Designation")]
    [StringLength(150)]
    public string? Nodal_Officer_Designation { get; set; }

    [Column("Vendor_Code_assigned_by_MDL")]
    [StringLength(100)]
    public string? Vendor_Code_assigned_by_MDL { get; set; }

    // ─── Audit fields (matching image column names exactly) ───────────────────

    [Column("Manual_Bulk_upload")]
    public bool Manual_Bulk_upload { get; set; } = false;

    [Column("Uploaded_by")]
    [StringLength(100)]
    public string? Uploaded_by { get; set; }

    [Column("Uploaded_DateTime")]
    public DateTime? Uploaded_DateTime { get; set; }

    [Column("Upload_IP")]
    [StringLength(50)]
    public string? Upload_IP { get; set; }

    [Column("Last_Modified_by")]
    [StringLength(100)]
    public string? Last_Modified_by { get; set; }

    [Column("Last_Modified_DateTime")]
    public DateTime? Last_Modified_DateTime { get; set; }

    [Column("Modify_IP")]
    [StringLength(50)]
    public string? Modify_IP { get; set; }

    // ─── Navigation ───────────────────────────────────────────────────────────

    public bool IsActive { get; set; } = true;
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<SupplyOrder> SupplyOrders { get; set; } = new List<SupplyOrder>();

}
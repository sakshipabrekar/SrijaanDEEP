using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SrijanDEEP.API.Entities;

[Table("Supply_Orders")]
public class SupplyOrder
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("PRO_No")]
    [StringLength(100)]
    public string? PRO_No { get; set; }

    [Column("PO_No")]
    [Required]
    [StringLength(100)]
    public string PO_No { get; set; } = string.Empty;

    [Column("PO_Date")]
    public DateTime? PO_Date { get; set; }

    // FK → Vendor_Master.URN_No
    [Required]
    [Column("URN_No")]
    [StringLength(100)]
    public string URN_No { get; set; } = string.Empty;

    [Column("Whether_MSME")]
    public bool Whether_MSME { get; set; } = false;

    [Column("Qty_Ordered")]
    public decimal Qty_Ordered { get; set; }

    [Column("Unit_Rate")]
    public decimal Unit_Rate { get; set; }

    /// <summary>Server-computed: Qty_Ordered × Unit_Rate. Never set from client.</summary>
    [Column("Total_line_value")]
    public decimal Total_line_value { get; set; }

    [Column("Qty_Supplied")]
    public decimal Qty_Supplied { get; set; }

    // ─── Audit fields ─────────────────────────────────────────────────────────

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

    // ─── Soft-delete ──────────────────────────────────────────────────────────

    public bool IsActive { get; set; } = true;

    // ─── Navigation ───────────────────────────────────────────────────────────

    /// <summary>FK to Products.Id — which product this supply order line is for.</summary>
    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }

    [ForeignKey(nameof(URN_No))]
    public Vendor? Vendor { get; set; }
}
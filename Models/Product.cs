using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SrijanDEEP.API.Entities;

[Table("Products")]
public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // FK → Vendor_Master.URN_No
    [Required]
    [Column("URN_No")]
    [StringLength(100)]
    public string URN_No { get; set; } = string.Empty;

    [Column("Part_No")]
    [StringLength(100)]
    public string? Part_No { get; set; }

    [Column("Defence_Platform")]
    [StringLength(200)]
    public string? Defence_Platform { get; set; }

    [Column("Product_Type")]
    [StringLength(200)]
    public string? Product_Type { get; set; }

    [Required]
    [Column("Product_Name")]
    [StringLength(250)]
    public string Product_Name { get; set; } = string.Empty;

    [Column("Is_Item_Supplied")]
    public bool Is_Item_Supplied { get; set; } = false;

    [Column("SO_No")]
    [StringLength(100)]
    public string? SO_No { get; set; }

    [Column("SO_Date")]
    public DateTime? SO_Date { get; set; }

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

    [ForeignKey(nameof(URN_No))]
    public Vendor? Vendor { get; set; }

    public ICollection<SupplyOrder> SupplyOrders { get; set; } = new List<SupplyOrder>();
    public int ProductId { get; internal set; }
  
}
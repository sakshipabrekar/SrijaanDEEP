namespace SrijanDEEP.API.Entities;

public class Vendor
{
    public int VendorId { get; set; }
    public string? URNNumber { get; set; }
    public string? CINNumber { get; set; }
    public string? MSMENumber { get; set; }
    public string? GSTNumber { get; set; }
    public string? PANNumber { get; set; }
    public string VendorOrganisationName { get; set; } = string.Empty;
    public string? VendorOrganisationEmail { get; set; }
    public string? NodalOfficerName { get; set; }
    public string? NodalOfficerEmail { get; set; }
    public string? NodalOfficerMobile { get; set; }
    public string? NodalOfficerDesignation { get; set; }
    public string? VendorCodeAssignedByDPSU { get; set; }

    /// <summary>Drives soft-deactivation; there is no hard delete endpoint.</summary>
    public bool IsActive { get; set; } = true;

    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
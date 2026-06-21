using SrijanDEEP.API.Common;

namespace SrijanDEEP.API.DTOs;

public class VendorResponseDto
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
    public bool IsActive { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateVendorDto
{
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
}

public class UpdateVendorDto
{
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

    /// <summary>Lets the front end deactivate a vendor without ever deleting it.</summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Extra filter fields on top of the common paging/search params, used by
/// GET /api/vendors when callers want to filter rather than free-text search.
/// </summary>
public class VendorFilterParams : PaginationParams
{
    public string? GSTNumber { get; set; }
    public string? PANNumber { get; set; }
    public string? MSMENumber { get; set; }
    public bool? IsActive { get; set; }
}
using AutoMapper;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;
using SrijanDEEP.API.Repositories;

namespace SrijanDEEP.API.Services;

public interface IVendorService
{
    Task<PagedResult<VendorResponseDto>> GetPagedAsync(VendorFilterParams filter);
    Task<VendorResponseDto?> GetByURNNoAsync(string urnNo);
    Task<(bool Success, string Message, VendorResponseDto? Data)> CreateAsync(CreateVendorDto dto, string uploadedBy, string uploadIp);
    Task<(bool Success, string Message, VendorResponseDto? Data)> UpdateAsync(string urnNo, UpdateVendorDto dto, string modifiedBy, string modifyIp);
}

public class VendorService : IVendorService
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IMapper _mapper;

    public VendorService(IVendorRepository vendorRepository, IMapper mapper)
    {
        _vendorRepository = vendorRepository;
        _mapper = mapper;
    }

    // ─── GET paged list ───────────────────────────────────────────────────────

    public async Task<PagedResult<VendorResponseDto>> GetPagedAsync(VendorFilterParams filter)
    {
        var (items, totalCount) = await _vendorRepository.GetPagedAsync(filter);
        return new PagedResult<VendorResponseDto>
        {
            Items = _mapper.Map<List<VendorResponseDto>>(items),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalCount
        };
    }

    // ─── GET by URN_No ────────────────────────────────────────────────────────

    public async Task<VendorResponseDto?> GetByURNNoAsync(string urnNo)
    {
        var vendor = await _vendorRepository.GetByURNNoAsync(urnNo);
        return vendor is null ? null : _mapper.Map<VendorResponseDto>(vendor);
    }

    // ─── CREATE ───────────────────────────────────────────────────────────────

    public async Task<(bool Success, string Message, VendorResponseDto? Data)> CreateAsync(
        CreateVendorDto dto, string uploadedBy, string uploadIp)
    {
        // URN_No is the PK — enforce uniqueness
        if (await _vendorRepository.ExistsAsync(v => v.URN_No == dto.URN_No))
            return (false, $"A vendor with URN_No '{dto.URN_No}' already exists.", null);

        var vendor = _mapper.Map<Vendor>(dto);

        // Populate audit fields
        vendor.IsActive = true;
        vendor.Uploaded_by = uploadedBy;
        vendor.Uploaded_DateTime = DateTime.UtcNow;
        vendor.Upload_IP = uploadIp;

        await _vendorRepository.AddAsync(vendor);
        await _vendorRepository.SaveChangesAsync();

        return (true, "Vendor created successfully.", _mapper.Map<VendorResponseDto>(vendor));
    }

    // ─── UPDATE ───────────────────────────────────────────────────────────────

    public async Task<(bool Success, string Message, VendorResponseDto? Data)> UpdateAsync(
        string urnNo, UpdateVendorDto dto, string modifiedBy, string modifyIp)
    {
        var existing = await _vendorRepository.GetByURNNoAsync(urnNo);
        if (existing is null)
            return (false, "Vendor not found.", null);

        _mapper.Map(dto, existing);

        // Populate modify audit fields
        existing.Last_Modified_by = modifiedBy;
        existing.Last_Modified_DateTime = DateTime.UtcNow;
        existing.Modify_IP = modifyIp;

        _vendorRepository.Update(existing);
        await _vendorRepository.SaveChangesAsync();

        return (true, "Vendor updated successfully.", _mapper.Map<VendorResponseDto>(existing));
    }
}
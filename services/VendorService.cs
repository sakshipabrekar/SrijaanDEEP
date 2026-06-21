using AutoMapper;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;
using SrijanDEEP.API.Repositories;

namespace SrijanDEEP.API.Services;

public interface IVendorService
{
    Task<PagedResult<VendorResponseDto>> GetPagedAsync(VendorFilterParams filter);
    Task<VendorResponseDto?> GetByIdAsync(int vendorId);
    Task<(bool Success, string Message, VendorResponseDto? Data)> CreateAsync(CreateVendorDto dto);
    Task<(bool Success, string Message, VendorResponseDto? Data)> UpdateAsync(int vendorId, UpdateVendorDto dto);
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

    public async Task<VendorResponseDto?> GetByIdAsync(int vendorId)
    {
        var vendor = await _vendorRepository.GetByIdAsync(vendorId);
        return vendor is null ? null : _mapper.Map<VendorResponseDto>(vendor);
    }

    public async Task<(bool Success, string Message, VendorResponseDto? Data)> CreateAsync(CreateVendorDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.URNNumber) &&
            await _vendorRepository.ExistsAsync(v => v.URNNumber == dto.URNNumber))
        {
            return (false, $"A vendor with URN number '{dto.URNNumber}' already exists.", null);
        }

        var vendor = _mapper.Map<Vendor>(dto);
        vendor.IsActive = true;

        await _vendorRepository.AddAsync(vendor);
        await _vendorRepository.SaveChangesAsync();

        return (true, "Vendor created successfully.", _mapper.Map<VendorResponseDto>(vendor));
    }

    public async Task<(bool Success, string Message, VendorResponseDto? Data)> UpdateAsync(int vendorId, UpdateVendorDto dto)
    {
        var existing = await _vendorRepository.GetByIdAsync(vendorId);
        if (existing is null)
        {
            return (false, "Vendor not found.", null);
        }

        if (!string.IsNullOrWhiteSpace(dto.URNNumber) &&
            dto.URNNumber != existing.URNNumber &&
            await _vendorRepository.ExistsAsync(v => v.URNNumber == dto.URNNumber && v.VendorId != vendorId))
        {
            return (false, $"A vendor with URN number '{dto.URNNumber}' already exists.", null);
        }

        _mapper.Map(dto, existing);
        _vendorRepository.Update(existing);
        await _vendorRepository.SaveChangesAsync();

        return (true, "Vendor updated successfully.", _mapper.Map<VendorResponseDto>(existing));
    }
}
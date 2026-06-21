using AutoMapper;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;

namespace SrijanDEEP.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ---------------- Vendor ----------------
        CreateMap<Vendor, VendorResponseDto>();
        CreateMap<CreateVendorDto, Vendor>();
        CreateMap<UpdateVendorDto, Vendor>();

        // ---------------- Product ----------------
        CreateMap<Product, ProductResponseDto>()
            .ForMember(d => d.VendorOrganisationName,
                opt => opt.MapFrom(s => s.Vendor != null ? s.Vendor.VendorOrganisationName : null));
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        // ---------------- SupplyOrder ----------------
        CreateMap<SupplyOrder, SupplyOrderResponseDto>()
            .ForMember(d => d.ProductName,
                opt => opt.MapFrom(s => s.Product != null ? s.Product.ProductName : null));
        CreateMap<CreateSupplyOrderDto, SupplyOrder>();
        CreateMap<UpdateSupplyOrderDto, SupplyOrder>();

        // ---------------- UploadHistory ----------------
        CreateMap<UploadHistory, UploadHistoryResponseDto>()
            .ForMember(d => d.UploadedByUsername,
                opt => opt.MapFrom(s => s.UploadedByUser != null ? s.UploadedByUser.Username : null));

        // ---------------- SyncHistory ----------------
        CreateMap<SyncHistory, SyncHistoryResponseDto>();
        CreateMap<SyncHistory, SyncHistoryDetailDto>();
    }
}
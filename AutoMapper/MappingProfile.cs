using AutoMapper;
using srijaanDEEP.DTOs;
using srijaanDEEP.Models;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;

namespace SrijanDEEP.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ---------------- Vendor ----------------

        // CreateVendorDto → Vendor entity
        // Audit fields (Uploaded_by, Uploaded_DateTime, Upload_IP) are NOT mapped here;
        // they are set manually in VendorService.CreateAsync from the HTTP context.
        CreateMap<CreateVendorDto, Vendor>()
            .ForMember(dest => dest.Last_Modified_by, opt => opt.Ignore())
            .ForMember(dest => dest.Last_Modified_DateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Modify_IP, opt => opt.Ignore())
            .ForMember(dest => dest.Uploaded_by, opt => opt.Ignore())
            .ForMember(dest => dest.Uploaded_DateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Upload_IP, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore());

        // UpdateVendorDto → Vendor entity (patch onto existing)
        // Modify audit fields are set manually in VendorService.UpdateAsync.
        CreateMap<UpdateVendorDto, Vendor>()
            .ForMember(dest => dest.URN_No, opt => opt.Ignore())
            .ForMember(dest => dest.Uploaded_by, opt => opt.Ignore())
            .ForMember(dest => dest.Uploaded_DateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Upload_IP, opt => opt.Ignore())
            .ForMember(dest => dest.Last_Modified_by, opt => opt.Ignore())
            .ForMember(dest => dest.Last_Modified_DateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Modify_IP, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore());

        // Vendor entity → VendorResponseDto (all fields including audit)
        CreateMap<Vendor, VendorResponseDto>();

        // ---------------- Product ----------------

        // Product entity → ProductResponseDto, pulling vendor snapshot from navigation
        CreateMap<Product, ProductResponseDto>()
            .ForMember(d => d.Vendor_Org_Name,
                opt => opt.MapFrom(s => s.Vendor != null ? s.Vendor.Vendor_Org_Name : null));

        // CreateProductDto → Product entity
        // Audit fields are set manually in ProductService.CreateAsync from the HTTP context.
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uploaded_by, opt => opt.Ignore())
            .ForMember(dest => dest.Uploaded_DateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Upload_IP, opt => opt.Ignore())
            .ForMember(dest => dest.Last_Modified_by, opt => opt.Ignore())
            .ForMember(dest => dest.Last_Modified_DateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Modify_IP, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.Vendor, opt => opt.Ignore())
            .ForMember(dest => dest.SupplyOrders, opt => opt.Ignore());

        // UpdateProductDto → Product entity (patch onto existing)
        // Modify audit fields are set manually in ProductService.UpdateAsync.
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uploaded_by, opt => opt.Ignore())
            .ForMember(dest => dest.Uploaded_DateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Upload_IP, opt => opt.Ignore())
            .ForMember(dest => dest.Last_Modified_by, opt => opt.Ignore())
            .ForMember(dest => dest.Last_Modified_DateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Modify_IP, opt => opt.Ignore())
            .ForMember(dest => dest.Vendor, opt => opt.Ignore())
            .ForMember(dest => dest.SupplyOrders, opt => opt.Ignore());

        // ---------------- SupplyOrder ----------------

        // SupplyOrder entity → SupplyOrderResponseDto, pulling product + vendor snapshots
        CreateMap<SupplyOrder, SupplyOrderResponseDto>()
            .ForMember(d => d.Product_Name,
                opt => opt.MapFrom(s => s.Product != null ? s.Product.Product_Name : null))
            .ForMember(d => d.Vendor_Org_Name,
                opt => opt.MapFrom(s => s.Vendor != null ? s.Vendor.Vendor_Org_Name : null));

        // CreateSupplyOrderDto → SupplyOrder entity
        // Total_line_value is server-computed (Qty_Ordered × Unit_Rate) in DbContext.SaveChanges,
        // never trusted from the client.
        CreateMap<CreateSupplyOrderDto, SupplyOrder>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Total_line_value, opt => opt.Ignore())
            .ForMember(dest => dest.Uploaded_by, opt => opt.Ignore())
            .ForMember(dest => dest.Uploaded_DateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Upload_IP, opt => opt.Ignore())
            .ForMember(dest => dest.Last_Modified_by, opt => opt.Ignore())
            .ForMember(dest => dest.Last_Modified_DateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Modify_IP, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.Vendor, opt => opt.Ignore());

        // UpdateSupplyOrderDto → SupplyOrder entity (patch onto existing)
        CreateMap<UpdateSupplyOrderDto, SupplyOrder>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Total_line_value, opt => opt.Ignore())
            .ForMember(dest => dest.Uploaded_by, opt => opt.Ignore())
            .ForMember(dest => dest.Uploaded_DateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Upload_IP, opt => opt.Ignore())
            .ForMember(dest => dest.Last_Modified_by, opt => opt.Ignore())
            .ForMember(dest => dest.Last_Modified_DateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Modify_IP, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.Vendor, opt => opt.Ignore());

        // ---------------- UploadHistory ----------------
        CreateMap<UploadHistory, UploadHistoryResponseDto>()
            .ForMember(d => d.UploadedByUsername,
                opt => opt.MapFrom(s => s.UploadedByUser != null ? s.UploadedByUser.Username : null));

        // ---------------- SyncHistory ----------------
        CreateMap<SyncHistory, SyncHistoryResponseDto>();
        CreateMap<SyncHistory, SyncHistoryDetailDto>();
    }
}
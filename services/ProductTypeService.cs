using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using srijaanDEEP.DTOs;
using srijaanDEEP.Models;
using srijaanDEEP.Repository;
using srijaanDEEP.services;

namespace srijaanDEEP.services
{
    // ─── Interface ───────────────────────────────────────────────────────────────
    public interface IProductTypeService
    {
        Task<IEnumerable<ProductTypeResponseDto>> GetAllAsync();
        Task<ProductTypeResponseDto> GetByIdAsync(int id);
        Task<ProductTypeResponseDto> CreateAsync(ProductTypeCreateDto dto, string uploadedBy, string uploadIp);
        Task<ProductTypeResponseDto> UpdateAsync(int id, ProductTypeUpdateDto dto, string modifiedBy, string modifyIp);
        Task<bool> DeleteAsync(int id);
    }
}


    public class ProductTypeService : IProductTypeService
    {
        private readonly IProductTypeRepository _repository;
        private readonly IMapper _mapper;

        public ProductTypeService(IProductTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductTypeResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductTypeResponseDto>>(entities);
        }

        public async Task<ProductTypeResponseDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ProductTypeResponseDto>(entity);
        }

        public async Task<ProductTypeResponseDto> CreateAsync(
            ProductTypeCreateDto dto, string uploadedBy, string uploadIp)
        {
            var entity = _mapper.Map<ProductTypeMaster>(dto);
            entity.Uploaded_by = uploadedBy;
            entity.Uploaded_DateTime = DateTime.UtcNow;
            entity.Upload_IP = uploadIp;

            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<ProductTypeResponseDto>(created);
        }

        public async Task<ProductTypeResponseDto> UpdateAsync(
            int id, ProductTypeUpdateDto dto, string modifiedBy, string modifyIp)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            entity.Last_Modified_by = modifiedBy;
            entity.Last_Modified_DateTime = DateTime.UtcNow;
            entity.Modify_IP = modifyIp;

            var updated = await _repository.UpdateAsync(entity);
            return _mapper.Map<ProductTypeResponseDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
            => await _repository.DeleteAsync(id);
    }

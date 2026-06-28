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
   
    public interface IDefencePlatformService
    {
        Task<IEnumerable<DefencePlatformResponseDto>> GetAllAsync();
        Task<DefencePlatformResponseDto> GetByIdAsync(int id);
        Task<DefencePlatformResponseDto> CreateAsync(DefencePlatformCreateDto dto, string uploadedBy, string uploadIp);
        Task<DefencePlatformResponseDto> UpdateAsync(int id, DefencePlatformUpdateDto dto, string modifiedBy, string modifyIp);
        Task<bool> DeleteAsync(int id);
    }
}



    public class DefencePlatformService : IDefencePlatformService
    {
        private readonly IDefencePlatformRepository _repository;
        private readonly IMapper _mapper;

        public DefencePlatformService(IDefencePlatformRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DefencePlatformResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<DefencePlatformResponseDto>>(entities);
        }

        public async Task<DefencePlatformResponseDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<DefencePlatformResponseDto>(entity);
        }

        public async Task<DefencePlatformResponseDto> CreateAsync(
            DefencePlatformCreateDto dto, string uploadedBy, string uploadIp)
        {
            var entity = _mapper.Map<DefencePlatformMaster>(dto);
            entity.Uploaded_by = uploadedBy;
            entity.Uploaded_DateTime = DateTime.UtcNow;
            entity.Upload_IP = uploadIp;

            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<DefencePlatformResponseDto>(created);
        }

        public async Task<DefencePlatformResponseDto> UpdateAsync(
            int id, DefencePlatformUpdateDto dto, string modifiedBy, string modifyIp)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            entity.Last_Modified_by = modifiedBy;
            entity.Last_Modified_DateTime = DateTime.UtcNow;
            entity.Modify_IP = modifyIp;

            var updated = await _repository.UpdateAsync(entity);
            return _mapper.Map<DefencePlatformResponseDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
            => await _repository.DeleteAsync(id);
    }

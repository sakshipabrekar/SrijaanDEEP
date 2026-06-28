using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using srijaanDEEP.DTOs;
using srijaanDEEP.services;

namespace srijaanDEEP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DefencePlatformController : ControllerBase
    {
        private readonly IDefencePlatformService _service;

        public DefencePlatformController(IDefencePlatformService service)
        {
            _service = service;
        }

        // GET: api/DefencePlatform
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // GET: api/DefencePlatform/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Defence Platform with ID {id} not found." });

            return Ok(result);
        }

        // POST: api/DefencePlatform
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DefencePlatformCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var uploadedBy = User.Identity?.Name ?? "System";
            var uploadIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var created = await _service.CreateAsync(dto, uploadedBy, uploadIp);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/DefencePlatform/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DefencePlatformUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modifiedBy = User.Identity?.Name ?? "System";
            var modifyIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var updated = await _service.UpdateAsync(id, dto, modifiedBy, modifyIp);
            if (updated == null)
                return NotFound(new { message = $"Defence Platform with ID {id} not found." });

            return Ok(updated);
        }

        
    }
}
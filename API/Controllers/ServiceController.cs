using API.Data;
using API.Models;
using API.Models.DTO.Service;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ServiceController(
            ApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var business = await _context.Businesses.FindAsync(dto.BusinessId);
            if (business == null)
            {
                return NotFound($"Business with ID {dto.BusinessId} not found.");
            }

            var service = _mapper.Map<Models.Service>(dto);
            service.Id = Guid.NewGuid();
            service.CreatedAt = DateTime.UtcNow;
            service.UpdatedAt = null;

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServiceById), new { id = service.Id }, service);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(Guid id)
        {
            var service = await _context.Services
                .Include(s => s.ApplicationUserServices)
                    .ThenInclude(aus => aus.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
            {
                return NotFound($"Service with ID {id} not found.");
            }

            var serviceDto = _mapper.Map<ServiceDto>(service);
            return Ok(serviceDto);
        }
    }
}

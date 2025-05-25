using API.Data;
using API.Models;
using API.Models.DTO.Business;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public BusinessController(
            ApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/create")]
        public async Task<IActionResult> CreateBusiness([FromBody] CreateBusinessDto dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var business = _mapper.Map<Business>(dto);
            business.Id = Guid.NewGuid();

            _context.Businesses.Add(business);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBusinessById), new { id = business.Id }, business);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusinessById(Guid id)
        {
            var business = await _context.Businesses.FindAsync(id);
            if (business == null)
                return NotFound();

            return Ok(business);
        }
    }
}

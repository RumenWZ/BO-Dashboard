using API.Data;
using API.Models.DTO.Appointment;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public CustomerController(
                ApplicationDbContext context,
                IMapper mapper
            )
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{customerId}/appointments")]
        public async Task<IActionResult> GetAppointmentsForCustomer(string customerId)
        {
            var customer = await _context.Users
                .Include(u => u.AppointmentsAsCustomer)
                    .ThenInclude(a => a.Service)
                .Include(u => u.AppointmentsAsCustomer)
                    .ThenInclude(a => a.Employee)
                .FirstOrDefaultAsync(u => u.Id == customerId);


            if (customer == null)
                return NotFound($"Customer with ID {customerId} not found.");

            var appointments = customer.AppointmentsAsCustomer
                .OrderByDescending(a => a.StartTime)
                .ToList();

            var result = _mapper.Map<List<AppointmentDto>>(appointments);
            return Ok(result);
        }
    }
}

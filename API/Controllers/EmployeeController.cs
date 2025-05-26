using API.Data;
using API.Models;
using API.Models.DTO.Appointment;
using API.Models.DTO.Service;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public EmployeeController(
                ApplicationDbContext context,
                IMapper mapper
            )
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{employeeId}/appointments/upcoming")]
        public async Task<IActionResult> GetUpcomingAppointmentsForEmployee(string employeeId)
        {
            var employee = await _context.Users
                .Include(u => u.AppointmentsAsEmployee)
                    .ThenInclude(a => a.Service)
                .Include(u => u.AppointmentsAsEmployee)
                    .ThenInclude(a => a.Customer)
                .FirstOrDefaultAsync(u => u.Id == employeeId);

            if (employee == null)
                return NotFound($"Employee with ID {employeeId} not found.");

            var now = DateTime.UtcNow;

            var upcomingAppointments = employee.AppointmentsAsEmployee
                .Where(a => a.StartTime > now && a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.StartTime)
                .ToList();

            var result = _mapper.Map<List<AppointmentDto>>(upcomingAppointments);
            return Ok(result);
        }

        [HttpGet("{employeeId}/services")]
        public async Task<IActionResult> GetServicesByEmployeeId(string employeeId)
        {
            var user = await _context.Users
                .Include(u => u.ApplicationUserServices)
                    .ThenInclude(aus => aus.Service)
                .FirstOrDefaultAsync(u => u.Id == employeeId);

            if (user == null)
            {
                return NotFound($"Employee with ID {employeeId} was not found or is not an employee.");
            }

            var services = user.ApplicationUserServices
                .Select(aus => aus.Service)
                .Where(s => s != null)
                .ToList();

            if (services.Count == 0)
            {
                return NotFound($"No services found for employee with ID {employeeId}.");
            }

            var serviceDtos = _mapper.Map<IEnumerable<ServiceDto>>(services);

            return Ok(serviceDtos);
        }

        [HttpPost("{employeeId}/services")]
        public async Task<IActionResult> AddServiceToEmployee(string employeeId, [FromBody] Guid serviceId)
        {
            var user = await _context.Users
                .Include(u => u.ApplicationUserServices)
                .FirstOrDefaultAsync(u => u.Id == employeeId);
            if (user == null)
            {
                return NotFound($"Employee with ID {employeeId} not found.");
            }
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
            {
                return NotFound($"Service with ID {serviceId} not found.");
            }
            if (user.ApplicationUserServices.Any(aus => aus.ServiceId == serviceId))
            {
                return BadRequest($"Service with ID {serviceId} is already assigned to employee with ID {employeeId}.");
            }
            user.ApplicationUserServices.Add(new ApplicationUserService
            {
                User = user,
                Service = service,
                ServiceId = serviceId
            });
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{employeeId}/services/{serviceId}")]
        public async Task<IActionResult> RemoveServiceFromEmployee(string employeeId, Guid serviceId)
        {
            var user = await _context.Users
                .Include(u => u.ApplicationUserServices)
                .FirstOrDefaultAsync(u => u.Id == employeeId);
            if (user == null)
            {
                return NotFound($"Employee with ID {employeeId} not found.");
            }
            var service = user.ApplicationUserServices.FirstOrDefault(aus => aus.ServiceId == serviceId);
            if (service == null)
            {
                return NotFound($"Service with ID {serviceId} is not assigned to employee with ID {employeeId}.");
            }
            user.ApplicationUserServices.Remove(service);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

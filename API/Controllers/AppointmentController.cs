using API.Data;
using API.Models;
using API.Models.DTO.Appointment;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AppointmentController(
                ApplicationDbContext context,
                IMapper mapper
            )
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.EndTime <= dto.StartTime)
                return BadRequest("EndTime must be after StartTime.");

            if (dto.StartTime < DateTime.UtcNow)
                return BadRequest("StartTime must be in the future.");

            var service = await _context.Services.FindAsync(dto.ServiceId);
            if (service == null)
                return NotFound($"Service with ID {dto.ServiceId} not found.");

            var employee = await _context.Users.FindAsync(dto.EmployeeId);
            if (employee == null)
                return NotFound($"Employee with ID {dto.EmployeeId} not found.");

            var customer = await _context.Users.FindAsync(dto.CustomerId);
            if (customer == null)
                return NotFound($"Customer with ID {dto.CustomerId} not found.");

            var appointment = _mapper.Map<Appointment>(dto);

            appointment.Id = Guid.NewGuid();
            appointment.Service = service;
            appointment.Employee = employee;
            appointment.Customer = customer;
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.Status = AppointmentStatus.Cancelled;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<AppointmentDto>(appointment);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(Guid id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Employee)
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            var result = _mapper.Map<AppointmentDto>(appointment);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAppointment(Guid id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound($"Appointment with ID {id} not found.");
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Employee)
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (appointment == null)
                return NotFound($"Appointment with ID {id} not found.");
            if (dto.StartTime < DateTime.UtcNow)
                return BadRequest("StartTime must be in the future.");
            if (dto.EndTime <= dto.StartTime)
                return BadRequest("EndTime must be after StartTime.");

            appointment.StartTime = dto.StartTime;
            appointment.EndTime = dto.EndTime;
            appointment.Status = dto.Status;
            appointment.Notes = dto.Notes;
            appointment.UpdatedAt = DateTime.UtcNow;

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            var result = _mapper.Map<AppointmentDto>(appointment);
            return Ok(result);
        }


        [HttpGet("customer/{customerId}/appointments")]
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

        [HttpGet("employee/{employeeId}/appointments/upcoming")]
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

    }
}

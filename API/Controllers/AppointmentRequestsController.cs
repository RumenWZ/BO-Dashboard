using API.Data;
using API.Models;
using API.Models.DTO.AppointmentRequests;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AppointmentRequestsController(
            ApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointmentRequest([FromBody] CreateAppointmentRequestDto dto)
        {

            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            if (dto.AvailableTo <= dto.AvailableFrom)
                return BadRequest("EndTime must be after StartTime.");
            if (dto.AvailableFrom < DateTime.UtcNow)
                return BadRequest("StartTime must be in the future.");
            if (dto.AvailableTo < DateTime.UtcNow)
                return BadRequest("EndTime must be in the future.");

            var customer = await _context.Users.FindAsync(dto.CustomerId);
            if (customer == null)
                return NotFound($"Customer with ID {dto.CustomerId} not found.");
            var service = await _context.Services.FindAsync(dto.ServiceId);
            if (service == null)
                return NotFound($"Service with ID {dto.ServiceId} not found.");

            var newAppointmentRequest = _mapper.Map<AppointmentRequest>(dto);

            newAppointmentRequest.Id = Guid.NewGuid();
            newAppointmentRequest.Customer = customer;
            newAppointmentRequest.Service = service;
            newAppointmentRequest.CreatedAt = DateTime.UtcNow;
            newAppointmentRequest.Status = RequestStatus.Pending;

            _context.AppointmentRequests.Add(newAppointmentRequest);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<AppointmentRequestDto>(newAppointmentRequest);
            return CreatedAtAction(nameof(GetAppointmentRequestById), new { Id = newAppointmentRequest.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentRequestById(Guid id)
        {
            var appointmentRequest = await _context.AppointmentRequests
                .Include(ar => ar.Customer)
                .Include(ar => ar.Service)
                .FirstOrDefaultAsync(ar => ar.Id == id);

            if (appointmentRequest == null)
                return NotFound($"Appointment request with ID {id} not found.");

            var result = _mapper.Map<AppointmentRequestDto>(appointmentRequest);
            return Ok(result);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveAppointmentRequest(Guid appointmentId, ApproveAppointmetRequestDto dto)
        {
            var appointmentRequest = await _context.AppointmentRequests
                .Include(ar => ar.Customer)
                .Include(ar => ar.Service)
                .FirstOrDefaultAsync(ar => ar.Id == appointmentId);
           
            if (appointmentRequest == null)
                return NotFound($"Appointment request with ID {appointmentId} not found.");

            var employee = await _context.Users
                .Include(e => e.ApplicationUserServices)
                .FirstOrDefaultAsync(e => e.Id == dto.EmployeeId);

            if (employee == null)
                return NotFound($"Employee with ID {dto.EmployeeId} not found.");

            var canProvideService = employee.ApplicationUserServices.Any(s => s.ServiceId == appointmentRequest.ServiceId);
            if (!canProvideService)
                return BadRequest("Employee cannot perform the requested service.");

            if (dto.StartTime < appointmentRequest.AvailableFrom || dto.StartTime > appointmentRequest.AvailableTo)
                return BadRequest($"Start time must be within the requested availability window by the customer. The customer is available from {appointmentRequest.AvailableFrom} to {appointmentRequest.AvailableTo}");

            var serviceDuration = appointmentRequest.Service.DurationInMinutes;
            var calculatedEndTime = dto.StartTime.AddMinutes(serviceDuration);

            var endTime = dto.EndTime != null && dto.EndTime != default(DateTime)
                ? dto.EndTime.Value
                : calculatedEndTime;

            if (endTime > appointmentRequest.AvailableTo)
                return BadRequest("End time exceeds the requested availability window.");

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                StartTime = dto.StartTime,
                EndTime = endTime,
                Customer = appointmentRequest.Customer,
                Employee = employee,
                Service = appointmentRequest.Service,
                CreatedAt = DateTime.UtcNow,
                Notes = appointmentRequest.Notes,
                Status = AppointmentStatus.Scheduled
            };

            _context.Appointments.Add(appointment);
            
            appointmentRequest.Status = RequestStatus.Accepted;
            appointmentRequest.Appointment = appointment;

            await _context.SaveChangesAsync();

            return Ok(new { appointment.Id });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointmentRequest(Guid id, [FromBody] UpdateAppointmentRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appointmentRequest = await _context.AppointmentRequests.FindAsync(id);
            if (appointmentRequest == null)
                return NotFound($"Appointment request with ID {id} not found.");

            if (appointmentRequest.Status != RequestStatus.Pending)
                return BadRequest("Only pending requests can be updated.");

            if (dto.AvailableTo <= dto.AvailableFrom)
                return BadRequest("EndTime must be after StartTime.");

            if (dto.AvailableFrom < DateTime.UtcNow)
                return BadRequest("StartTime must be in the future.");

            if (dto.AvailableTo < DateTime.UtcNow)
                return BadRequest("EndTime must be in the future.");

            if (dto.ServiceId != appointmentRequest.ServiceId)
            {
                var service = await _context.Services.FindAsync(dto.ServiceId);
                if (service == null)
                    return NotFound($"Service with ID {dto.ServiceId} not found.");
                appointmentRequest.ServiceId = dto.ServiceId;
            }

            appointmentRequest.Notes = dto.Notes;
            appointmentRequest.AvailableFrom = dto.AvailableFrom;
            appointmentRequest.AvailableTo = dto.AvailableTo;
            _context.AppointmentRequests.Update(appointmentRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelAppointmentRequest(Guid id)
        {
            var appointmentRequest = await _context.AppointmentRequests.FindAsync(id);

            if (appointmentRequest == null)
                return NotFound($"Appointment request with ID {id} not found.");

            if (appointmentRequest.Status != RequestStatus.Pending)
                return BadRequest("Only pending requests can be cancelled.");

            appointmentRequest.Status = RequestStatus.Cancelled;
            _context.AppointmentRequests.Update(appointmentRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("service/{serviceId}")]
        public async Task<IActionResult> GetAppointmentRequestsByServiceId(Guid serviceId, [FromQuery] string? status)
        {
            var appointmentRequestsQuery = _context.AppointmentRequests
                .Include(ar => ar.Customer)
                .Include(ar => ar.Service)
                .Where(ar => ar.ServiceId == serviceId);

            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<RequestStatus>(status, true, out var parsedStatus))
                {
                    appointmentRequestsQuery = appointmentRequestsQuery.Where(aq => aq.Status == parsedStatus);
                }
                else
                {
                    return BadRequest($"Invalid status value: {status}");
                }
            }

            var appointmentRequests = await appointmentRequestsQuery.ToListAsync();
            var results = _mapper.Map<IEnumerable<AppointmentRequestDto>>(appointmentRequests);

            return Ok(results);
        }
    }
}

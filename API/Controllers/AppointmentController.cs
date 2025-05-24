using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        [HttpPost("create")]
        public IActionResult CreateAppointment([FromBody] Appointment appointment)
        {
            if (appointment == null)
            {
                return BadRequest("Appointment data is required.");
            }
            // Here you would typically save the appointment to the database
            // For demonstration, we will just return the appointment back
            return Ok();
        }
    }
}

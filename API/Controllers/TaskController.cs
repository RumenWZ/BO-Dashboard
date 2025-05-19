using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTasks()
        {
            return Ok(new List<object>
            {
                new { Id = 1, Title = "Task 1", Status = "In Progress" },
                new { Id = 2, Title = "Task 2", Status = "Pending" }
            });
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAdminTasks()
        {
            return Ok(new List<object>
            {
                new { Id = 3, Title = "Task 3", Status = "Urgent" },
                new { Id = 4, Title = "Task 4", Status = "Pending" }
            });
        }
    }
}

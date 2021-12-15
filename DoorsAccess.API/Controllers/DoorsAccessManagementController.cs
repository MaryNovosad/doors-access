using System.Threading.Tasks;
using DoorsAccess.API.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsAccess.API.Controllers
{
    [Route("api/doors/")]
    [ApiController]
    public class DoorsAccessManagementController : ControllerBase
    {
        [HttpPut("{doorId:long}/access")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllowDoorAccess(long doorId, [FromBody] AllowDoorAccessRequest request)
        {
            return Ok();
        }
    }
}

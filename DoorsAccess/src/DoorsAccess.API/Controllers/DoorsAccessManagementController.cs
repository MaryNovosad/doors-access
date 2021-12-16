using System.Threading.Tasks;
using DoorsAccess.API.Requests;
using DoorsAccess.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsAccess.API.Controllers
{
    [Route("v1/doors/")]
    [ApiController]
    public class DoorsAccessManagementController : ControllerBase
    {
        private readonly IDoorsAccessService _doorsAccessService;

        public DoorsAccessManagementController(IDoorsAccessService doorsAccessService)
        {
            _doorsAccessService = doorsAccessService;
        }

        [HttpPut("{doorId:long}/access")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllowDoorAccess(long doorId, [FromBody] AllowDoorAccessRequest request)
        {
            await _doorsAccessService.AllowDoorAccessAsync(doorId, request.UsersIds);

            return Ok();
        }
    }
}

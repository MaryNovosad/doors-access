using System;
using DoorsAccess.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DoorsAccess.API.Infrastructure;

namespace DoorsAccess.API.Controllers
{
    [Route("v1/doors")]
    [ApiController]
    [Authorize]
    public class DoorsController : ControllerBase
    {
        private readonly IDoorsAccessService _doorsAccessService;

        public DoorsController(IDoorsAccessService doorsAccessService)
        {
            _doorsAccessService = doorsAccessService;
        }

        [HttpPut("{doorId:long}/state/open")]
        public async Task<IActionResult> OpenDoor(long doorId)
        {
            var userId = HttpContext.User.GetUserId();

            await _doorsAccessService.OpenDoorAsync(doorId, userId);

            return Ok();
        }
    }
}

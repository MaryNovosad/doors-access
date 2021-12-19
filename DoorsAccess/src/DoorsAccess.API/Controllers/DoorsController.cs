using System;
using DoorsAccess.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DoorsAccess.API.Controllers
{
    [Route("v1/doors")]
    [ApiController]
    [Authorize]
    public class DoorsController : ControllerBase
    {
        private readonly IDoorsAccessService _doorsAccessService;
        private IDoorsAccessHistoryService _doorsAccessHistoryService;

        public DoorsController(IDoorsAccessService doorsAccessService, IDoorsAccessHistoryService doorsAccessHistoryService)
        {
            _doorsAccessService = doorsAccessService;
            _doorsAccessHistoryService = doorsAccessHistoryService;
        }

        [HttpPut("{doorId:long}/state/open")]
        public async Task<IActionResult> OpenDoor(long doorId)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new InvalidOperationException($"User principal info is not complete: claim {ClaimTypes.NameIdentifier} is missing");
            }

            long.TryParse(userIdClaim.Value, out long userId);

            await _doorsAccessService.OpenDoorAsync(doorId, userId);

            return Ok();
        }
    }
}

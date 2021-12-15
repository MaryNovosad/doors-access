using System;
using System.Collections;
using System.Collections.Generic;
using DoorsAccess.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> OpenDoor(int doorId)
        {
            var userIdClaim = HttpContext.User.Claims.ToList().FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
            {
                throw new ArgumentException();
            }

            long.TryParse(userIdClaim.Value, out long userId);

            await _doorsAccessService.GrantDoorAccessAsync(doorId, userId);

            return Ok();
        }
    }
}

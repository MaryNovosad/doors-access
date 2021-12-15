using System;
using System.Threading.Tasks;
using DoorsAccess.API.Requests;
using DoorsAccess.Domain;
using DoorsAccess.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsAccess.API.Controllers
{
    [Route("api/doors")]
    [ApiController]
    public class DoorsConfigurationController : ControllerBase
    {
        private readonly IDoorsConfigurationService _doorsConfigurationService;

        public DoorsConfigurationController(IDoorsConfigurationService doorsConfigurationService)
        {
            _doorsConfigurationService = doorsConfigurationService;
        }

        [HttpPut("{doorId:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateOrUpdateDoor(CreateOrUpdateDoorRequest request)
        {
            await _doorsConfigurationService.CreateOrUpdateDoorAsync(MapDoorInfo(request));

            return Ok();
        }

        [HttpPatch("{doorId:long}")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> ChangeDoorActivationState(long doorId, bool isActivated)
        {
            throw new NotImplementedException();
        }

        private DoorInfo MapDoorInfo(CreateOrUpdateDoorRequest request)
        {
            return new DoorInfo
            {
                Id = request.DoorId,
                IsDeactivated = request.IsDeactivated,
                Name = request.DoorName
            };
        }
    }
}

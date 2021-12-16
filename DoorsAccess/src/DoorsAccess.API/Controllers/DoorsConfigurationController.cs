﻿using System;
using System.Threading.Tasks;
using DoorsAccess.API.Requests;
using DoorsAccess.API.Responses;
using DoorsAccess.DAL;
using DoorsAccess.Domain;
using DoorsAccess.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsAccess.API.Controllers
{
    [Route("v1/doors/")]
    [ApiController]
    public class DoorsConfigurationController : ControllerBase
    {
        private readonly IDoorsConfigurationService _doorsConfigurationService;

        public DoorsConfigurationController(IDoorsConfigurationService doorsConfigurationService)
        {
            _doorsConfigurationService = doorsConfigurationService;
        }

        [HttpGet("{doorId:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetDoorResponse>> GetDoor(long doorId)
        {
            var door = await _doorsConfigurationService.GetDoorAsync(doorId);

            if (door == null)
            {
                return NotFound();
            }

            return MapDoorResponse(door);
        }

        [HttpPut("{doorId:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateOrUpdateDoor(long doorId, [FromBody] CreateOrUpdateDoorRequest request)
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

        private GetDoorResponse MapDoorResponse(Door door)
        {
            return new GetDoorResponse
            {
                Id = door.Id,
                Name = door.Name,
                CreatedAt = door.CreatedAt,
                UpdatedAt = door.UpdatedAt,
                State = (Responses.DoorState)door.State,
                IsDeactivated = door.IsDeactivated
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DoorsAccess.API.Responses;
using DoorsAccess.Domain;
using DoorsAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsAccess.API.Controllers
{
    [Route("v1/doors/history")]
    [ApiController]
    [Authorize]
    public class DoorsAccessHistoryController : ControllerBase
    {
        private readonly IDoorsAccessHistoryService _doorsAccessHistoryService;

        public DoorsAccessHistoryController(IDoorsAccessHistoryService doorsAccessHistoryService)
        {
            _doorsAccessHistoryService = doorsAccessHistoryService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<DoorsAccessHistoryResponse> Get()
        {
            var doorEvents = await _doorsAccessHistoryService.GetDoorAccessHistoryAsync();

            return CreateDoorsAccessHistoryResponse(doorEvents);
        }

        [HttpGet("user/{userId:long}")]
        public async Task<ActionResult<DoorsAccessHistoryResponse>> Get(long userId)
        {
            if (!HttpContext.User.IsInRole("Admin"))
            {
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    throw new InvalidOperationException($"User principal info is not complete: claim {ClaimTypes.NameIdentifier} is missing");
                }

                long.TryParse(userIdClaim.Value, out long userIdFromClaim);

                if (userIdFromClaim != userId)
                {
                    return Forbid();
                }
            }
            var doorEvents = await _doorsAccessHistoryService.GetDoorAccessHistoryAsync(userId);

            return CreateDoorsAccessHistoryResponse(doorEvents);
        }

        private DoorsAccessHistoryResponse CreateDoorsAccessHistoryResponse(IList<DetailedDoorEventLog> doorEventLogs)
        {
            return new DoorsAccessHistoryResponse
            {
                DoorEvents = doorEventLogs.Select(l => new Responses.DoorEventLog
                {
                    UserId = l.UserId,
                    DoorId = l.DoorId,
                    DoorName = l.DoorName,
                    IsDoorDeactivated = l.IsDoorDeactivated,
                    Event = (Responses.DoorEvent)l.EventType,
                    TimeStamp = l.TimeStamp
                }).ToList()
            };
        }
    }
}

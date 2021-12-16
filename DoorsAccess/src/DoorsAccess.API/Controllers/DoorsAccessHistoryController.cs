using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoorsAccess.API.Responses;
using DoorsAccess.DAL;
using DoorsAccess.Domain;
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
        public async Task<DoorsAccessHistoryResponse> Get(long userId)
        {
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
                    Event = (Responses.DoorState)l.Event,
                    TimeStamp = l.TimeStamp
                }).ToList()
            };
        }
    }
}

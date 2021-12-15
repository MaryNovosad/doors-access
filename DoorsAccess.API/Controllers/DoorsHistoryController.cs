using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using DoorsAccess.API.Responses;
using DoorsAccess.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoorsAccess.API.Controllers
{
    [Route("v1/doors/history")]
    [ApiController]
    [Authorize]
    public class DoorsHistoryController : ControllerBase
    {
        private readonly IDoorsAccessHistoryService _doorsAccessHistoryService;

        public DoorsHistoryController(IDoorsAccessHistoryService doorsAccessHistoryService)
        {
            _doorsAccessHistoryService = doorsAccessHistoryService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<DoorHistoryResponse> Get()
        {
            var doorEvents = await _doorsAccessHistoryService.GetDoorAccessHistoryAsync();

            return new DoorHistoryResponse();
        }

        [HttpGet("user/{userId:long}")]
        public async Task<DoorHistoryResponse> Get(long userId)
        {
            var doorEvents = await _doorsAccessHistoryService.GetDoorAccessHistoryAsync(userId);

            return new DoorHistoryResponse();
        }
    }
}

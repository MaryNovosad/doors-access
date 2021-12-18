using System;

namespace DoorsAccess.API.Responses
{
    public class GetDoorResponse
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DoorState State { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeactivated { get; set; }
    }
}

using System;
using DoorsAccess.Models.Enums;

namespace DoorsAccess.Models
{
    public class Door
    {
        public long Id { get; set; }
        
        public string? Name { get; set; }

        public DoorState State { get; set; } = DoorState.Closed;

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeactivated { get; set; }
    }
}

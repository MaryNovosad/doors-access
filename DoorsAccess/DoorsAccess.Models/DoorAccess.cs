using System;

namespace DoorsAccess.Models
{
    public class DoorAccess
    {
        public long DoorId { get; set; }
        public long UserId { get; set; }
       
        public bool IsOwner { get; set; }
        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeactivated { get; set; } = false;
    }
}

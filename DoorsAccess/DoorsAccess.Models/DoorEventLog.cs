using System;
using DoorsAccess.Models.Enums;

namespace DoorsAccess.Models
{
    public class DoorEventLog
    {
        public long DoorId { get; set; }
        public long UserId { get; set; }
        public DateTime TimeStamp { get; set; }
        public DoorEventType EventType { get; set; }
    }
}

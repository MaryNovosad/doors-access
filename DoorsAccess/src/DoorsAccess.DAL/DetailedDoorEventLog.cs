using System;

namespace DoorsAccess.DAL
{
    // save them to Table storage maybe
    public class DetailedDoorEventLog : DoorEventLog
    {
        public string? DoorName { get; set; }
        public bool IsDoorDeactivated { get; set; }
    }
}

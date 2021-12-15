using System;

namespace DoorsAccess.DAL
{
    // save them to Table storage maybe
    public class DoorEventLog
    {
        public long DoorId { get; set; }
        public long UserId { get; set; }
        public DateTime TimeStamp { get; set; }
        public DoorEvent Event { get; set; }
    }
}

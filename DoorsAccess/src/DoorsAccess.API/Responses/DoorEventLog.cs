using System;

namespace DoorsAccess.API.Responses;

public class DoorEventLog
{
    public long DoorId { get; set; }
    public string DoorName { get; set; }
    public bool IsDoorDeactivated { get; set; }
    public long UserId { get; set; }
    public DateTime TimeStamp { get; set; }
    public DoorEvent Event { get; set; }
}
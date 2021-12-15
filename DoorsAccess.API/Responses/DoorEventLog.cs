using System;

namespace DoorsAccess.API.Responses;

public class DoorEventLog
{
    public long DoorId { get; set; }
    public long UserId { get; set; }
    public DateTime TimeStamp { get; set; }
    public DoorState Event { get; set; }
}
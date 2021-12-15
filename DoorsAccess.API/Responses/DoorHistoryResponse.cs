using System.Collections.Generic;

namespace DoorsAccess.API.Responses;

public class DoorHistoryResponse
{
    public ICollection<DoorEventLog> DoorEvents { get; set; }
}
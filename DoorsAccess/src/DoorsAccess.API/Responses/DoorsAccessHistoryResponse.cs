using System.Collections.Generic;

namespace DoorsAccess.API.Responses;

public class DoorsAccessHistoryResponse
{
    public IList<DoorEventLog> DoorEvents { get; set; }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DoorsAccess.API.Responses;

[JsonConverter(typeof(StringEnumConverter))]
public enum DoorState
{
    AccessGranted = 1,
    Opened = 2,
    Closed = 3
}
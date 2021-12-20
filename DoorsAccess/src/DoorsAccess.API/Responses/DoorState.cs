using System.Text.Json.Serialization;

namespace DoorsAccess.API.Responses;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DoorState
{
    AccessGranted = 1,
    Opened = 2,
    Closed = 3
}
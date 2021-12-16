using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DoorsAccess.API.Responses
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DoorEvent
    {
        AccessGranted = 1,
        AccessDenied = 2,
        DeactivatedDoorAccessAttempt = 3,
        DoorOpened = 4,
        DoorClosed = 5
    }
}

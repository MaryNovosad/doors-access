using System.Text.Json.Serialization;

namespace DoorsAccess.API.Responses
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DoorEvent
    {
        AccessGranted = 1,
        AccessDenied = 2,
        DeactivatedDoorAccessAttempt = 3,
        DoorOpened = 4,
        DoorClosed = 5
    }
}

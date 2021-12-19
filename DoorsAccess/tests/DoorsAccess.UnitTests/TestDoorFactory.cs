using DoorsAccess.Models;
using DoorsAccess.Models.Enums;

namespace DoorsAccess.UnitTests;

public class TestDoorFactory
{
    public static Door Create(long doorId = TestConstants.DoorId, string doorName = TestConstants.DoorName, bool isDeactivated = false)
    {
        return new Door
        {
            Id = doorId,
            Name = doorName,
            IsDeactivated = isDeactivated,
            CreatedAt = TestConstants.DoorDateTime,
            UpdatedAt = TestConstants.DoorDateTime,
            State = DoorState.Closed
        };
    }
}
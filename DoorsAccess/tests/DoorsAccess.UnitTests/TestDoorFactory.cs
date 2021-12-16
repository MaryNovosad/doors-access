using System;
using DoorsAccess.DAL;

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
            CreatedAt = new DateTime(2021, 12, 1),
            UpdatedAt = new DateTime(2021, 12, 1),
            State = DoorState.Closed
        };
    }
}
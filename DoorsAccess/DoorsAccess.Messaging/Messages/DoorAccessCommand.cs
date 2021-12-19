using System;

namespace DoorsAccess.Messaging.Messages
{
    public class DoorAccessCommand : DoorMessage
    {
        public DoorAccessCommand(long doorId, long userId, DateTime timeStamp, DoorCommandType type): base(doorId, userId, timeStamp)
        {
            Type = type;
        }

        public DoorCommandType Type { get; }
    }
}
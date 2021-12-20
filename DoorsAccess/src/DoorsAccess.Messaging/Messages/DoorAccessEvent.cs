using System;
using DoorsAccess.Models.Enums;

namespace DoorsAccess.Messaging.Messages
{
    public class DoorAccessEvent : DoorMessage
    {
        public DoorAccessEvent(long doorId, long userId, DateTime timeStamp, DoorEventType type): base(doorId, userId, timeStamp)
        {
            Type = type;
        }

        public DoorEventType Type { get; }
    }
}
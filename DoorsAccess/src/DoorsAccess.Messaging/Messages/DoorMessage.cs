using System;

namespace DoorsAccess.Messaging.Messages
{
    public abstract class DoorMessage
    {
        protected DoorMessage(long doorId, long userId, DateTime timeStamp)
        {
            DoorId = doorId;
            UserId = userId;
            TimeStamp = timeStamp;
        }

        public long DoorId { get; }
        public long UserId { get; }
        public DateTime TimeStamp { get; }
    }
}
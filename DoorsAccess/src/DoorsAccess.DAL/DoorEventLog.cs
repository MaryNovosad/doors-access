﻿using System;

namespace DoorsAccess.DAL
{
    public class DoorEventLog
    {
        public long DoorId { get; set; }
        public long UserId { get; set; }
        public DateTime TimeStamp { get; set; }
        public DoorEvent Event { get; set; }
    }
}
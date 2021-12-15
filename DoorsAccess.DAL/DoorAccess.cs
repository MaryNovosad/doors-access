using System;

namespace DoorsAccess.DAL
{
    public class DoorAccess
    {
        public long DoorId { get; set; }
        public long UserId { get; set; }
       
        public bool IsOwner { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

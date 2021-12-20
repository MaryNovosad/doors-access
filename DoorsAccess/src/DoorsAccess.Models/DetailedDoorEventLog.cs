namespace DoorsAccess.Models
{
    public class DetailedDoorEventLog : DoorEventLog
    {
        public string? DoorName { get; set; }
        public bool IsDoorDeactivated { get; set; }
    }
}

namespace DoorsAccess.Domain.DTOs;

public class DoorInfo
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public bool IsDeactivated { get; set; }
}
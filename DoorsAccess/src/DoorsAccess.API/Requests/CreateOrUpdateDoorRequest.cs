using System.ComponentModel.DataAnnotations;

namespace DoorsAccess.API.Requests;

public class CreateOrUpdateDoorRequest
{
    [Required]
    public long DoorId { get; set; }

    [Required]
    [MaxLength(30)]
    public string DoorName { get; set; }

    public bool IsDeactivated { get; set; }
}
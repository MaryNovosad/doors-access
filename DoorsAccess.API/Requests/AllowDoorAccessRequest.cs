using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoorsAccess.API.Requests;

public class AllowDoorAccessRequest
{
    [Required]
    public ICollection<long> UsersIds { get; set; }

}
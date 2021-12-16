using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoorsAccess.API.Requests;

public class AllowDoorAccessRequest
{
    [Required]
    public IList<long> UsersIds { get; set; }

}
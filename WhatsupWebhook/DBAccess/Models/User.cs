using System;
using System.Collections.Generic;

namespace WhatsupWebhook.DBAccess.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string EmailId { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? RoleId { get; set; }

    public string? OrganisationName { get; set; }

    public string? Qualification { get; set; }

    public DateTime LastUpdatedTime { get; set; }

    public virtual Role? Role { get; set; }
}

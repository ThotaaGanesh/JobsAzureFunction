using System;
using System.Collections.Generic;

namespace WhatsupWebhook.DBAccess.Models;

public partial class Job
{
    public int Id { get; set; }

    public string JobName { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Qualification { get; set; } = null!;

    public string Salary { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string ContactPerson { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateTime LastUpdatedTime { get; set; }

    public int UserId { get; set; }
}

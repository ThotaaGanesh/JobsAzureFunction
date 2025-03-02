using System;
using System.Collections.Generic;

namespace WhatsupWebhook.DBAccess.Models;

public partial class Subscribe
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? SendTo { get; set; }

    public string? Qualification { get; set; }

    public string? SubscriptionType { get; set; }

    public bool IsActive { get; set; }
}

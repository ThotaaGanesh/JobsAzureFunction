using System;
using System.Collections.Generic;

namespace WhatsupWebhook.DBAccess.Models;

public partial class SendNotification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int JobId { get; set; }

    public string Type { get; set; } = null!;

    public bool IsNotificationSent { get; set; }

    // Navigation properties
    public Job Job { get; set; }
    public User User { get; set; }

}

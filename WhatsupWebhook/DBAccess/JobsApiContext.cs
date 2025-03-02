using Microsoft.EntityFrameworkCore;
using WhatsupWebhook.DBAccess.Models;

namespace WhatsupWebhook.DBAccess;

public partial class JobsApiContext : DbContext
{
    public JobsApiContext()
    {
    }

    public JobsApiContext(DbContextOptions<JobsApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Job> Job { get; set; }


    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<SendNotification> SendNotifications { get; set; }

    public virtual DbSet<Subscribe> Subscribe { get; set; }

    public virtual DbSet<User> User { get; set; }


}

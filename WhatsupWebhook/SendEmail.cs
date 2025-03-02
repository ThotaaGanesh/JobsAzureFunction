using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using WhatsupWebhook.DBAccess.Models;
using WhatsupWebhook.DBAccess;
using WhatsupWebhook.Notofications;
using System.Linq;

namespace WhatsupWebhook
{
    public static class SendEmail
    {
        [FunctionName("SendEmails")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing the SendEmails function started.");

            // Set up DbContext with dependency injection or ensure disposal
            var optionsBuilder = new DbContextOptionsBuilder<JobsApiContext>();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DefaultConnection"));

            using (var context = new JobsApiContext(optionsBuilder.Options))
            {
                // Retrieve notifications to be sent
                List<SendNotification> notifications = await context.SendNotifications
                    .Where(x => !x.IsNotificationSent && x.Type == "mail")
                    .Include(x => x.Job)
                    .Include(x => x.User)
                    .ToListAsync();

                // Send notifications
                if (notifications.Count > 0)
                {
                    SendNotifications sender = new SendNotifications(log);
                    sender.SendEmails(notifications);
                    foreach (var notification in notifications)
                    {
                        notification.IsNotificationSent = true;
                        context.Update(notification);
                    }
                    context.SaveChanges();
                }
                else
                {
                    log.LogInformation("No notifications to send.");
                }


                log.LogInformation("Processing the SendEmails function ended.");

                return new OkObjectResult("Operation Successfully finished");
            }
        }
    }
}

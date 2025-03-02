using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WhatsupWebhook.DBAccess.Models;

namespace WhatsupWebhook.Notofications
{
    internal class SendNotifications
    {
        public SendNotifications(ILogger log) => Logger = log;

        public ILogger Logger { get; }

        internal async Task SendWhatsupMessage(IEnumerable<SendNotification> sendNotifications)
        {
            var accessToken = Environment.GetEnvironmentVariable("accessToken");
            var apiUrl = Environment.GetEnvironmentVariable("whatsupapi");
            var templateName = Environment.GetEnvironmentVariable("templateName");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                foreach (var notification in sendNotifications)
                {
                    var payload = new
                    {
                        messaging_product = "whatsapp",
                        to = notification.User.PhoneNumber,
                        type = "template",
                        template = new
                        {
                            name = templateName,
                            language = new
                            {
                                code = "en_US"
                            },
                            components = new[]
                                {
                        new {
                            type = "body",
                            parameters = new[]
                            {
                                new { type = "text", text = "+919700619326" },
                                new { type = "text", text = "https://ganeshthota.com" }
                            }
                        }
                    }
                        }
                    };

                    var jsonPayload = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await client.PostAsync(apiUrl, content);
                        var result = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            Logger.LogInformation($"Job info {notification.Job.Description} sent successfully! to {notification.User.PhoneNumber}");
                            Logger.LogInformation(result);
                        }
                        else
                        {
                            Logger.LogInformation($"while sedning message failed to {notification.User.PhoneNumber}");
                            Logger.LogInformation(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogInformation($"An exception occurred: while sedning message {notification.User.PhoneNumber} to  {ex.Message}");
                    }
                }
            }
        }

        internal void SendEmails(IEnumerable<SendNotification> sendNotifications)
        {
            using SmtpClient smtpClient = new SmtpClient("smtp.hostinger.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential("connect@ganeshthota.com", "Ganeshthota@409");
            smtpClient.EnableSsl = true;

            using MailMessage mail = new MailMessage();
            mail.From = new MailAddress("connect@ganeshthota.com");
            mail.Subject = "Job Opputunity";

            mail.IsBodyHtml = true;

            try
            {
                foreach (var notification in sendNotifications)
                {
                    mail.Body = PrepareHtmlContent(notification.Job.JobName, notification.Job.Location, notification.Job.Description, "https://ganeshthota.com");
                    mail.To.Add(new MailAddress(notification.User.EmailId));
                    smtpClient.Send(mail);
                    Logger.LogInformation("Emails sent successfully!");
                }


            }
            catch (Exception ex)
            {
                Logger.LogInformation("Could not send the email. Error : " + ex.Message);
            }
        }

        private string PrepareHtmlContent(string title, string location, string description, string url)
        {
            string htmlTemplate = $@"
        <!DOCTYPE html>
        <html lang=""en"">
        <head>
        <meta charset=""UTF-8"">
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>New Job Alert</title>
        <style>
            body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; }}
            .container {{ padding: 20px; }}
            .header {{ background-color: #f2f2f2; padding: 10px 20px; text-align: center; }}
            .content {{ margin-top: 20px; }}
            .footer {{ background-color: #f2f2f2; padding: 10px 20px; text-align: center; margin-top: 20px; }}
        </style>
        </head>
        <body>
        <div class='container'>
            <div class='header'>
                <h2>New Job Opportunity</h2>
            </div>
            <div class='content'>
                <h3>Dear Subscriber,</h3>
                <p>We are excited to announce a new job opportunity that may interest you:</p>
                <p><strong>Job Title: </strong>{title}</p>
                <p><strong>Location: </strong>{location}</p>
                <p><strong>Description: </strong>{description}</p>
                <p>For more details and to apply, please click on the link below:</p>
                <p><a href='{url}' target='_blank'>View Job and Apply</a></p>
            </div>
            <div class='footer'>
                <p>Thank you for being a valued member of our community!</p>
                <p>Regards,<br>Jobs Portal Team</p>
            </div>
        </div>
        </body>
        </html>";

            return htmlTemplate;
        }
    }
}

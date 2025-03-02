using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace WhatsupWebhook
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
       [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
       ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (req.Method == HttpMethods.Get)
            {
                string mode = req.Query["hub.mode"];
                string token = req.Query["hub.verify_token"];
                string challenge = req.Query["hub.challenge"];

                // Set your verify token here (must match the token provided in the Meta webhook setup)
                const string verifyToken = "webhhok";

                if (mode == "subscribe" && token == verifyToken)
                {
                    log.LogInformation("Webhook verification successful.");
                    return new OkObjectResult(challenge); // Responds back with the received 'challenge' query param
                }
                else
                {
                    log.LogInformation("Webhook verification failed. Invalid token.");
                    return new ForbidResult(); // Forbidden result if tokens don't match
                }
            }

            if (req.Method.ToUpperInvariant() == "POST")
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                log.LogInformation($"Webhook received: {requestBody}");

                if (data != null)
                {
                    // Assuming the structure based on standard webhook payload, adapt as necessary
                    string eventObject = (string)data["object"];
                    if (eventObject == "whatsapp_business_account")
                    {
                        JArray statuses = (JArray)data["entry"][0]["changes"][0]["value"]["statuses"];
                        if (statuses != null)
                        {
                            foreach (var status in statuses)
                            {
                                string messageId = (string)status["id"];
                                string messageStatus = (string)status["status"];
                                string recipientId = (string)status["recipient_id"];

                                log.LogInformation($"Message ID: {messageId}");
                                log.LogInformation($"Recipient ID: {recipientId}");
                                log.LogInformation($"Status: {messageStatus}");

                                // Implement further logic based on status values

                            }
                        }
                        else
                        {
                            log.LogInformation("Statuses array is null.");
                        }

                        return new OkObjectResult("Webhook received and processed");
                    }
                    else
                    {
                        return new BadRequestObjectResult("Error: missing or incorrect data");
                    }
                }
                else
                {
                    // Handle GET or other methods if necessary, or return not supported
                    return new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);
                }
            }

            return new OkObjectResult("Webhook received and processed");
        }
    }
}

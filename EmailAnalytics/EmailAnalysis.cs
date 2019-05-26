using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EmailAnalytics
{
    public static class EmailAnalysis
    {
        [FunctionName("Email")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,"post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Email emailData = JsonConvert.DeserializeObject<Email>(requestBody);

            if (emailData == null)
                return (ActionResult)new BadRequestObjectResult("Please pass the email data");

            return (ActionResult)new OkResult();
        }

        public class Email
        {
            public string From { get; set; }
            public string To { get; set; }
            public Data data { get; set; }
        }

        public class Data
        {
            public string Subject { get; set; }
            public string CC { get; set; }
            public string BCC { get; set; }
            public DateTime DateSent { get; set; }
            public int NumOfAttachments { get; set; }
            public int WordCount { get; set; }
        }
    }
}

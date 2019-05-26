using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;

namespace EmailAnalytics
{
    public static class EmailAnalysis
    {
        [FunctionName("Email")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,"post", Route = null)] HttpRequest req,
            [Table("Email")]CloudTable tableout,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Email emailData = JsonConvert.DeserializeObject<Email>(requestBody);

            if (emailData == null)
                return (ActionResult)new BadRequestObjectResult("Please pass the email data");

            emailData.PartitionKey = emailData.From;
            emailData.RowKey = emailData.To + "_" + emailData.DateSent;

            try
            {
                var operation = TableOperation.Insert(emailData);
                await tableout.ExecuteAsync(operation);
            }
            catch(Exception ex)
            {
                if(ex.Message.ToLower() == "conflict")
                    return (ActionResult)new BadRequestObjectResult("Conflict - identical data already exists");
            }

            return (ActionResult)new OkResult();
        }

        public class Email : TableEntity
        {
            public string From { get; set; }
            public string To { get; set; }
            public string Subject { get; set; }
            public string CC { get; set; }
            public string BCC { get; set; }
            public DateTime DateSent { get; set; }
            public int NumOfAttachments { get; set; }
            public int WordCount { get; set; }
        }
    }
}

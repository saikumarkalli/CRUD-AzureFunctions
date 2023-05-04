using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Pratice_AF_Core
{
    public class TimerTrigger
    {
        [FunctionName("TimerTrigger")]
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer)
        {
            //outPut = "TimerTrigger";
           // log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}

using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.Payroll
{
    [RoutePrefix("api/background")]
    public class BackGroundCheckController : ApiController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        // api/background/test
        [Route("test")]
        [HttpPost]
        public IHttpActionResult Execute()
        {
            logger.Debug("Doing something.");

            //HostingEnvironment.QueueBackgroundWorkItem(ct => FireAndForgetMethodAsync());
            HostingEnvironment.QueueBackgroundWorkItem(ct => FireAndForgetMethod());
            // HostingEnvironment.QueueBackgroundWorkItem(ct => FaultyFireAndForgetMethod());

            logger.Debug("I don't care for that stuff running in the background. I'm out.");

            return Ok("It still returns, regardless of exceptions.");
        }

        private async Task FireAndForgetMethodAsync()
        {
            logger.Debug("Started running an asynchronous background work item at {0}...", DateTime.Now.ToString("mm:ss:FFFFFFF"));

            int i = 1;
            while (i != 11)
            {
                logger.Debug("Value : {0}", i);
                i++;
                await Task.Delay(2000); // Pretend we are doing something for 2s
            }

            logger.Debug("Finished running that.");
        }

        private void FireAndForgetMethod()
        {
            logger.Debug("Started running a background work item at {0}...", DateTime.Now.ToString("mm:ss:FFFFFFF"));
            int i = 1;
            while (i != 11)
            {
                logger.Debug("Value : {0}", i);
                i++;
                Thread.Sleep(2000); // Pretend we are doing something for 2s
            }
            // One Second = 1 * 1000;

            logger.Debug("Finished running that.");
        }

        private void FaultyFireAndForgetMethod()
        {
            throw new Exception("I'm failing just to make a point.");
        }
    }
}
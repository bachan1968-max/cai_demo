using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace cai.Service.ControllerService
{
    public class ControllerService : IControllerService
    {
        public Task RunTask(string taskId, CancellationToken ct = default)
        {
            RecurringJob.TriggerJob(taskId);
            return Task.CompletedTask;
        }
    }
}

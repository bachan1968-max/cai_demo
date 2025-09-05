using System.Threading.Tasks;
using Hangfire.Server;
using cai.Domain;

namespace cai.Service.HangfireTasks
{
    public interface ITaskInterface
    {
        Task DoJobAsync(TasksHfEnumeration task, PerformContext context);
    }
}

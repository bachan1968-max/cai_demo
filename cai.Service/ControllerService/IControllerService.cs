using System.Threading;
using System.Threading.Tasks;

namespace cai.Service.ControllerService
{
    public interface IControllerService
    {
        public Task RunTask(string taskId, CancellationToken ct = default);
    }
}

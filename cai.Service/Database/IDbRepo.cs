using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace cai.Service.Database
{
    public interface IDbRepo
    {
        Task<bool> AddDeliveredData(PriceList pl, List<PriceListRow> plRows, CancellationToken ct);
    }
}

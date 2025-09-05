using System.Collections.Generic;
using System.Threading.Tasks;
using cai.Domain;
using System.Threading;

namespace cai.Service.B2bInteraction
{
    public interface IB2bRepository  
    {
        public Task<List<WareItem>> GetFullStock(string user, string password, CancellationToken ct = default);
    }
}

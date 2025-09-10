using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace cai.Service.Database
{
    public class DbRepo : IDbRepo
    {
        private readonly CaiDbContext _dbContext;
        ILogger<DbRepo> _logger;
        public DbRepo(CaiDbContext dbContext, ILogger<DbRepo> logger, CancellationToken ct = default)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> AddDeliveredData(PriceList pl, List<PriceListRow> plRows, CancellationToken ct)
        {
            if (pl == null) throw new ArgumentNullException(nameof(pl));
            if (plRows == null) throw new ArgumentNullException(nameof(plRows));

            try
            {
                await _dbContext.PriceLists.AddAsync(pl, ct).ConfigureAwait(false);
                await _dbContext.PriceListRows.AddRangeAsync(plRows, ct).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync(ct);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception");
                return false;
            }
        }
    }
}

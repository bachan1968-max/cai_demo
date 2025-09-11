using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using cai.Domain;
using System.Linq;
using cai.Service.HttpClients;

namespace cai.Service.B2bInteraction
{
	public class B2bRepository : IB2bRepository
	{
		public readonly ILogger<B2bRepository> _logger;
		private readonly B2bHttpClient _b2bHttpClient;

		public B2bRepository(B2bHttpClient b2bHttpClient, ILogger<B2bRepository> logger)
		{
			_b2bHttpClient = b2bHttpClient ?? throw new ArgumentNullException(nameof(b2bHttpClient));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<List<WareItem>> GetFullStock(string user, string password, CancellationToken ct = default)
		{
			try
			{
				var response = await _b2bHttpClient.GetFullStock(user, password, ct);
				if (response.IsSuccessStatusCode)
				{
					var contents = await response.Content.ReadAsStringAsync(ct);
					var data = JsonConvert.DeserializeObject<B2bResponse>(contents);
					if (data.Header.Code != 0)
					{
						_logger.LogError("Remote server error: {Message}", data.Header.Message);
						return null;
					}
					return data.Body.CategoryItem.ToList();
				}
				else
				{
					_logger.LogError("Failed get data, status code: {StatusCode}", response.StatusCode);
					return null;
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Exeption: ");
				return null;
			}

		}
	}
}
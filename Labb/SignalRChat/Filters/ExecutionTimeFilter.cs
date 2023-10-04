using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace SignalRChat.Filters
{
	public class ExecutionTimeFilter : IHubFilter
	{
		private readonly ILogger _logger;

		public ExecutionTimeFilter(ILogger<ExecutionTimeFilter> logger)
		{
			_logger = logger;
		}

		// Detta är metoden som faktiskt utför exekveringstidsmätningen
		public async ValueTask<object> InvokenMethodAsync(HubInvocationContext invocationContext,
			Func<HubInvocationContext, ValueTask<object>> next)
		{
			// Skapar en stopwatch för att mäta tiden det tar att exekvera hubmetoden
			var stopwatch = Stopwatch.StartNew();

			try
			{
				// Kör nästa filter eller hubmetod i kedjan
				return await next(invocationContext);
			}
			finally
			{
				// Stoppa tidtagningen när hubmetoden har slutförts
				stopwatch.Stop();

				// Hämta den totala tid som tagits
				var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

				// Hämta namnet på hubmetoden som exekverades
				var methodName = invocationContext.HubMethodName;

				// Logga information om exekveringstiden
				_logger.LogInformation($"Hub metod {methodName} took {elapsedMilliseconds} ms to execute.");
			}
		}
	}
}

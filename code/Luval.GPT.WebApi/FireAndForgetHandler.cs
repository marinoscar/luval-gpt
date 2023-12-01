using Microsoft.Extensions.DependencyInjection;

namespace Luval.GPT.WebApi
{
    public class FireAndForgetHandler
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;

        public FireAndForgetHandler(ILogger logger, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void Execute<TService>(Func<TService, Task> doWork) where TService : class
        {
            Task.Run(async () =>
            {
                // Exceptions must be caught
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var item = scope.ServiceProvider.GetRequiredService<TService>();
                    await doWork(item);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to complete operation");
                }
            });
        }



    }
}

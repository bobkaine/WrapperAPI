using WrapperAPI.Models.Configuration;
using Microsoft.Extensions.Options;

namespace WrapperAPI.Classes
{
    internal class ValidateOptionsService<TConfiguration> : IHostedService where TConfiguration : class, new()
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IOptions<SiteConfiguration> _settings;
        private readonly ILogger<ValidateOptionsService<TConfiguration>> _logger;

        public ValidateOptionsService(
            IHostApplicationLifetime hostApplicationLifetime,
            IOptions<SiteConfiguration> settings,
            ILogger<ValidateOptionsService<TConfiguration>> logger
            )
        {
            _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _ = _settings.Value; // Accessing this triggers validation
            }
            catch (OptionsValidationException ex)
            {
                _logger.LogError("One or more options validation checks failed");

                foreach (var failure in ex.Failures)
                {
                    _logger.LogError(failure);
                }

                _hostApplicationLifetime.StopApplication(); // Stop the app now
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask; // Nothing to do
        }
    }
}
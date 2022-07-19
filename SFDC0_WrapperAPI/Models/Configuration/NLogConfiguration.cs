using Microsoft.Extensions.Options;

namespace WrapperAPI.Models.Configuration
{
#pragma warning disable S1118 // Utility classes should not have public constructors
    internal class NLogConfiguration
#pragma warning restore S1118 // Utility classes should not have public constructors
    {
        internal static Task UpdateNetworkAddressAsync(IServiceProvider serviceProvider, string v)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }

    internal class NLogConfigurationValidator : IValidateOptions<NLogConfiguration>
    {
        public ValidateOptionsResult Validate(string name, NLogConfiguration options)
        {
            //throw new NotImplementedException();
            return ValidateOptionsResult.Success;
        }
    }
}
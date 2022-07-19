using Microsoft.Extensions.Options;

namespace WrapperAPI.Models.Configuration
{
    internal class CachingConfiguration
    {
    }

    internal class CachingConfigurationValidator : IValidateOptions<CachingConfiguration>
    {
        public ValidateOptionsResult Validate(string name, CachingConfiguration options)
        {
            //throw new NotImplementedException();
            return ValidateOptionsResult.Success;
        }
    }
}
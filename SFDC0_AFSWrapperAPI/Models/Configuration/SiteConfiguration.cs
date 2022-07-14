using Microsoft.Extensions.Options;

namespace WrapperAPI.Models.Configuration
{
    internal class SiteConfiguration
    {
    }

    internal class SiteConfigurationValidator : IValidateOptions<SiteConfiguration>
    {
        public ValidateOptionsResult Validate(string name, SiteConfiguration options)
        {
            //throw new NotImplementedException();
            return ValidateOptionsResult.Success;
        }
    }
}
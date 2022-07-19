using FluentValidation;
using Microsoft.Extensions.Options;

namespace WrapperAPI.Models.Configuration
{
    internal class HealthChecksCommonConfiguration
    {
    }

    internal class HealthChecksCommonConfigurationValidator : IValidateOptions<HealthChecksCommonConfiguration>
    {
        public ValidateOptionsResult Validate(string name, HealthChecksCommonConfiguration options)
        {
            //throw new NotImplementedException();
            return ValidateOptionsResult.Success;
        }
    }

}
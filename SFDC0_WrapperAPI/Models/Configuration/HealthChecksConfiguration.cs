using FluentValidation;
using Microsoft.Extensions.Options;

namespace WrapperAPI.Models.Configuration
{
    public class HealthChecksConfiguration
    {
        public string AfsEndpointUri { get; set; } = "";

    }

    public class HealthChecksConfigurationValidator : IValidateOptions<HealthChecksConfiguration>
    {
        public ValidateOptionsResult Validate(string name, HealthChecksConfiguration options)
        {
            //throw new NotImplementedException();
            return ValidateOptionsResult.Success;
        }
    }
}

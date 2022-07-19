using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WrapperAPI.Factories
{
    internal class HealthCheckResponseFactory
    {
        internal static Task WriteHealthCheckResponse(HttpContext arg1, HealthReport arg2)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
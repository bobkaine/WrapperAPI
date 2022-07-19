using Microsoft.AspNetCore.Mvc.Filters;

namespace WrapperAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class UncaughtExceptionHandlerAttribute : ActionFilterAttribute
    {
        //TODO: Uncaught Exception Handler Code Goes Here
    }
}

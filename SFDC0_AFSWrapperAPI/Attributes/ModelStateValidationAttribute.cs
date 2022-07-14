using Microsoft.AspNetCore.Mvc.Filters;

namespace WrapperAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ModelStateValidationAttribute : ActionFilterAttribute
    {
        //TODO: Model State Validation Code Goes Here
    }
}

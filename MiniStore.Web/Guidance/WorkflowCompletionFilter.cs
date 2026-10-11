using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MiniStore.Web.Guidance;

public sealed class WorkflowCompletionFilter(ITempDataDictionaryFactory tempDataFactory) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executed = await next();
        var controller = context.RouteData.Values["controller"]?.ToString();

        if (!HttpMethods.IsPost(context.HttpContext.Request.Method) ||
            !WorkflowNextStepGuidance.IsSupportedController(controller) ||
            executed.Exception is not null ||
            executed.Result is not RedirectResult and not RedirectToActionResult and not RedirectToRouteResult)
            return;

        var tempData = tempDataFactory.GetTempData(context.HttpContext);
        if (tempData.ContainsKey("NotificationMessage") || tempData.ContainsKey("Success")) return;

        tempData["NotificationType"] = "success";
        tempData["NotificationMessage"] = "Operation completed successfully.";
    }
}

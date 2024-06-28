using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Extensions
{
    public static class ValidationExtensions
    {
        public static ActionResult ValidationResponse(this ControllerBase controller)
        {
            var modelState = controller.ModelState;

            var errors = modelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return new BadRequestObjectResult(new { errors });
        }
    }
}

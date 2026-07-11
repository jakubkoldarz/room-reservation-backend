using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace RoomReservation.Api.Binders
{
    public class UserIdBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var claim = bindingContext.ActionContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if(claim is null || !Guid.TryParse(claim.Value, out var userId))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(userId);
            return Task.CompletedTask;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Parapharmacie.Models;

namespace Parapharmacie.Filters
{
    /// <summary>
    /// Minimal role-based authorization using session (demo only).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RoleAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly Role[] _roles;

        public RoleAuthorizeAttribute(params Role[] roles)
        {
            _roles = roles ?? Array.Empty<Role>();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var roleString = context.HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(roleString) || !Enum.TryParse<Role>(roleString, out var currentRole))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (_roles.Length > 0 && !_roles.Contains(currentRole))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}





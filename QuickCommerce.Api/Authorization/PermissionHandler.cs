using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Api.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // User must be authenticated
            if (context.User.Identity?.IsAuthenticated != true)
            {
                Debug.WriteLine("❌ User is NOT authenticated.");
                return Task.CompletedTask;
            }

            Debug.WriteLine("");
            Debug.WriteLine("==============================================");
            Debug.WriteLine("        PERMISSION HANDLER");
            Debug.WriteLine("==============================================");

            Debug.WriteLine($"Required Permission : {requirement.Permission}");

            Debug.WriteLine("");
            Debug.WriteLine("----- ALL JWT CLAIMS -----");

            foreach (var claim in context.User.Claims)
            {
                Debug.WriteLine($"{claim.Type} = {claim.Value}");
            }

            Debug.WriteLine("--------------------------");

            // Read permission claims from JWT
            var permissions = context.User.Claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();

            Debug.WriteLine("");
            Debug.WriteLine($"Permission Claims Found : {permissions.Count}");

            foreach (var permission in permissions)
            {
                Debug.WriteLine($"Permission => {permission}");
            }

            if (permissions.Contains(requirement.Permission))
            {
                Debug.WriteLine("");
                Debug.WriteLine("✅ AUTHORIZED");
                context.Succeed(requirement);
            }
            else
            {
                Debug.WriteLine("");
                Debug.WriteLine("❌ NOT AUTHORIZED");
            }

            Debug.WriteLine("==============================================");
            Debug.WriteLine("");

            return Task.CompletedTask;
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using RoomReservation.Core.Authorization.Requirements;
using System.Collections.Concurrent;

namespace RoomReservation.Api.Authorization
{
    public class CustomPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallback;
        private readonly AuthorizationPolicy _profileCompletedPolicy;
        private readonly ConcurrentDictionary<string, AuthorizationPolicy> _permissionPolicies = [];

        public CustomPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallback = new DefaultAuthorizationPolicyProvider(options);

            _profileCompletedPolicy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new ProfileCompletedRequirement())
                    .Build();
        }
        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.Equals("ProfileCompleted", StringComparison.Ordinal))
            {
                return Task.FromResult<AuthorizationPolicy?>(_profileCompletedPolicy);
            }

            if (policyName.StartsWith("Permission:"))
            {
                var permission = policyName.Split(":")[1];

                var policy = _permissionPolicies.GetOrAdd(permission, p =>
                    new AuthorizationPolicyBuilder()
                        .AddRequirements(new PermissionRequirement(p))
                        .Build());

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return _fallback.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();
        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();
    }
}

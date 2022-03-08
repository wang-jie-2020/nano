using System.Security.Claims;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Services;
using IdentityServer4.Test;
using IdentityServer4.Validation;

namespace NanoService.IdentityServer.Extend
{
    public class ExtendClaimService : DefaultClaimsService, IClaimsService
    {
        public ExtendClaimService(IProfileService profile, ILogger<DefaultClaimsService> logger)
            : base(profile, logger)
        {
        }
    }
}

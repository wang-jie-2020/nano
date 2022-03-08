using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Identity;

namespace NanoService.IdentityServer.Extend
{
    public class ExtendProfileService : DefaultProfileService, IProfileService
    {
        public ExtendProfileService(ILogger<DefaultProfileService> logger) : base(logger)
        {
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //from base
            context.LogProfileRequest(this.Logger);
            context.AddRequestedClaims(context.Subject.Claims);
            context.LogIssuedClaims(this.Logger);
            return Task.CompletedTask;
        }

        public override Task IsActiveAsync(IsActiveContext context)
        {
            //from base
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}

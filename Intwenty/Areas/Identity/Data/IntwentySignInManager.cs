using Intwenty.Areas.Identity.Entity;
using Intwenty.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Areas.Identity.Data
{
    /// <summary>
    /// NOT IN USE
    /// </summary>
    public class IntwentySignInManager : SignInManager<IntwentyUser>
    {

        private IIntwentyOrganizationManager OrganizationManager { get; }

        private IntwentySettings Settings { get; }

        public IntwentySignInManager(
            IntwentyUserManager userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<IntwentyUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<IntwentySignInManager> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<IntwentyUser> confirmation,
            IIntwentyOrganizationManager orgmanager,
            IOptions<IntwentySettings> settings)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            OrganizationManager = orgmanager;
            Settings = settings.Value;
        }

        /*
        public override async Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
        {
            var user = await UserManager.FindByLoginAsync(loginProvider, providerKey);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            var error = await PreSignInCheck(user);
            if (error != null)
            {
                return error;
            }
            return await SignInOrTwoFactorAsync(user, isPersistent, loginProvider, bypassTwoFactor);
        }
        */

        public override async Task<SignInResult> PasswordSignInAsync(IntwentyUser user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            if (!await OrganizationManager.IsProductUser(Settings.ProductId, user))
                return SignInResult.NotAllowed;

            return await base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }

        public override async Task<bool> CanSignInAsync(IntwentyUser user)
        {
            if (!await OrganizationManager.IsProductUser(Settings.ProductId, user))
                return false;

            return await base.CanSignInAsync(user);
        }

        public override async Task<SignInResult> CheckPasswordSignInAsync(IntwentyUser user, string password, bool lockoutOnFailure)
        {
            if (!await OrganizationManager.IsProductUser(Settings.ProductId, user))
                return SignInResult.NotAllowed;

            return await base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
        }

        public override async Task<ClaimsPrincipal> CreateUserPrincipalAsync(IntwentyUser user)
        {
            var t = await base.CreateUserPrincipalAsync(user);
            var identity = (ClaimsIdentity)t.Identity;
            identity.AddClaim(new Claim("ProductId", Settings.ProductId));
            return t;
        }

        public override bool IsSignedIn(ClaimsPrincipal principal)
        {
            return base.IsSignedIn(principal);
        }

     

      
    }
}

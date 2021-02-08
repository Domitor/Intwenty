using Intwenty.Areas.Identity.Entity;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Areas.Identity.Data
{
    public class IntwentyClaimsPricipalFactory : UserClaimsPrincipalFactory<IntwentyUser, IntwentyProductAuthorizationItem>
    {
        private IntwentySettings Settings { get; }

        private IIntwentyOrganizationManager OrganizationManager { get; }

        public IntwentyClaimsPricipalFactory(IntwentyUserManager userManager, 
                                            RoleManager<IntwentyProductAuthorizationItem> roleManager,
                                            IOptions<IdentityOptions> optionsAccessor, 
                                            IOptions<IntwentySettings> settings,
                                            IIntwentyOrganizationManager orgmanager) : base(userManager, roleManager, optionsAccessor)
        {
            Settings = settings.Value;
            OrganizationManager = orgmanager;
        }

        public async override Task<ClaimsPrincipal> CreateAsync(IntwentyUser user)
        {
            var principal = await base.CreateAsync(user);

            //Get all organizations that this user is member of and where this product is available, should be one
            var orgproducts = await OrganizationManager.GetUserOrganizationProductsInfoAsync(user.Id, Settings.ProductId);

            if (!string.IsNullOrWhiteSpace(user.FirstName))
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] { new Claim("FirstName", user.FirstName)});
            

            if (!string.IsNullOrWhiteSpace(user.LastName))
               ((ClaimsIdentity)principal.Identity).AddClaims(new[] { new Claim("LastName", user.LastName)});

            ((ClaimsIdentity)principal.Identity).AddClaims(new[] { new Claim("UserTablePrefix", user.TablePrefix)});

            if (orgproducts.Count > 0)
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] { new Claim("OrganizationId", Convert.ToString(orgproducts[0].OrganizationId)) });
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] { new Claim("OrganizationName", orgproducts[0].OrganizationName) });
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] { new Claim("OrganizationTablePrefix", orgproducts[0].OrganizationTablePrefix) });

            }
            

            return principal;
        }

    }
}

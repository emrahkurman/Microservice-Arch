﻿using ShoesOnContainers.Web.WebMvc.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace ShoesOnContainers.Web.WebMvc.Services
{
    public class IdentityService : IIdentityService<ApplicationUser>
    {
        public ApplicationUser Get(IPrincipal principal)
        {
            if (principal is ClaimsPrincipal claims)
            {
                var user = new ApplicationUser()
                {
                    Email = claims.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value ?? "",
                    Id = claims.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? "",
                };

                return new ApplicationUser
                {
                    Email = claims.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value ?? "",
                    Id = claims.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? "",
                };
            }

            throw new ArgumentException(message: "The principal must be a ClaimsPrincipal", paramName: nameof(principal));
        }
    }
}
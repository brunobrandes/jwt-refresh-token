
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Jwt.Refresh.Token.Domain.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Jwt.Refresh.Token.Domain.Services
{
	public class ClaimsIdentityService : IClaimsIdentityService
    {
        private readonly IServiceProvider _serviceProvicer;

        public ClaimsIdentityService(IServiceProvider serviceProvicer)
        {
            _serviceProvicer = serviceProvicer;
        }

        public async Task<ClaimsIdentity> GetAsync(string userId, CancellationToken cancellationToken = default)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };

            var claimsService = _serviceProvicer.GetService<IClaimsService>();

            if (claimsService != null)
            {
                var customClaims =  await claimsService.GetAsync(userId, cancellationToken);
                customClaims.RemoveAll(x => x.Type == ClaimTypes.NameIdentifier);

                claims.AddRange(customClaims);
            }

            return new ClaimsIdentity(claims);
        }
    }
}


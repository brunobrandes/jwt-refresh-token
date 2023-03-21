using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Jwt.Refresh.Token.Domain.Services.Interfaces
{
	public interface IClaimsService
	{
		Task<List<Claim>> GetAsync(string userId, CancellationToken cancellationToken);
	}
}


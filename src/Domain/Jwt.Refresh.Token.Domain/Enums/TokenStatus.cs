using System;
namespace Jwt.Refresh.Token.Domain.Enums
{
    public enum TokenStatus
    {
        None = 0,
        Authorized = 200,
        Unauthorized = 401,
        Forbidden = 403,
        Error = 500
    }
}


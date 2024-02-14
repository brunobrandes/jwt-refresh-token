using System;
using System.Text.Json;
using System.Threading.Tasks;
using Jwt.Refresh.Token.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jwt.Refresh.Token.Infra.Http
{
	public class TokenResult : ActionResult
    {
        private readonly Domain.DataTransferObjects.Token _token;

        public TokenResult(Domain.DataTransferObjects.Token token)
        {
            _token = token;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)_token.Status;
            context.HttpContext.Response.ContentType = "application/json";

            if(_token.Status == TokenStatus.Authorized)
                await context.HttpContext.Response.Body.WriteAsync(JsonSerializer.SerializeToUtf8Bytes<Domain.DataTransferObjects.Token>(_token));
        }
    }
}
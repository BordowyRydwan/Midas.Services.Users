using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Middlewares;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IList<string> _omittedUrls = new List<string>
    {
        "/api/User/Register", "/api/User/Email"
    };
    
    public AuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserService authorizationService)
    {
        var isAuthorized = await CheckAuthorization(context, authorizationService).ConfigureAwait(false);
        var isOnOmittedUrlList = _omittedUrls.Any(x => context.Request.Path.StartsWithSegments(x));

        if (isAuthorized || isOnOmittedUrlList)
        {
            try { await _next(context).ConfigureAwait(false); }
            catch (Exception ex) { await HandleExceptionAsync(context, ex).ConfigureAwait(false); }
        }
        else
        {
            var ex = new Exception("Could not authorize user");
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
    }

    public async Task<bool> CheckAuthorization(HttpContext context, IUserService authorizationService)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrWhiteSpace(authHeader))
        {
            return false;
        }

        var token = authHeader.Replace("Bearer ", "");
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var email = jwtToken.Claims.First(x => x.Type == ClaimTypes.Email)?.Value;
        var user = await authorizationService.GetUserByEmail(email).ConfigureAwait(false);

        return user is not null;
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        
        await context.Response.WriteAsync(ex.Message + "\n\n" + ex.StackTrace).ConfigureAwait(false);
    }
}
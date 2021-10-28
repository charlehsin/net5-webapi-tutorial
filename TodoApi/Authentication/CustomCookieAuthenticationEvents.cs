using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using TodoApi.Authentication;

public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
{
    private readonly IMyClaim _myClaim;
    private readonly ILogger _logger;
    
    public CustomCookieAuthenticationEvents(IMyClaim myClaim, 
        ILogger<CustomCookieAuthenticationEvents> logger)
    {
        _myClaim = myClaim;
        _logger = logger;
    }

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var userPrincipal = context.Principal;
        var username = _myClaim.ParseAuthClaim(userPrincipal);

        // TODO: Validate the user agin in DB.
        var isValid = true;

        if (!isValid)
        {
            _logger.Log(LogLevel.Information, $"User {username} is not valid anymore. Signing out.");

            context.RejectPrincipal();

            await context.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }

        _logger.Log(LogLevel.Debug, $"User {username} is valid.");
    }
}
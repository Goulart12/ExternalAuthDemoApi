using System.Security.Claims;
using System.Text;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoogleAuthDemoApi;

[AllowAnonymous, Route("api/authorization/")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    
    [HttpGet]
    [Route("google-login")]
    public async Task GoogleLogin()
    {
       await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, 
           new AuthenticationProperties
           {
               RedirectUri = Url.Action("GoogleResponse")
           });
    }
    
    [HttpGet]
    [Route("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if(result.Principal == null) return BadRequest();
        
        var claims = result.Principal?.Identities
            .FirstOrDefault()?.Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });
        
        return Ok(claims);
    }
    
    [HttpGet]
    [Route("facebook-login")]
    public IActionResult FacebookLogin()
    {
        var properties = new AuthenticationProperties { RedirectUri = Url.Action("FacebookResponse") };
        return Challenge(properties, FacebookDefaults.AuthenticationScheme);
    }

    [HttpGet]
    [Route("facebook-response")]
    public async Task<IActionResult> FacebookResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if(result.Principal == null) return BadRequest();

        var claims = result.Principal?.Identities
            .FirstOrDefault()?.Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

        return Ok(claims);
    }
}
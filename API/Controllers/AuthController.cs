using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly TokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            TokenService tokenService,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet("login")]
        public IActionResult ExternalLogin(string provider = "Google", string returnUrl = null)
        {
            _logger.LogInformation($"Starting external login for provider: {provider}");

            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback(string returnUrl = null, string remoteError = null)
        {
            _logger.LogInformation("Google callback received");
            return await HandleExternalLogin(returnUrl, remoteError);
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            _logger.LogInformation("External login callback received");
            return await HandleExternalLogin(returnUrl, remoteError);
        }
        private async Task<IActionResult> HandleExternalLogin(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                _logger.LogError($"Error from external provider: {remoteError}");
                return BadRequest($"Error from external provider: {remoteError}");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogError("Error loading external login information.");
                return BadRequest("Error loading external login information.");
            }

            _logger.LogInformation($"External login info received for: {info.Principal.FindFirstValue(ClaimTypes.Email)}");

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User successfully signed in with external provider");
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                var token = await _tokenService.CreateToken(user);

                return Ok(new { token });
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    _logger.LogInformation($"Creating new user for email: {email}");
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
                        LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
                        EmailConfirmed = true
                    };

                    var createResult = await _userManager.CreateAsync(user);

                    if (createResult.Succeeded)
                    {
                        if (!await _roleManager.RoleExistsAsync("User"))
                        {
                            await _roleManager.CreateAsync(new ApplicationRole { Name = "User", Description = "Standard user role" });
                        }

                        await _userManager.AddToRoleAsync(user, "User");

                        await _userManager.AddLoginAsync(user, info);

                        await _signInManager.SignInAsync(user, isPersistent: false);

                        var token = await _tokenService.CreateToken(user);
                        _logger.LogInformation("New user created and signed in");

                        return Ok(new { token });
                    }

                    _logger.LogError("Error creating user");
                    return BadRequest("Error creating user.");
                }

                _logger.LogInformation($"Adding external login to existing user: {email}");
                var addLoginResult = await _userManager.AddLoginAsync(user, info);

                if (addLoginResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    var token = await _tokenService.CreateToken(user);
                    _logger.LogInformation("External login added to existing user");

                    return Ok(new { token });
                }
            }

            _logger.LogError("Error signing in with external provider");
            return BadRequest("Error signing in with external provider.");
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles
            });
        }
    }
}
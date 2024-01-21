using AuthCenter.Models.Dtos.Authentization;
using AuthCenter.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AuthCenter.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
    public class AuthController : ControllerBase
    {
        private IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAuthService _authService;
        public AuthController(IConfiguration config, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IAuthService authService)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }

        //[HttpPost(nameof(GenerateToken), Name = nameof(GenerateToken))]
        //public async Task<IActionResult> GenerateToken([FromBody] AuthRequest input)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var authResult = await _authService.GenerateTokenByUser(input);
        //        if (authResult.IsSuccess)
        //            return Ok(authResult);
        //        else
        //        {
        //            ModelState.AddModelError("Error", authResult.Message);
        //            return BadRequest(ModelState);
        //        }

        //    }
        //    return BadRequest(ModelState);

        //}


        //[HttpPost(nameof(GenerateTokenByRole), Name = nameof(GenerateTokenByRole))]
        //public async Task<IActionResult> GenerateTokenByRole(string roleId)
        //{
        //    var authResult = await _authService.GenerateTokenByRole(roleId);
        //    if (authResult.IsSuccess)
        //        return Ok(authResult);
        //    else
        //    {
        //        ModelState.AddModelError("Error", authResult.Message);
        //        return BadRequest(ModelState);
        //    }
        //}

        [HttpGet(nameof(GetCurrentClaims))]
        public IActionResult GetCurrentClaims()
        {
            var claims = User.Claims.ToList();
            var sb = new StringBuilder();
            claims.ForEach(t =>
            {
                sb.AppendLine($"{t.Type}={t.Value}");
            });
            return Ok(sb.ToString());
        }
    }
}

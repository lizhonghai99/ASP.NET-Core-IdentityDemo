using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User.Management.API.Models;
using User.Management.Data.Models;
using User.Management.Service.Models;
using User.Management.Service.Models.Authentication.Login;
using User.Management.Service.Models.Authentication.SignUp;
using User.Management.Service.Models.Authentication.User;
using User.Management.Service.Services;

namespace User.Management.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IUserManagement _userManagement;


        public AuthenticationController(IUserManagement userManagement, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _userManagement = userManagement;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            var tokenResponse = await _userManagement.CreateUserWithTokenAsync(registerUser);
            if (tokenResponse.IsSuccess)
            {
                await _userManagement.AssignRoleToUserAsync(registerUser.Roles, tokenResponse.Response.User);

                var confirmationLink = Url.Action(
                                       action: nameof(ConfirmEmail),
                                      controller: "Authentication",
                                      values: new { tokenResponse.Response.Token, email = registerUser.Email },
                                       protocol: Request.Scheme,
                                       host: Request.Host.Value);
                var message = new Message(new string[] { registerUser.Email! }, "Confirmation email link", confirmationLink!);
                await _emailService.SendEmailAsync(message);
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", IsSuccess = true, Message = "Email Verified Successfully" });
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Message = tokenResponse.Message, IsSuccess = false });

        }

        [HttpGet(nameof(ConfirmEmail))]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email Verified SuccessFully" });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "This User Doesnot exist!" });
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var loginOTPResponse = await _userManagement.GetOtpByLoginAsync(login);
            if (loginOTPResponse.Response != null)
            {
                var user = loginOTPResponse!.Response!.User;
                if (user!.TwoFactorEnabled)
                {
                    var token = loginOTPResponse.Response!.Token;
                    var message = new Message(new string[] { user.Email! }, "OTP Confirmation", token);
                    await _emailService.SendEmailAsync(message);
                    return StatusCode(StatusCodes.Status200OK,
                        new Response { IsSuccess = loginOTPResponse.IsSuccess, Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}" });
                }
                if (user != null && await _userManager.CheckPasswordAsync(user, login.Password!))
                {
                    var serviceResponse = await _userManagement.GetJwtTokenAsync(user);
                    return Ok(serviceResponse);
                }
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route(nameof(LoginWithOTP))]
        public async Task<IActionResult> LoginWithOTP(string userName, string code)
        {
            var jwt = await _userManagement.LoginUserWithJWTokenAsync(code, userName);
            if (jwt.IsSuccess)
            {
                return Ok(jwt);
            }
            return StatusCode(StatusCodes.Status404NotFound,
                new Response { Status = "Success", Message = $"Invalid Code" });
        }

        [HttpPost]
        [Route("Refresh-Token")]
        public async Task<IActionResult> RefreshToken(LoginResponse tokens)
        {
            var jwt = await _userManagement.RenewAccessTokenAsync(tokens);
            if (jwt.IsSuccess)
            {
                return Ok(jwt);
            }
            return StatusCode(StatusCodes.Status404NotFound,
                new Response { Status = "Success", Message = $"Invalid Code" });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(ForgotPassword))]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgotPasswordLink = Url.Action(nameof(ResetPassword), "Authentication", new { token, email = user.Email }, Request.Scheme);
                var message = new Message(new string[] { user.Email! }, "Forgot Password link", forgotPasswordLink!);
                await _emailService.SendEmailAsync(message);
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = $"Password Changed request is sent on Email {user.Email}.Please Open your email & click the link" });
            }
            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = $"Couldnot send link to email, Please try again." });
        }


        [HttpPost(nameof(ResetPassword))]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (!resetPasswordResult.Succeeded)
                {
                    foreach (var error in resetPasswordResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return Ok(ModelState);
                }
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = $"Password has been changed" });
            }
            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = $"Couldnot send link to email, Please try again." });
        }
    }
}

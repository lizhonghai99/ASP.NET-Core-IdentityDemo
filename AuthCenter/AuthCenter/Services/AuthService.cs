using AuthCenter.Data;
using AuthCenter.Models.Dtos.Authentization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthCenter.Services
{
    public class AuthService : IAuthService
    {
        private IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IServiceProvider _serviceProvider;
        public AuthService(IConfiguration config, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IServiceProvider serviceProvider)
        {
            _config = config;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _serviceProvider = serviceProvider;
        }
        public async Task<AuthResponse> GenerateTokenByRole(string roleId)
        {
            var tokenId = Guid.NewGuid().ToString();
            var secret = GenerateRandomString();
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return new AuthResponse { IsSuccess = false, Message = "Role does not exsits" };
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti,tokenId),
                    new Claim(ClaimTypes.Role,role.Name)
            };
            var tokenConfigSection = _config.GetSection("Security:Token");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigSection["Key"]));
            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var signCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(
                issuer: tokenConfigSection["Issuer"],
                audience: tokenConfigSection["Audience"],
                claims: claims,
                expires: DateTime.Now.AddYears(100),
                signingCredentials: signCredential
                );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return new AuthResponse { IsSuccess = true, Token = token, Message = "Ok", TokenId = tokenId, Secret = secret };
        }
        public async Task<AuthResponse> GenerateTokenByUser(AuthRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return new AuthResponse { IsSuccess = false, Message = "User already exists!" };

            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);
            if (!signInResult.Succeeded)
                return new AuthResponse { IsSuccess = false, Message = "Check Email or Password" };
            var tokenId = Guid.NewGuid().ToString();
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,tokenId)
                };
            var userClaims = await _userManager.GetClaimsAsync(user);
            if (userClaims.Any())
            {
                foreach (var claim in userClaims)
                {
                    claims.Add(claim);
                }
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Any())
            {
                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            var tokenConfigSection = _config.GetSection("Security:Token");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigSection["Key"]));
            var signCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(
                issuer: tokenConfigSection["Issuer"],
                audience: tokenConfigSection["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: signCredential
                );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            var expiration = TimeZoneInfo.ConvertTimeFromUtc(jwtToken.ValidTo, TimeZoneInfo.Local);
            return new AuthResponse { IsSuccess = true, Token = token, Expiration = expiration, Message = "Ok", TokenId = tokenId };
        }

        public async Task ValidateToken(TokenValidatedContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var tokenId = context.Principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                var accessToken = await dbContext.AccessToken.Include(t => t.Role).FirstOrDefaultAsync(a => a.Id == tokenId);
                if (accessToken == null || !accessToken.InActive || context.SecurityToken == null)
                {
                    context.Fail("Token validation failed");
                    return;
                }
                var roleClaims = await _roleManager.GetClaimsAsync(accessToken.Role);
                foreach (var claim in roleClaims)
                {
                    context.Principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(claim.Type, claim.Value) }));
                }
                context.Success();
            }

        }

        private string GenerateRandomString(int length = 32)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }

    }
}

using AuthCenter.Models.Dtos.Authentization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuthCenter.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> GenerateTokenByRole(string roleId);

        Task<AuthResponse> GenerateTokenByUser(AuthRequest request);

        Task ValidateToken(TokenValidatedContext context);


    }
}

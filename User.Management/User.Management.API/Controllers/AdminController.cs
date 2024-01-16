using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace User.Management.API.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet("employees")]
        public IEnumerable<string> Get()
        {
            var claims = User.Claims.ToList();
            var name = claims.Select(t => t.Type == "Name").FirstOrDefault();
            var role = claims.Select(t => t.Type == "Role").FirstOrDefault();
            return new List<string>() { "Ahmed", "Ali", "Ahsan" };
        }
    }
}

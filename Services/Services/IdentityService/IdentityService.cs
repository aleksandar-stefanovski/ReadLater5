using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.IdentityService
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly AppSettings _jwtConfig;
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityService(IHttpContextAccessor httpContext, IOptionsMonitor<AppSettings> optionsMonitor, UserManager<IdentityUser> userManager)
        {
            _httpContext = httpContext;
            _jwtConfig = optionsMonitor.CurrentValue;
            _userManager = userManager;
        }

        public string GetUserId()
        {
            return _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string GenerateJwtToken(IdentityUser user)
        {
            Claim[] claims = {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimsIdentity.DefaultNameClaimType, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

            SigningCredentials credentials =
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret)),
            SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token =
           new JwtSecurityToken
           (claims: claims,
           signingCredentials: credentials,
           expires: DateTime.Now.AddHours(1));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

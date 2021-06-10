using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReadLater5.Controllers.API.Contracts;
using Services.IdentityService;
using System.Threading.Tasks;

namespace ReadLater5.Controllers.API
{
    [ApiController]
    [Route("api/identity")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityController(IIdentityService identityService, UserManager<IdentityUser> userManager)
        {
            _identityService = identityService;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Token([FromBody] UserLoginRequest user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if (existingUser == null)
                {
                    return BadRequest();
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);
                if (isCorrect)
                {
                    var jwtToken = _identityService.GenerateJwtToken(existingUser);

                    return Ok(new AuthResult()
                    {
                        Result = true,
                        Token = jwtToken
                    });
                }
                else
                {
                    return BadRequest();
                }
            }

            return BadRequest();
        }
    }
}

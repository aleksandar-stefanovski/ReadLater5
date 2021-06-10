using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.IdentityService
{
    public interface IIdentityService
    {
        string GetUserId();
        string GenerateJwtToken(IdentityUser model);
    }
}

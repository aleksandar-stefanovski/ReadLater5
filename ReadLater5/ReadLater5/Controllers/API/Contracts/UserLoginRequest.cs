using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadLater5.Controllers.API.Contracts
{
    public class UserLoginRequest
    {
            [Required]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }
    }
}

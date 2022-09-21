using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Användarnamn krävs")]
        [Display(Name = "Användarnamn: ")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Lösenord krävs")]
        [Display(Name = "Lösenord: ")]
        [UIHint("password")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}

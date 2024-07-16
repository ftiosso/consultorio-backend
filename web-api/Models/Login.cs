using System.ComponentModel.DataAnnotations;

namespace web_api.Models
{
    public class Login
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }

        public Login()
        {
            this.Email = string.Empty;
            this.Senha = string.Empty;
        }
    }
}
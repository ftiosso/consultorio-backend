using System.ComponentModel.DataAnnotations;

namespace web_api.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Email é obrigatório.")]
        [StringLength(100, ErrorMessage = "E-mail deve ter no máximo 100 caracteres.")]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória.")]        
        [StringLength(32, MinimumLength = 3, ErrorMessage = "Senha deve conter no mínimo 3 caracteres.")]
        public string Senha { get; set; }

        public Usuario()
        {            
            this.Nome = string.Empty;
            this.Email = string.Empty;
            this.Senha = string.Empty;
        }
    }
}
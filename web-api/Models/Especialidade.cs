using System.ComponentModel.DataAnnotations;

namespace web_api.Models
{
    public class Especialidade
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        public Especialidade() 
        { 
            this.Nome = string.Empty;
        }
    }
}
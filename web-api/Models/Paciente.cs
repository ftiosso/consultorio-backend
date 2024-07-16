using System;
using System.ComponentModel.DataAnnotations;

namespace web_api.Models
{
    public class Paciente
    {
        public int Codigo { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
        public string Nome { get; set; }
        
        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime? DataNascimento { get; set; }
    }
}
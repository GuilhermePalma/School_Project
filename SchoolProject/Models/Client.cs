using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SchoolProject.Models
{
    public class Client
    {

        private string name;
        private string cpf;
        private string logradouro;
        private string cidade;
        private string estado;
        private int numero;
        private string complemento;

        public Client() { }

        public bool validationCPF(string cpf)
        {
            return cpf != null && cpf.Length == 11;
        }

        [DisplayName("Nome")]
        [RegularExpression(@"^[A-Za-zà-úÀ-Ú\s]*", ErrorMessage = "O Nome deve ter apenas Letras")]
        [Required(ErrorMessage = "O Nome deve ser informado !")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "O Nome deve Ter entre {2} a {1} Letras")]
        public string Name { get => name; set => name = value; }

        [DisplayName("CPF")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "O CPF deve ter apenas Numeros")]
        [Required(ErrorMessage = "O CPF deve ser informado !")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve Ter {1} Digitos")]
        public string Cpf { get => cpf; set => cpf = value; }

        [DisplayName("Logradouro")]
        [RegularExpression(@"^[A-Za-zà-úÀ-Ú\s]*", ErrorMessage = "O Logradouro deve ter apenas Letras")]
        [Required(ErrorMessage = "O Logradouro deve ser informado !")]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "o Logradouro deve Ter entre {2} a {1} Letras")]
        public string Logradouro { get => logradouro; set => logradouro = value; }

        [DisplayName("Cidade")]
        [Required(ErrorMessage = "A Cidade deve ser informado !")]
        [RegularExpression(@"^[A-Za-zà-úÀ-Ú\s]*", ErrorMessage = "A Cidade deve ter apenas Letras")]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "A Cidade deve Ter entre {2} a {1} Letras")]
        public string Cidade { get => cidade; set => cidade = value; }

        // Uso da Lista de Estados ---> Armazenará somente a Sigla
        [DisplayName("Estado")]
        [Required(ErrorMessage = "O Estado deve ser informado !")]
        public string Estado { get => estado; set => estado = value; }

        [Required(ErrorMessage = "O Numero deve ser informado !")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "O Numero deve ter apenas Numeros")]
        [Range(0,1000000, ErrorMessage = "O Numero deve estar entre {1} a {2}")]
        [DisplayName("Numero")]
        public int Numero { get => numero; set => numero = value; }

        [DisplayName("Complemento")]
        [StringLength(40, ErrorMessage = "O Complemento deve ter no Maximo {1} Letas")]
        public string Complemento { get => complemento; set => complemento = value; }

    }
}
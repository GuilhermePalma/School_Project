using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SchoolProject.Models
{
    public class Seller
    {
        public Seller() { }

        public static bool ValidationCNPJ(string cnpj)
        {
            return !string.IsNullOrEmpty(cnpj) && cnpj.Length == 14;
        }

        [DisplayName("Nome")]
        [RegularExpression(@"^[A-Za-zà-úÀ-Ú\s]*", ErrorMessage = "O Nome deve ter apenas Letras")]
        [Required(ErrorMessage = "O Nome deve ser informado !")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "O Nome deve Ter entre {2} a {1} Letras")]
        public string Name { get; set; }

        [DisplayName("CNPJ")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "O CNPJ deve ter apenas Numeros")]
        [Required(ErrorMessage = "O CNPJ deve ser informado !")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "O CNPJ deve Ter {1} Digitos")]
        public string Cnpj { get; set; }

        [DisplayName("Logradouro")]
        [RegularExpression(@"^[A-Za-zà-úÀ-Ú\s]*", ErrorMessage = "O Logradouro deve ter apenas Letras")]
        [Required(ErrorMessage = "O Logradouro deve ser informado !")]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "o Logradouro deve Ter entre {2} a {1} Letras")]
        public string Logradouro { get; set; }

        [DisplayName("Cidade")]
        [Required(ErrorMessage = "A Cidade deve ser informado !")]
        [RegularExpression(@"^[A-Za-zà-úÀ-Ú\s]*", ErrorMessage = "A Cidade deve ter apenas Letras")]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "A Cidade deve Ter entre {2} a {1} Letras")]
        public string Cidade { get; set; }

        // Uso da Lista de Estados ---> Armazenará somente a Sigla
        [DisplayName("Estado")]
        [Required(ErrorMessage = "O Estado deve ser informado !")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "O Numero deve ser informado !")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "O Numero deve ter apenas Numeros")]
        [Range(0, 1000000, ErrorMessage = "O Numero deve estar entre {1} a {2}")]
        [DisplayName("Numero")]
        public int Numero { get; set; }

        [DisplayName("Complemento")]
        [StringLength(40, ErrorMessage = "O Complemento deve ter no Maximo {1} Letas")]
        public string Complemento { get; set; }

    }
}
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SchoolProject.Models
{
    public class Product
    {
        [DisplayName("Codigo do Produto")]
        public int Id_product { get; set; }

        [DisplayName("CNPJ do Vendedor")]
        public string Cnpj_Seller{ get; set; }

        [DisplayName("Nome")]
        [Required(ErrorMessage = "O Nome do Produto deve ser informado !")]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "O Nome deve Ter entre {2} a {1} Letras")]
        [RegularExpression(@"^[A-Za-zà-úÀ-Ú0-9\-\s]*", 
            ErrorMessage = "O Nome do Produto deve ter apenas Letras, Numeros e Hifen")]
        public string Name { get; set; }

        [DisplayName("Descrição")]
        [Required(ErrorMessage = "A descrição deve ser informado !")]
        [StringLength(5000, MinimumLength = 50, 
            ErrorMessage = "A Descrição do Peoduto deve Ter entre {2} a {1} Caracteres")]
        [RegularExpression(@"^[A-Za-zà-úÀ-Ú0-9\s\+\-\:\.\,\(\)\*\?\!\@\#\$]*",
            ErrorMessage = "A Descrição do Produto pode contar apenas os seguintes caracteres: " +
            "Letras, Numeros, Pontuações (., !, ? e ,) e Caracteres Especiais (+, -, :, (, ), *, @, #, $)")]
        public string Description { get; set; }

        [DisplayName("Quantidade")]
        [Required(ErrorMessage = "A Quantidade do Produto deve ser informado !")]
        [Range(1, 1000000000, 
            ErrorMessage = "A Quantidade do Produto deve estar entre {1} até 1.000.000.000 Itens")]
        [RegularExpression(@"^[0-9\S]*",
            ErrorMessage = "A Quantidade do Produto conter apenas Numeros")]
        public int Quantity { get; set; }

        [DisplayName("Preço")]
        [Required(ErrorMessage = "O Preço do Produto deve ser informado !")]
        [Range(0.5, 1000000000, ErrorMessage = "O Preço do Produto deve estar entre R$0,50 até R$1.000.000.000,00 ")]
        [RegularExpression(@"^[0-9\S]*",
            ErrorMessage = "O Preço do Produto deve conter apenas Numeros")]
        public int Price { get; set; }

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

    }
}
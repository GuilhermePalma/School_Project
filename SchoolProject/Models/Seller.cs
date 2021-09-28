using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SchoolProject.Models
{
    public class Seller
    {
        public Seller() { }

        // Strings de Erro no CPF
        public string Error_Validation { get; set; }

        public bool ValidationCNPJ(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj) || cnpj.Length != 14)
            {
                Error_Validation = "CNPJ Invalido. CNPJ Deve conter 14 Caracteres";
                return false;
            }

            Regex regex = new Regex(@"^[0-9]+$");
            if (!regex.IsMatch(cnpj))
            {
                Error_Validation = "CNPJ deve estar no Seguinte Formato: 99999999000099";
                return false;
            }
            return true;
        }

        // Valida o CNPJ e se está na mascara do CNPJ
        public bool ValidationMaskCNPJ(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj) || cnpj.Length != 18)
            {
                Error_Validation = "CNPJ Invalido. CNPJ Deve conter 18 Caracteres " +
                    "no seguinte Formato: 99.999.999/0000-99";
                return false;
            }

            Regex regex = new Regex(@"[0-9]{2}\.[0-9]{3}\.[0-9]{3}\/[0-9]{4}-[0-9]{2}");
            if (!regex.IsMatch(cnpj))
            {
                Error_Validation = "CNPJ deve estar no Seguinte Formato: 99.999.999/0000-99";
                return false;
            }
            return true;
        }

        // Valida e Converte o CNPJ com Mascara
        public string RemoveMaskCNPJ(string cnpj)
        {
            if (!ValidationMaskCNPJ(cnpj)) return string.Empty;

            try
            {
                string formatted_cpnj = "";
                formatted_cpnj = cnpj.Replace(".", string.Empty)
                    .Replace("-", string.Empty).Replace("/", string.Empty);

                return ValidationCNPJ(formatted_cpnj) ? formatted_cpnj : string.Empty;
            }
            catch (ArgumentException ex)
            {
                Error_Validation = "Não foi possivel Converter o CNPJ";
                System.Diagnostics.Debug.WriteLine(Error_Validation + " Exceção: " + ex);
                return string.Empty;
            }
        }

        // Coloca o CNPJ em uma Mascara
        public string Formatted_Cnpj(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj) || !ValidationCNPJ(cnpj))
            {
                Error_Validation = "CNPJ Invalido";
                return string.Empty;
            }

            try
            {
                // Formatação e Censura do CNPJ
                string cpf_formatted = string.Format("{0}.{1}.{2}/{3}-{4}",
                    cnpj.Substring(0, 2), "XXX", "XX" + cnpj.Substring(7, 1),
                    cnpj.Substring(8, 4), cnpj.Substring(12, 2));
                
                return cpf_formatted;
            }
            catch (Exception ex)
            {
                Error_Validation = "Houve um Erro ao Formatar o CNPJ.";
                System.Diagnostics.Debug.WriteLine(Error_Validation + " Exceção: " + ex);
                return string.Empty;
            }
        }

        [DisplayName("Nome")]
        [RegularExpression(@"^[A-Za-zà-úÀ-Ú\s]*", ErrorMessage = "O Nome deve ter apenas Letras")]
        [Required(ErrorMessage = "O Nome deve ser informado !")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "O Nome deve Ter entre {2} a {1} Letras")]
        public string Name { get; set; }

        [DisplayName("CNPJ")]
        [RegularExpression(@"[0-9]{2}\.[0-9]{3}\.[0-9]{3}\/[0-9]{4}-[0-9]{2}", 
            ErrorMessage = "CPF deve estar no Seguinte Formato: 99.999.999/0000-99")]
        [Required(ErrorMessage = "O CNPJ deve ser informado !")]
        [StringLength(18, MinimumLength = 18, 
            ErrorMessage = "O CNPJ deve Ter {1} Digitos, no seguinte Formato: 99.999.999/0000-99")]
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

        // Uso da Lista de DDD ---> Armazenará somente a 3 Digitos
        [DisplayName("DDD")]
        [Required(ErrorMessage = "O DDD deve ser informado !")]
        public string Ddd { get; set; }

        [DisplayName("Telefone")]
        [Required(ErrorMessage = "O Telefone deve ser informado !")]
        [RegularExpression(@"\([0-9]{2}\)[\s]{1}[0-9]{5}-[0-9]{4}",
            ErrorMessage = "O Telefone deve estar no Seguinte Formato: (00) 99999-9999")]
        public string Telefone { get; set; }
    }
}
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SchoolProject.Models
{
    public class Client
    {
        // Strings de Erro no CPF
        public string Error_Validation { get; set; }

        public Client() { }

        // Valida o CPF passados nas URL
        public bool ValidationCPF(string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
            {
                Error_Validation = "CPF Invalido. CPF deve conter 11 Caracteres";
                return false;
            }
            
            Regex regex = new Regex(@"^[0-9]+$");

            if (!regex.IsMatch(cpf))
            {
                Error_Validation = "CPF deve estar no Seguinte Formato: 99999999900";
                return false;
            }
            return true;
        }

        // Valida o CPF e a Mascara do CPF
        public bool ValidationMaskCPF(string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 14)
            {
                Error_Validation = "CPF Invalido. CPF deve conter 11 Caracteres";
                return false;
            }

            Regex regex = new Regex(@"[0-9]{3}\.[0-9]{3}\.[0-9]{3}-[0-9]{2}");

            if (!regex.IsMatch(cpf))
            {
                Error_Validation = "CPF deve estar no Seguinte Formato: 999.999.999-99";
                return false;
            }
            return true;
        }

        // Valida e Converte o CPF com Mascara
        public string ConvertMask(string cpf)
        {
            // Verifica o CPF com Mascara
            if (!ValidationMaskCPF(cpf)) return string.Empty; 

            try
            {
                // Substitui a Formatação do CPF e Valida
                string formmated_cpf = "";
                formmated_cpf = cpf.Replace(".", string.Empty).Replace("-",string.Empty);
                return ValidationCPF(formmated_cpf) ? formmated_cpf : string.Empty;
            }
            catch (ArgumentException ex)
            {
                System.Diagnostics.Debug.WriteLine("Não foi possivel Retirar a " +
                    "Mascara do CPF. Exeção: " + ex);
                Error_Validation = "Não foi possivel Converter o CPF";
                return string.Empty;
            }
        }

        [DisplayName("Nome")]
        [RegularExpression(@"^[A-Za-zà-úÀ-Ú\s]*", ErrorMessage = "O Nome deve ter apenas Letras")]
        [Required(ErrorMessage = "O Nome deve ser informado !")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "O Nome deve Ter entre {2} a {1} Letras")]
        public string Name { get; set; }

        [DisplayName("CPF")]
        [RegularExpression(@"[0-9]{3}\.[0-9]{3}\.[0-9]{3}-[0-9]{2}", 
            ErrorMessage = "CPF deve estar no Seguinte Formato: 999.999.999-99")]
        [Required(ErrorMessage = "O CPF deve ser informado !")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "O CPF deve Ter {1} Digitos " +
            "(11 do CPF e 3 da Formatação)")]
        public string Cpf { get; set; }

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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

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

        // Valida o CPF na Mascara
        public bool ValidationMaskCPF(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
            {
                Error_Validation = "CPF Invalido. CPF é obrigatorio";
                return false;
            }
            else if(cpf.Length != 14)
            {
                Error_Validation = "CPF Invalido. CPF deve conter 14 Caracteres " +
                    "no seguinte Formato: 999.999.999-99";
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

        // Valida somente o Telefone
        public bool ValidationPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                Error_Validation = "Telefone Invalido. Telefone é Obrigatorio";
                return false;
            } else if(phone.Length != 9)
            {
                Error_Validation = "Telefone Invalido. Telefone deve conter 9 Caracteres";
                return false;
            }

            Regex regex = new Regex(@"^[0-9]+$");

            if (!regex.IsMatch(phone))
            {
                Error_Validation = "Telefone deve estar no Seguinte Formato: 999999999";
                return false;
            }
            return true;
        }

        // Valida o Telefone na Mascara do CPF
        public bool ValidationMaskPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                Error_Validation = "Telefone Invalido. Telefone é Obrigatorio";
                return false;
            } else if(phone.Length != 15)
            {
                Error_Validation = "Telefone Invalido. Telefone deve conter 15 Caracteres " +
                    "no seguinte Formato: (00) 99999-9999";
                return false;
            }

            Regex regex = new Regex(@"\([0-9]{2}\)[\s]{1}[0-9]{5}-[0-9]{4}");

            if (!regex.IsMatch(phone))
            {
                Error_Validation = "Telefone deve estar no Seguinte Formato: (00) 99999-9999";
                return false;
            }
            return true;
        }
        
        // Valida e Converte o CPF com Mascara
        public string RemoveMaskCPF(string cpf)
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
                Error_Validation = "Não foi possivel Retirar a Mascara do CPF";
                System.Diagnostics.Debug.WriteLine(Error_Validation + " Exeção: " + ex);
                return string.Empty;
            }
        }

        // Retira o Telefone da Mascara
        public string RemoveMaskPhone(string phone)
        {
            // Verifica o CPF com Mascara
            if (!ValidationMaskPhone(phone)) return string.Empty;

            try
            {
                // Substitui a Formatação do CPF e Valida
                string formatted_phone = "";
                formatted_phone = phone.Substring(5, 10).Replace("-", string.Empty);

                return ValidationPhone(formatted_phone) ? formatted_phone : string.Empty;
            }
            catch (Exception ex)
            {
                Error_Validation = "Não foi possivel Retirar a Mascara do Telefone";
                System.Diagnostics.Debug.WriteLine(Error_Validation + " Exeção: " + ex);
                return string.Empty;
            }
        }

        // Retira o DDD da Mascara
        public string RemoveMaskDDD(string phone)
        {
            // Verifica o CPF com Mascara
            if (!ValidationMaskPhone(phone)) return string.Empty;

            string formmated_ddd = "";
            try
            {
                // Formatação do Telefone
                formmated_ddd = string.Format("0{0}", phone.Substring(0, 4)
                    .Replace("(", string.Empty).Replace(")", string.Empty));
            }
            catch (Exception ex)
            {
                Error_Validation = "Houve um Erro ao Formatar o DDD.";
                System.Diagnostics.Debug.WriteLine(Error_Validation + " Exceção: " + ex);
                return string.Empty;
            }

            string[] ddd_valid = DddValid();

            for (int i = 0; i < ddd_valid.Length; i++)
            {
                if (ddd_valid[i] == formmated_ddd) return formmated_ddd;
            }

            Error_Validation = "DDD Invalido. Tente Novamente";
            return string.Empty;
        }

        // Coloca o Telefone em uma Mascara
        public string FormattedPhone(string ddd, string phone)
        {
            if (string.IsNullOrEmpty(phone) || !ValidationPhone(phone))
            {
                return "Telefone Invalido";
            }

            string phone_formatted = "";
            try
            {
                // Formatação do Telefone
                phone_formatted = string.Format("({0}) {1}-{2}",
                ddd.Substring(1, 2), phone.Substring(0, 5), phone.Substring(5, 4));
            }
            catch (Exception ex)
            {
                Error_Validation = "Houve um Erro ao Formatar o Telefone.";
                System.Diagnostics.Debug.WriteLine(Error_Validation + " Exceção: " + ex);
                return string.Empty;
            }

            return phone_formatted.Length == 15 ? 
                phone_formatted : "Formatação do Telefone Invalida";
        }

        // Coloca o CPF em uma Mascara
        public string FormattedCPF(string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || !ValidationCPF(cpf))
            {
                return "CPF Invalido";
            }

            string cpf_formatted = "";
            try
            {
                // Formatação e Censura do CPF
                cpf_formatted = string.Format("{0}.{1}.{2}-{3}",
                    cpf.Substring(0, 3), "XXX", "XX" + cpf.Substring(8, 1),
                    cpf.Substring(9, 2));
            }
            catch(Exception ex)
            {
                Error_Validation = "Houve um Erro ao Formatar o CPF.";
                System.Diagnostics.Debug.WriteLine(Error_Validation + " Exceção: " + ex);
                return string.Empty;
            }

            return cpf_formatted.Length == 14 ? cpf_formatted : "Formatação do CPF Invalida";
        }

        // Lista com os DDD brasileiros Validos
        private string[] DddValid()
        {
            return new string[]
            {
                "011","012","013","014","015","016","017","018","019","021","022",
                "024","027","028","031","032","033","034","035","037","038","041",
                "042","043","044","045","046","047","048","049","051","053","054",
                "055","061","062","063","064","065","066","067","068","069","071",
                "073","074","075","077","079","081","082","083","084","085","086",
                "087","088","089","091","092","093","094","095","096","097","098","099"
            };
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
        [StringLength(14, MinimumLength = 14, ErrorMessage = "O CPF deve Ter {1} Digitos, " +
            "no seguinte Formato: 999.999.999-99")]
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

        [DisplayName("Numero")]
        [Required(ErrorMessage = "O Numero deve ser informado !")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "O Numero deve ter apenas Numeros")]
        [Range(0, 1000000, ErrorMessage = "O Numero deve estar entre {1} a {2}")]
        public int Numero { get; set; }

        [DisplayName("Complemento")]
        [StringLength(40, ErrorMessage = "O Complemento deve ter no Maximo {1} Letas")]
        public string Complemento { get; set; }

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
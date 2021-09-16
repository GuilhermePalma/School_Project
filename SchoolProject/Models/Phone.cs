using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SchoolProject.Models
{
    public class Phone
    {

        public Phone() { }

        public string Error_Validation { get; set; }


        public string Telefone { get; set; }
        public string Ddd { get; set; }


        // Valida somente o Telefone
        public bool ValidationPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                Error_Validation = "Telefone Invalido. Telefone é Obrigatorio";
                return false;
            }
            else if (phone.Length != 9)
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

        // Valida o Telefone na Mascara do Telegone
        public bool ValidationMaskPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                Error_Validation = "Telefone Invalido. Telefone é Obrigatorio";
                return false;
            }
            else if (phone.Length != 15)
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
                return string.IsNullOrEmpty(Error_Validation) ?
                    "Telefone Invalido" : Error_Validation;
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


        // Array com os DDD brasileiros Validos
        public string[] DddValid()
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
    }
}
using System;

namespace SchoolProject.Models
{
    // Classe criada na Normalização do Banco de Dados
    public class Address
    {
        public Address() { }

        // Validação do Logradouro
        public bool ValidationLogradouro(string logradouro)
        {
            if (string.IsNullOrEmpty(logradouro))
            {
                Error_Validation = "Cidade não Informada";
                return false;
            }
            else if (logradouro.Length < 5 || logradouro.Length > 80)
            {
                Error_Validation = string.Format("Logradouro Invalido. Somente é aceito " +
                    "Logradouros entre {0} a {1} Caracteres", 5, 80);
                return false;
            }
            else return true;
        }

        // Faz uma comparação entre objetos da Classe Address
        public override bool Equals(object objectAddress)
        {
            if (objectAddress is Address)
            {
                Address address = (Address)objectAddress;
                if (this.Logradouro == address.Logradouro && this.Cep == address.Cep)
                {
                    return true;
                }
                else if (this.Logradouro != address.Logradouro && this.Cep != address.Cep)
                {
                    Error_Validation = string.Empty;
                    return false;
                }
                else
                {
                    Error_Validation = "A alteração do Endereço só é permitida com " +
                        "a Alteração do CEP E ENDEREÇO";
                    return false;
                }
            }
            else return false;
        }

        public bool ValidationCEP(string cep)
        {
            try
            {
                if (string.IsNullOrEmpty(cep))
                {
                    Error_Validation = "CEP Invalido. Não foi Encontrado nenhum valor do CEP";
                    return false;
                }

                string normalizedCep = cep.Replace("-", string.Empty).Replace(".", string.Empty);
                if (normalizedCep.Length != 8)
                {
                    Error_Validation = "CEP Invalido. O CEP deve conter 8 Numeros";
                    return false;
                }
                else return true;
            }
            catch (Exception ex)
            {
                Error_Validation = "Não foi Possivel Validar o CEP";
                System.Diagnostics.Debug.WriteLine(Error_Validation + " Exception: " + ex);
                return false;
            }
        }

        public string RemoveMaskCEP(string cep)
        {
            if (!ValidationCEP(cep)) return string.Empty;
            try
            {
                string normalizedCep = cep.Replace("-", string.Empty).Replace(".", string.Empty);
                return ValidationCEP(normalizedCep) ? normalizedCep : string.Empty;
            }
            catch (Exception ex)
            {
                Error_Validation = "Não foi Possivel Formatar o CEP para o Banco de Dados";
                System.Diagnostics.Debug.WriteLine(Error_Validation + " Exception: " + ex);
                return string.Empty;
            }
        }
        
        public string FormattedCepMask(string cep)
        {
            if (!ValidationCEP(cep)) return string.Empty;
            try
            {
                string formattedCep = cep.Substring(0,5) + "-" + cep.Substring(5, 3);
                return formattedCep.Length == 9 ? formattedCep : string.Empty;
            }
            catch (Exception ex)
            {
                Error_Validation = "Não foi Possivel Formatar o CEP para o Banco de Dados";
                System.Diagnostics.Debug.WriteLine(Error_Validation + " Exception: " + ex);
                return string.Empty;
            }
        }

        public int Code_address { get; set; }
        public string Logradouro { get; set; }
        public string Cep { get; set; }
        public string Error_Validation { get; set; }
    }
}

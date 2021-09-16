using System.Collections.Generic;
using System.Web.Mvc;

namespace SchoolProject.Models
{
    // Classe criada na Normalização do Banco de Dados
    public class StateCity
    {
        public StateCity() { }

        public string Estado { get; set; }
        public string Cidade { get; set; }
        public int Code_stateCity { get; set; }

        public string Error_Validation { get; set; }

        // Valida os dados do Estado
        public bool ValidationState(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                Error_Validation = "Estado não Informado";
                return false;
            }
            else if (state.Length != 2)
            {
                Error_Validation = string.Format("Estado Invalido. Somente é aceito " +
                    "as Siglas dos Estados");
                return false;
            }
            else return true;
        }

        // Valida os Dados da Cidade
        public bool ValidationCity(string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                Error_Validation = "Cidade não Informada";
                return false;
            }
            else if (city.Length < 5 || city.Length > 80)
            {
                Error_Validation = string.Format("Cidade Invalida. Somente é aceito " +
                    "Cidades entre {0} a {1} Caracteres",5,80);
                return false;
            }
            else return true;
        }

        // Lista com o Nome e Valores dos Estados
        public List<SelectListItem> ListStates()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Selecione uma Opção", Value="", Disabled=true ,Selected = true},
                new SelectListItem { Text = "Acre", Value="AC"},
                new SelectListItem { Text = "Alagoas", Value = "AL" },
                new SelectListItem { Text = "Amapá", Value = "AP" },
                new SelectListItem { Text = "Amazonas", Value = "AM" },
                new SelectListItem { Text = "Bahia", Value = "BA" },
                new SelectListItem { Text = "Ceará", Value = "CE" },
                new SelectListItem { Text = "Distrito Federal", Value = "DF" },
                new SelectListItem { Text = "Espírito Santo", Value = "ES" },
                new SelectListItem { Text = "Goiás", Value = "GO" },
                new SelectListItem { Text = "Maranhão", Value = "MA" },
                new SelectListItem { Text = "Mato Grosso", Value = "MT" },
                new SelectListItem { Text = "Mato Grosso do Sul", Value = "MS" },
                new SelectListItem { Text = "Minas Gerais", Value = "MG" },
                new SelectListItem { Text = "Pará", Value = "PA" },
                new SelectListItem { Text = "Paraíba", Value = "PB" },
                new SelectListItem { Text = "Paraná", Value = "PR" },
                new SelectListItem { Text = "Pernambuco", Value = "PE" },
                new SelectListItem { Text = "Piauí", Value = "PI" },
                new SelectListItem { Text = "Rio de Janeiro", Value = "RJ" },
                new SelectListItem { Text = "Rio Grande do Norte", Value = "RN" },
                new SelectListItem { Text = "Rio Grande do Sul", Value = "RS" },
                new SelectListItem { Text = "Rondônia", Value = "RO" },
                new SelectListItem { Text = "Roraima", Value = "RR" },
                new SelectListItem { Text = "Santa Catarina", Value = "SC" },
                new SelectListItem { Text = "São Paulo", Value = "SP" },
                new SelectListItem { Text = "Sergipe", Value = "SE" },
                new SelectListItem { Text = "Tocantins", Value = "TO" },
                new SelectListItem { Text = "Estrangeiro", Value = "EX" }
            };
        }

        // Faz uma comparação entre objetos da Classe StateCity
        public override bool Equals(object o)
        {
            if (o is StateCity)
            {
                StateCity address = (StateCity)o;
                return this.Estado == address.Estado && this.Cidade == address.Cidade;
            } else return false;
        }

    }
}

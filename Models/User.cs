using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolProject.Models
{
    public class User
    {

        private string name;
        private string cpf;
        private string logradouro;
        private string cidade;
        private string estado;
        private int numero;
        private string complemento;

        public User() { }

        public User(string name, string cpf, string logradouro, string cidade, string estado, int numero, string complemento)
        {
            this.Name = name;
            this.Cpf = cpf;
            this.Logradouro = logradouro;
            this.Cidade = cidade;
            this.Estado = estado;
            this.Numero = numero;
            this.Complemento = complemento;
        }

        [DisplayName("Nome")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "O Nome deve ter apenas Letras")]
        [Required(ErrorMessage = "O Nome deve ser informado !")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "O Nome deve Ter entre {2} a {1} Letras")]
        public string Name { get => name; set => name = value; }

        [DisplayName("CPF")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "O CPF deve ter apenas Numeros")]
        [Required(ErrorMessage = "O CPF deve ser informado !")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve Ter {1} Digitos")]
        public string Cpf { get => cpf; set => cpf = value; }

        [DisplayName("Logradouro")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "O Logradouro deve ter apenas Letras")]
        [Required(ErrorMessage = "O Logradouro deve ser informado !")]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "o Logradouro deve Ter entre {2} a {1} Letras")]
        public string Logradouro { get => logradouro; set => logradouro = value; }

        [DisplayName("Cidade")]
        [Required(ErrorMessage = "A Cidade deve ser informado !")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "A Cidade deve ter apenas Letras")]
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



        // Lista com o Nome e Valores dos Estados
        public List<SelectListItem> listStates()
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

    }
}
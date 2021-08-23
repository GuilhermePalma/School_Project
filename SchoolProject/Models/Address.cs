using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Models
{
    // Classe criada na Normalização do Banco de Dados
    public class Address
    {
        private int code_address;
        private string logradouro;

        public Address() { }

        public int Code_address { get => code_address; set => code_address = value; }
        public string Logradouro { get => logradouro; set => logradouro = value; }
    }
}

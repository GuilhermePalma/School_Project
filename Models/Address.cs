using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpreationsDatabase.DLLs
{
    public class Address
    {
        private int code_address;
        private string logradouro;

        public Address() { }
        public Address(int code_address, string logradouro)
        {
            this.Code_address = code_address;
            this.Logradouro = logradouro;
        }

        public int Code_address { get => code_address; set => code_address = value; }
        public string Logradouro { get => logradouro; set => logradouro = value; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpreationsDatabase.DLLs
{
    public class StateCity
    {
        private int code_statecity;
        private string estado;
        private string cidade;

        public StateCity()
        {
        }

        public StateCity(int code_statecity, string estado, string cidade)
        {
            this.Estado = estado;
            this.Cidade = cidade;
            this.code_statecity = code_statecity;
        }

        public string Estado { get => estado; set => estado = value; }
        public string Cidade { get => cidade; set => cidade = value; }
        public int Code_statecity { get => code_statecity; set => code_statecity = value; }
    }
}

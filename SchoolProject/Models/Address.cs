namespace SchoolProject.Models
{
    // Classe criada na Normalização do Banco de Dados
    public class Address
    {
        public Address() { }

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

        public int Code_address { get; set; }
        public string Logradouro { get; set; }
        public string Error_Validation { get; set; }
    }
}

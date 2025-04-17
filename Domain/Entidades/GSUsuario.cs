using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.NET.Core.Validador;
using JJ.NET.CrossData.Atributo;

namespace Domain.Entidades
{
    public class GSUsuario
    {
        [ChavePrimaria, Obrigatorio]
        public int PK_GSUsuario { get; set; }

        [Obrigatorio]
        public string Usuario { get; set; }

        [Obrigatorio]
        public string Senha { get; set; }

        [Editavel(false)]
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();
    }
}

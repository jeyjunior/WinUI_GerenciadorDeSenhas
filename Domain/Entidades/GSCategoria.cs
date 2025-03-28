using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.NET.Core.Validador;
using JJ.NET.CrossData.Atributo;

namespace Domain.Entidades
{
    public class GSCategoria
    {
        [ChavePrimaria, Obrigatorio]
        public int PK_GSCategoria { get; set; }

        [Obrigatorio]
        public string Categoria { get; set; }

        [Editavel(false)]
        public ValidarResultado Validar { get; set; } = new ValidarResultado();
    }
}

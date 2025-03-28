using JJ.NET.Core.Validador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidades
{
    public class GSConfiguracao
    {
    }

    public class CriptografiaRequest
    {
        public string Valor { get; set; }
        public string IV { get; set; }

        public ValidarResultado ValidarResultado { get; set; }
    }
}

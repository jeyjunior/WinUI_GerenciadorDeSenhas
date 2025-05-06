using JJ.NET.Core.Validador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSDomain.DTO
{
    public abstract class CriptografiaBase
    {
        public string Valor { get; set; }
        public string Salt { get; set; }
    }

    public class CriptografiaRequisicao : CriptografiaBase
    {
        public int PK_GSUsuario { get; set; }
    }

    public class CriptografiaResultado : CriptografiaBase
    {
        public ValidarResultado ValidarResultado { get; set; }
    }

    public class DescriptografiaResultado
    {
        public string Valor { get; set; }
        public ValidarResultado ValidarResultado { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSDomain.Enumeradores;
using JJ.NET.Core.Validador;
using JJ.NET.CrossData.Atributo;

namespace GSDomain.Entidades
{
    public class GSCredencial
    {
        [ChavePrimaria, Obrigatorio]
        public int PK_GSCredencial { get; set; }

        [Obrigatorio]
        public string Credencial { get; set; }
        [Obrigatorio]
        public string Senha { get; set; }
        [Obrigatorio]
        public string IVSenha { get; set; }

        [Relacionamento("GSCategoria", "PK_GSCategoria")]
        public int? FK_GSCategoria { get; set; }

        [Obrigatorio]
        public int FK_GSUsuario { get; set; }

        [Obrigatorio]
        public DateTime DataCriacao { get; set; }
        public DateTime? DataModificacao { get; set; }

        [Editavel(false)]
        public GSCategoria GSCategoria { get; set; }

        [Editavel(false)]
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();
    }


    public class GSCredencialPesquisaRequest
    {
        public string Valor { get; set; }
        public TipoDePesquisa TipoDePesquisa { get; set; }
        public TipoDeOrdenacao TipoDeOrdenacao { get; set; }
        public int FK_GSUsuario { get; set; }
        public ValidarResultado ValidarResultado { get; set; }
    }
}

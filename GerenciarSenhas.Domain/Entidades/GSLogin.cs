using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.NET.Core.Extensoes;
using JJ.NET.Core.Validador;
using JJ.NET.CrossData.Atributo;

namespace GerenciarSenhas.Domain.Entidades
{
    [Entidade("GSLogin")]
    public class GSLogin
    {
        [ChavePrimaria, Obrigatorio]
        public int PK_GSLogin { get; set; }
        [Obrigatorio]
        public string Email { get; set; }

        [Relacionamento("GSSenha", "PK_GSSenha")]
        public int FK_GSSenha { get; set; }
        [Obrigatorio]
        public DateTime DataCriacao { get; set; }

        [Editavel(false)]
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();

        public bool Validar()
        {
            this.ValidarResultado = new ValidarResultado();

            if (Email.ObterValorOuPadrao("").Trim() == "" || !Email.Contains("@"))
            {
                this.ValidarResultado.Adicionar("E-mail inválido.");
                return false;
            }

            return true;
        }
    }
}

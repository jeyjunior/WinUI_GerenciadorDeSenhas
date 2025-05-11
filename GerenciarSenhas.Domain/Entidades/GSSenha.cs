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
    public class GSSenha
    {
        [ChavePrimaria, Obrigatorio]
        public int PK_GSSenha { get; set; }
        [Obrigatorio]
        public string Senha { get; set; }
        [Obrigatorio]
        public string Salt { get; set; }
        [Obrigatorio]
        public DateTime DataCriacao { get; set; }
        public DateTime? DateExpira { get; set; }

        [Editavel(false)]
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();

        public bool Validar()
        {
            this.ValidarResultado = new ValidarResultado();

            if (Senha.ObterValorOuPadrao("").Trim() == "")
            {
                this.ValidarResultado.Adicionar("Senha inválida.");
                return false;
            }
            else if(Salt.ObterValorOuPadrao("").Trim() == "")
            {
                this.ValidarResultado.Adicionar("Salt inválido.");
                return false;
            }

            return true;
        }
    }
}

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
    [Entidade("GSCredencial")]
    public class GSCredencial
    {
        [ChavePrimaria, Obrigatorio]
        public int PK_GSCredencial { get; set; }
        [Obrigatorio]
        public string Credencial { get; set; }
        [Obrigatorio]
        public string Usuario { get; set; }
        public string Acesso { get; set; } // App, OS, Site, SW...
        [Obrigatorio]
        public int FK_GSSenha { get; set; }
        public int? FK_GSCategoria { get; set; }

        [Obrigatorio]
        public DateTime DataCriacao { get; set; }
        public DateTime? DataModificacao { get; set; }

        [Editavel(false)]
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();
        public bool Validar()
        {
            this.ValidarResultado = new ValidarResultado();

            if (Credencial.ObterValorOuPadrao("").Trim() == "")
            {
                this.ValidarResultado.Adicionar("Credencial inválida.");
                return false;
            }
            else if (Usuario.ObterValorOuPadrao("").Trim() == "")
            {
                this.ValidarResultado.Adicionar("Usuário inválida.");
                return false;
            }
            else if (FK_GSSenha <= 0)
            {
                this.ValidarResultado.Adicionar("Senha inválida.");
                return false;
            }

            return true;
        }
    }
}

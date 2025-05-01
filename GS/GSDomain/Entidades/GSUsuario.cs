using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.NET.Core.Validador;
using JJ.NET.CrossData.Atributo;

namespace GSDomain.Entidades
{
    public class GSUsuario
    {
        [ChavePrimaria, Obrigatorio]
        public int PK_GSUsuario { get; set; }

        [Obrigatorio]
        public string Usuario { get; set; }
        [Obrigatorio]
        public string Nome { get; set; }
        [Obrigatorio]
        public string Senha { get; set; }
        [Obrigatorio]
        public string IVSenha { get; set; }
        [Editavel(false)]
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();

        public GSUsuario DeepCopy()
        {
            return new GSUsuario
            {
                PK_GSUsuario = this.PK_GSUsuario,
                Usuario = this.Usuario,
                Nome = this.Nome,
                Senha = this.Senha,
                IVSenha = this.IVSenha,
                ValidarResultado = new ValidarResultado(),
            };
        }
    }

    public class GSUsuarioRequest
    {
        public string Usuario { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();
    }

    public class GSUsuarioLembrar
    {
        public string Usuario { get; set; }
    }
}

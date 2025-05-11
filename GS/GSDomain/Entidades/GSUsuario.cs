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
        public string Login { get; set; }
        [Obrigatorio]
        public string Email { get; set; }
        [Obrigatorio]
        public string Senha { get; set; }
        [Obrigatorio]
        public string Salt { get; set; }
        [Editavel(false)]
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();

        public GSUsuario DeepCopy()
        {
            return new GSUsuario
            {
                PK_GSUsuario = this.PK_GSUsuario,
                Login = this.Login,
                Email = this.Email,
                Senha = this.Senha,
                Salt = this.Salt,
                ValidarResultado = new ValidarResultado(),
            };
        }
    }

    public class GSUsuarioRequest
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();
    }

    public class GSUsuarioLembrar
    {
        public string Login { get; set; }
    }
}

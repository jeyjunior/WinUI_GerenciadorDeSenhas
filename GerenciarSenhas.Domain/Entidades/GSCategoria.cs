using JJ.NET.Core.Extensoes;
using JJ.NET.Core.Validador;
using JJ.NET.CrossData.Atributo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciarSenhas.Domain.Entidades
{
    public class GSCategoria
    {
        [ChavePrimaria, Obrigatorio]
        public int PK_GSCategoria { get; set; }
        [Obrigatorio]
        public string Categoria { get; set; }
        [Obrigatorio]
        public int FK_GSLogin { get; set; }

        [Editavel(false)]
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();

        public bool Validar()
        {
            this.ValidarResultado = new ValidarResultado();

            if (Categoria.ObterValorOuPadrao("").Trim() == "")
            {
                this.ValidarResultado.Adicionar("Categoria inválida.");
                return false;
            }

            return true;
        }
    }
}

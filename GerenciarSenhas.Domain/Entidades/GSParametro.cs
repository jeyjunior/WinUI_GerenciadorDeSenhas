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
    public class GSParametro
    {
        [ChavePrimaria, Obrigatorio]
        public int PK_GSParametro { get; set; }
        [Obrigatorio]
        public string Parametro { get; set; }
        public string Valor { get; set; }
        public string Descricao { get; set; }

        [Editavel(false)]
        public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();
        
        public bool Validar()
        {
            this.ValidarResultado = new ValidarResultado();

            if (Parametro.ObterValorOuPadrao("").Trim() == "")
            {
                this.ValidarResultado.Adicionar("Parâmetro inválido.");
                return false;
            }

            return true;
        }
    }
}

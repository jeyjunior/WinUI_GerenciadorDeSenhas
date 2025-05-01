using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSDomain.Enumeradores
{
    public enum TipoDePesquisa
    {
        Todos = 0,
        Categoria = 1,
        Credencial = 2,
    }

    public enum TipoDeOrdenacao
    {
        Cadastro = 0,
        Modificação = 1,
        Categoria = 2,
        Credencial = 3
    }

    public enum DirecaoOrdenacao
    {
        Crescente = 1,
        Decrescente = 2
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSDomain.Entidades;
using JJ.NET.CrossData.Interface;

namespace GSDomain.Interfaces
{
    public interface IGSCredencialRepository : IRepository<GSCredencial>
    {
        IEnumerable<GSCredencial> ObterLista(string condition = "", string orderBy = "", object parameters = null);
        int Deletar(string condition = "", object parameters = null);
    }
}

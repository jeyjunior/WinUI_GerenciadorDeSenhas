using Domain.Entidades;
using JJ.NET.CrossData.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IGSCredencialRepository : IRepository<GSCredencial>
    {
        IEnumerable<GSCredencial> ObterLista(string condition = "", string orderBy = "", object parameters = null);
    }
}

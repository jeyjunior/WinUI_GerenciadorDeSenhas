using GerenciarSenhas.Domain.Entidades;
using GerenciarSenhas.Domain.Interfaces;
using JJ.NET.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciarSenhas.InfraData.Repository
{
    public class GSParametroRepository : Repository<GSParametro>, IGSParametroRepository
    {
        public GSParametroRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

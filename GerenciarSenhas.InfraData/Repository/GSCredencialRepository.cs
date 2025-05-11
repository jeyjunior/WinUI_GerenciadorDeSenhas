using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GerenciarSenhas.Domain.Entidades;
using GerenciarSenhas.Domain.Interfaces;
using JJ.NET.Data.Interfaces;

namespace GerenciarSenhas.InfraData.Repository
{
    public class GSCredencialRepository : Repository<GSCredencial>, IGSCredencialRepository
    {
        public GSCredencialRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

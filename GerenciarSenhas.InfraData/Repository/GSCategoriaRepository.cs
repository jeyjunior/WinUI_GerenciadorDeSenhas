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
    public class GSCategoriaRepository : Repository<GSCategoria>, IGSCategoriaRepository
    {
        public GSCategoriaRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

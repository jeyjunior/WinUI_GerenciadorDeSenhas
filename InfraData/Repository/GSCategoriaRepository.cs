using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entidades;
using Domain.Interfaces;
using JJ.NET.Data.Interfaces;

namespace InfraData.Repository
{
    public class GSCategoriaRepository : Repository<GSCategoria>, IGSCategoriaRepository
    {
        public GSCategoriaRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}

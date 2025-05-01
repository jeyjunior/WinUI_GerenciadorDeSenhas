using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSDomain.Entidades;
using GSDomain.Interfaces;
using JJ.NET.Data.Interfaces;

namespace GSInfraData.Repository
{
    public class GSUsuarioRepository : Repository<GSUsuario>, IGSUsuarioRepository
    {
        public GSUsuarioRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

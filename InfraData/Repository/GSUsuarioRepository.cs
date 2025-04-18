using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.NET.Data.Interfaces;
using Domain.Entidades;
using Domain.Interfaces;

namespace InfraData.Repository
{
    public class GSUsuarioRepository : Repository<GSUsuario>, IGSUsuarioRepository
    {
        public GSUsuarioRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

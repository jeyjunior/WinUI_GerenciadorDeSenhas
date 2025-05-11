using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.NET.Data.Interfaces;
using GerenciarSenhas.Domain.Entidades;
using GerenciarSenhas.Domain.Interfaces;

namespace GerenciarSenhas.InfraData.Repository
{
    public class GSSenhaRepository : Repository<GSSenha>, IGSSenhaRepository
    {
        public GSSenhaRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

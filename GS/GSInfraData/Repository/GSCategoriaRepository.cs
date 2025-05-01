using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSDomain.Entidades;
using GSDomain.Interfaces;
using JJ.NET.Core.Extensoes;
using JJ.NET.Data.Interfaces;
using Dapper;

namespace GSInfraData.Repository
{
    public class GSCategoriaRepository : Repository<GSCategoria>, IGSCategoriaRepository
    {
        public GSCategoriaRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public int Deletar(string condition = "", object parameters = null)
        {
            string query = "DELETE FROM GSCategoria";

            if (condition.ObterValorOuPadrao("").Trim() != "")
                query += " WHERE " + condition + "\n";

            var resultado = unitOfWork.Connection.Execute(
                sql: query.ToSQL(),
                param: parameters,
                transaction: unitOfWork.Transaction);

            return resultado;
        }
    }
}

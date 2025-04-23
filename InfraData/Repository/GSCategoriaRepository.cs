using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Domain.Entidades;
using Domain.Interfaces;
using JJ.NET.Core.Extensoes;
using JJ.NET.Data.Interfaces;

namespace InfraData.Repository
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
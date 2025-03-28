using Dapper;
using Domain.Entidades;
using Domain.Interfaces;
using JJ.NET.Core.Extensoes;
using JJ.NET.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfraData.Repository
{
    public class GSCredencialRepository : Repository<GSCredencial>, IGSCredencialRepository
    {
        private string QUERY = "";
        private string ORDERBY = "";

        public GSCredencialRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            QUERY = " SELECT  GSCredencial.*, \n" +
                    "         GSCategoria.* \n" +
                    " FROM    GSCredencial\n" +
                    " LEFT    JOIN    GSCategoria\n" +
                    "         ON      GSCategoria.PK_GSCategoria = GSCredencial.FK_GSCategoria\n";

            ORDERBY = " ORDER   BY \n" +
                    "           GSCredencial.DataModificacao \n" +
                    "           DESC; \n";
        }

        public override IEnumerable<GSCredencial> ObterLista(string condition = "", object parameters = null)
        {
            string query = QUERY;

            if (condition.ObterValorOuPadrao("").Trim() != "")
                query += " WHERE " + condition + "\n";

            query += ORDERBY;

            var resultado = unitOfWork.Connection.Query<GSCredencial, GSCategoria, GSCredencial>(
                sql: query.ToSQL(),
                map: (credencial, categoria) =>
                {
                    credencial.GSCategoria = categoria;
                    return credencial;
                },
                param: parameters,
                splitOn: "PK_GSCredencial, PK_GSCategoria")
                .ToList();

            return resultado;
        }

        public IEnumerable<GSCredencial> ObterLista(string condition = "", string orderBy = "", object parameters = null)
        {
            string query = QUERY;

            if (condition.ObterValorOuPadrao("").Trim() != "")
                query += " WHERE " + condition + "\n";

            query += (orderBy.ObterValorOuPadrao("").Trim() != "") ? orderBy : ORDERBY;

            var resultado = unitOfWork.Connection.Query<GSCredencial, GSCategoria, GSCredencial>(
                sql: query.ToSQL(),
                map: (credencial, categoria) =>
                {
                    credencial.GSCategoria = categoria;
                    return credencial;
                },
                param: parameters,
                splitOn: "PK_GSCredencial, PK_GSCategoria")
                .ToList();

            return resultado;
        }
    }
}

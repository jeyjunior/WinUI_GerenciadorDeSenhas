using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.NET.Core.Extensoes;
using JJ.NET.CrossData;
using JJ.NET.Cryptography;
using JJ.NET.Data;
using GSInfraData.Repository;
using GSApplication.Interfaces;

namespace GSApplication.Services
{
    public class ConfigAppService : IConfigAppService
    {
        public CriptografiaResult Descriptografar(string valor, string iv)
        {
            var criptografiaRequest = new JJ.NET.Cryptography.CriptografiaRequest
            {
                IV = iv.ObterValorOuPadrao("").Trim(),
                Valor = valor.ObterValorOuPadrao("").Trim(),
                TipoCriptografia = JJ.NET.Cryptography.Enumerador.TipoCriptografia.AES,
            };

            var criptografiaResult = Criptografia.Descriptografar(criptografiaRequest);

            if (criptografiaResult.Erro.ObterValorOuPadrao("").Trim() != "")
                return new CriptografiaResult { Valor = "", IV = "", Erro = criptografiaResult.Erro };

            return criptografiaResult;
        }
        public CriptografiaResult Criptografar(string valor, string iv)
        {
            var criptografiaRequest = new JJ.NET.Cryptography.CriptografiaRequest
            {
                IV = iv.ObterValorOuPadrao("").Trim(),
                Valor = valor.ObterValorOuPadrao("").Trim(),
                TipoCriptografia = JJ.NET.Cryptography.Enumerador.TipoCriptografia.AES,
            };

            var criptografiaResult = Criptografia.Criptografar(criptografiaRequest);

            if (criptografiaResult.Erro.ObterValorOuPadrao("").Trim() != "")
                return new CriptografiaResult { Valor = "", IV = "", Erro = criptografiaResult.Erro };

            return criptografiaResult;
        }
        public bool DeletarContaUsuarioLogado(int PK_GSUsuario)
        {
            bool ret = false;

            using (var uow = new UnitOfWork(ConfiguracaoBancoDados.ObterConexao()))
            {
                var _gSCategoriaRepository = new GSCategoriaRepository(uow);
                var _gSCredencialRepository = new GSCredencialRepository(uow);
                var _gSUsuarioRepository = new GSUsuarioRepository(uow);
                try
                {
                    uow.Begin();

                    var categoriaDeletada = _gSCategoriaRepository.Deletar(" GSCategoria.FK_GSUsuario = @PK_GSUsuario", new { PK_GSUsuario = PK_GSUsuario });
                    var credencialDeletado = _gSCredencialRepository.Deletar(" GSCredencial.FK_GSUsuario = @PK_GSUsuario", new { PK_GSUsuario = PK_GSUsuario });
                    var usuarioDeletado = _gSUsuarioRepository.Deletar(PK_GSUsuario);
                    uow.Commit();

                    if (categoriaDeletada > 0 || credencialDeletado > 0 || usuarioDeletado > 0)
                        ret = true;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    throw new Exception(ex.Message);
                }
            }

            return ret;
        }
    }
}

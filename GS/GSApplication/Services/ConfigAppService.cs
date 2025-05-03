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
using GSDomain.DTO;
using JJ.NET.Cryptography.Interfaces;

namespace GSApplication.Services
{
    public class ConfigAppService : IConfigAppService
    {
        private readonly ISeguranca seguranca;
        public ConfigAppService()
        {
            seguranca = Bootstrap.Container.GetInstance<ISeguranca>();
        }
        public DescriptografiaResultado Descriptografar(CriptografiaRequisicao criptoRequest)
        {
            var result = new DescriptografiaResultado
            {
                ValidarResultado = new JJ.NET.Core.Validador.ValidarResultado(),
                Valor = ""
            };

            try
            {
                var request = new JJ.NET.Cryptography.DTO.DescriptografiaRequest
                {
                    ValorCriptografado = criptoRequest.Valor.ObterValorOuPadrao("").Trim(),
                    Salt = criptoRequest.Salt.ObterValorOuPadrao("").Trim()
                };

                var valorDescriptografado = seguranca.Descriptografar(request);

                if (valorDescriptografado.ObterValorOuPadrao("").Trim() == "")
                {
                    result.ValidarResultado.Adicionar("Falha ao tentar descriptografar.");
                    return result;
                }

                result.Valor = valorDescriptografado;
            }
            catch (Exception ex)
            {
                result.Valor = "";
                result.ValidarResultado.Adicionar(ex.Message);
            }

            return result;
        }
        public CriptografiaResultado Criptografar(string valor)
        {
            var result = new CriptografiaResultado
            {
                ValidarResultado = new JJ.NET.Core.Validador.ValidarResultado(),
                Valor = "",
                Salt = ""
            };

            try
            {
                var criptografiaResult = seguranca.Criptografar(valor);

                if (criptografiaResult == null)
                {
                    result.ValidarResultado.Adicionar("Falha ao tentar criptografar.");
                    return result;
                }
                else if(criptografiaResult.ValorCriptografado.ObterValorOuPadrao("").Trim() == "" || criptografiaResult.Salt.ObterValorOuPadrao("").Trim() == "")
                {
                    result.ValidarResultado.Adicionar("Falha ao tentar criptografar.");
                    return result;
                }

                result.Salt = criptografiaResult.Salt;
                result.Valor = criptografiaResult.ValorCriptografado;
            }
            catch (Exception ex)
            {
                result.Salt = "";
                result.Valor = "";
                result.ValidarResultado.Adicionar(ex.Message);
            }

            return result;
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

        public string GerarChavePrincipal()
        {
            return seguranca.GerarChavePrincipal();
        }
    }
}

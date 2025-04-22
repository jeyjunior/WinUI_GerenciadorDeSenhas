using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entidades;
using Domain.Enumeradores;
using Domain.Interfaces;
using InfraData.Repository;
using Application.Interfaces;
using JJ.NET.Core.DTO;
using JJ.NET.Core.Validador;
using JJ.NET.Core.Extensoes;
using JJ.NET.Cryptography;
using JJ.NET.Data;
using JJ.NET.CrossData;

namespace Application.Services
{
    internal class CredencialAppService : ICredencialAppService
    {
        #region Interface
        private readonly IGSCredencialRepository gSCredencialRepository;
        private readonly IConfigAppService configAppService;
        #endregion

        #region Construtor
        public CredencialAppService()
        {
            gSCredencialRepository = Bootstrap.Container.GetInstance<IGSCredencialRepository>();
            configAppService = Bootstrap.Container.GetInstance<IConfigAppService>();
        }
        #endregion

        #region Eventos
        #endregion

        #region Metodos
        public IEnumerable<GSCredencial> Pesquisar(GSCredencialPesquisaRequest requisicao)
        {
            if (requisicao == null)
                return new List<GSCredencial>();

            requisicao.ValidarResultado = new ValidarResultado();

            string condicao = "";
            string ordernacao = "";
            string valor = requisicao.Valor.LimparEntradaSQL();

            switch (requisicao.TipoDePesquisa)
            {
                case TipoDePesquisa.Todos:
                    condicao = $" (GSCategoria.Categoria COLLATE NOCASE LIKE '%{valor}%'   OR \n" +
                                $" GSCredencial.Credencial COLLATE NOCASE LIKE '%{valor}%') AND \n";
                    break;
                case TipoDePesquisa.Categoria:
                    condicao = $" (GSCategoria.Categoria COLLATE NOCASE LIKE '%{valor}%') AND \n";
                    break;
                case TipoDePesquisa.Credencial:
                    condicao = $" (GSCredencial.Credencial COLLATE NOCASE LIKE '%{valor}%') AND \n";
                    break;
                default:
                    break;
            }
            
            condicao += $" GSCredencial.FK_GSUsuario = {requisicao.FK_GSUsuario} ";

            switch (requisicao.TipoDeOrdenacao)
            {
                case TipoDeOrdenacao.Cadastro: ordernacao = " GSCredencial.DataCriacao, GSCredencial.DataModificacao, GSCategoria.Categoria, GSCredencial.Credencial "; break;
                case TipoDeOrdenacao.Modificação: ordernacao = " GSCredencial.DataModificacao, GSCategoria.Categoria, GSCredencial.Credencial "; break;
                case TipoDeOrdenacao.Categoria: ordernacao = " GSCategoria.Categoria, GSCredencial.DataModificacao, GSCredencial.Credencial "; break;
                case TipoDeOrdenacao.Credencial: ordernacao = " GSCredencial.Credencial, GSCategoria.Categoria, GSCredencial.DataModificacao "; break;
            }

            if (ordernacao.ObterValorOuPadrao("").Trim() != "")
                ordernacao = "ORDER   BY \n" + ordernacao + " DESC";

            return gSCredencialRepository.ObterLista(condicao, ordernacao);
        }
        public GSCredencial PesquisarPorID(int PK_GSCredencial)
        {
            return gSCredencialRepository.Obter(PK_GSCredencial);
        }
        public ObservableCollection<Item> ObterTipoDePesquisa()
        {
            var tipoDePesquisa = new ObservableCollection<Item>()
            {
                new Item { ID = ((int)TipoDePesquisa.Todos).ToString(), Valor = TipoDePesquisa.Todos.ToString()},
                new Item { ID = ((int)TipoDePesquisa.Categoria).ToString(), Valor = TipoDePesquisa.Categoria.ToString()},
                new Item { ID = ((int)TipoDePesquisa.Credencial).ToString(), Valor = TipoDePesquisa.Credencial.ToString()},
            };

            return tipoDePesquisa;
        }
        public ObservableCollection<Item> ObterTipoDeOrdenacao()
        {
            var tipoDeOrdenacao = new ObservableCollection<Item>()
            {
                new Item { ID = ((int)TipoDeOrdenacao.Cadastro).ToString(), Valor = TipoDeOrdenacao.Cadastro.ToString()},
                new Item { ID = ((int)TipoDeOrdenacao.Modificação).ToString(), Valor = TipoDeOrdenacao.Modificação.ToString()},
                new Item { ID = ((int)TipoDeOrdenacao.Categoria).ToString(), Valor = TipoDeOrdenacao.Categoria.ToString()},
                new Item { ID = ((int)TipoDeOrdenacao.Credencial).ToString(), Valor = TipoDeOrdenacao.Credencial.ToString()},
            };

            return tipoDeOrdenacao;
        }
        public int SalvarCredencial(GSCredencial gSCredencial)
        {
            if (gSCredencial == null)
                return -1;

            bool atualizarRegistro = (gSCredencial.PK_GSCredencial > 0);

            gSCredencial.ValidarResultado = new ValidarResultado();

            if (gSCredencial.Credencial.ObterValorOuPadrao("").Trim() == "")
            {
                gSCredencial.ValidarResultado.Adicionar("Credencial é um campo obrigatório.");
                return -1;
            }

            if (gSCredencial.Senha.ObterValorOuPadrao("").Trim() == "")
            {
                gSCredencial.ValidarResultado.Adicionar("Senha é um campo obrigatório.");
                return -1;
            }

            var credencial = new GSCredencial
            {
                PK_GSCredencial = gSCredencial.PK_GSCredencial,
                Credencial = gSCredencial.Credencial,
                DataCriacao = gSCredencial.DataCriacao,
                DataModificacao = (atualizarRegistro) ? DateTime.Now : gSCredencial.DataCriacao,
                FK_GSUsuario = gSCredencial.FK_GSUsuario
            };

            if (gSCredencial.FK_GSCategoria != null && gSCredencial.FK_GSCategoria > 0)
                credencial.FK_GSCategoria = gSCredencial.FK_GSCategoria;

            var criptografarRequest = new JJ.NET.Cryptography.CriptografiaRequest
            {
                TipoCriptografia = JJ.NET.Cryptography.Enumerador.TipoCriptografia.AES,
                Valor = gSCredencial.Senha,
                IV = gSCredencial.IVSenha.ObterValorOuPadrao(""),
            };

            var criptografarResult = Criptografia.Criptografar(criptografarRequest);

            if (criptografarResult.Erro.ObterValorOuPadrao("").Trim() != "")
            {
                gSCredencial.ValidarResultado.Adicionar(criptografarResult.Erro);
                return -1;
            }

            credencial.Senha = criptografarResult.Valor;
            credencial.IVSenha = criptografarResult.IV;

            int PK_GESCredencial = -1;

            using (var uow = new UnitOfWork(ConfiguracaoBancoDados.ObterConexao()))
            {
                var _gSCredencialRepository = new GSCredencialRepository(uow);
                try
                {
                    uow.Begin();

                    if (atualizarRegistro)
                    {
                        PK_GESCredencial = _gSCredencialRepository.Atualizar(credencial);
                    }
                    else
                    {
                        PK_GESCredencial = _gSCredencialRepository.Adicionar(credencial);
                    }

                    uow.Commit();
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    throw new Exception(ex.Message);
                }
            }

            if (atualizarRegistro)
                return PK_GESCredencial > 0 ? gSCredencial.PK_GSCredencial : -1;

            return PK_GESCredencial;
        }
        public bool DeletarCredencial(int PK_GSCredencial)
        {
            bool ret = false;

            using (var uow = new UnitOfWork(ConfiguracaoBancoDados.ObterConexao()))
            {
                var _gSCredencialRepository = new GSCredencialRepository(uow);
                try
                {
                    uow.Begin();

                    var result = _gSCredencialRepository.Deletar(PK_GSCredencial);

                    uow.Commit();

                    if (result > 0)
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
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entidades;
using Domain.Interfaces;
using InfraData.Repository;
using JJ.NET.Core.Extensoes;
using JJ.NET.Core.Validador;
using JJ.NET.CrossData;
using JJ.NET.Cryptography;
using JJ.NET.Data;

namespace Application.Services
{
    public class CategoriaAppService : ICategoriaAppService
    {
        #region Interfaces
        private readonly IGSCategoriaRepository gSCategoriaRepository;
        #endregion
        
        #region Construtor
        public CategoriaAppService ()
        {
            gSCategoriaRepository = Bootstrap.Container.GetInstance<IGSCategoriaRepository>();
        }
        #endregion

        #region Eventos
        #endregion

        #region Metodos
        public IEnumerable<GSCategoria> ObterCategorias()
        {
            return gSCategoriaRepository.ObterLista();
        }
        public ObservableCollection<GSCategoria> ObterCategoriasObservableCollection()
        {
            var gSCategoria = gSCategoriaRepository.ObterLista().OrderBy(i => i.Categoria);
            var gSCategoriaObservableCollection = new ObservableCollection<GSCategoria>();

            foreach (var item in gSCategoria)
                gSCategoriaObservableCollection.Add(item);

            return gSCategoriaObservableCollection;
        }
        public bool DeletarCategoria(int PK_GSCategoria)
        {
            bool ret = false;

            using (var uow = new UnitOfWork(ConfiguracaoBancoDados.ObterConexao()))
            {
                var _gSCategoriaRepository = new GSCategoriaRepository(uow);
                try
                {
                    uow.Begin();

                    var result = _gSCategoriaRepository.Deletar(PK_GSCategoria);

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

        public int SalvarCategoria(GSCategoria gSCategoria)
        {
            if (gSCategoria == null)
                return -1;

            bool atualizarRegistro = (gSCategoria.PK_GSCategoria > 0);

            gSCategoria.ValidarResultado = new ValidarResultado();

            if (gSCategoria.Categoria.ObterValorOuPadrao("").Trim() == "")
            {
                gSCategoria.ValidarResultado.Adicionar("Categoria é um campo obrigatório.");
                return -1;
            }

            var categoria = new GSCategoria
            {
                PK_GSCategoria = gSCategoria.PK_GSCategoria,
                Categoria = gSCategoria.Categoria
            };

            int PK_GSCategoria = -1;

            using (var uow = new UnitOfWork(ConfiguracaoBancoDados.ObterConexao()))
            {
                var _gSCategoriaRepository = new GSCategoriaRepository(uow);
                try
                {
                    uow.Begin();

                    if (atualizarRegistro)
                    {
                        PK_GSCategoria = _gSCategoriaRepository.Atualizar(categoria);
                    }
                    else
                    {
                        PK_GSCategoria = _gSCategoriaRepository.Adicionar(categoria);
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
                return PK_GSCategoria > 0 ? gSCategoria.PK_GSCategoria : -1;

            return PK_GSCategoria;
        }
        #endregion
    }
}

using Microsoft.Data.SqlClient;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.NET.CrossData;
using JJ.NET.CrossData.Extensao;
using JJ.NET.CrossData.Enumerador;
using JJ.NET.Data.Interfaces;
using GSDomain.Entidades;
using GSDomain.Interfaces;
using GSApplication.Interfaces;
using JJ.NET.Data;
using GSApplication.Services;
using GSInfraData.Repository;
using Windows.Storage;
using JJ.NET.Cryptography.Interfaces;
using JJ.NET.Cryptography;

namespace GSApplication
{
    public static class Bootstrap
    {
        public static Container Container { get; private set; }
        public static async Task IniciarAsync()
        {
            try
            {
                string caminhoDestino = ApplicationData.Current.LocalFolder.Path;
                ConfiguracaoBancoDados.IniciarConfiguracao(Conexao.SQLite, caminhoDestino);
                var dbConnection = ConfiguracaoBancoDados.ObterConexao();
                Container = new Container();
                Container.Options.DefaultLifestyle = Lifestyle.Scoped;

                Container.Register<IUnitOfWork>(() => new UnitOfWork(dbConnection), Lifestyle.Singleton);

                //REPOSITORIOS
                Container.Register<IGSUsuarioRepository, GSUsuarioRepository>(Lifestyle.Singleton);
                Container.Register<IGSCategoriaRepository, GSCategoriaRepository>(Lifestyle.Singleton);
                Container.Register<IGSCredencialRepository, GSCredencialRepository>(Lifestyle.Singleton);

                //APP SERVICE
                Container.Register<ILoginService, LoginService>(Lifestyle.Singleton);
                Container.Register<INotificationService, NotificationService>(Lifestyle.Singleton);
                Container.Register<ICredencialAppService, CredencialAppService>(Lifestyle.Singleton);
                Container.Register<ICategoriaAppService, CategoriaAppService>(Lifestyle.Singleton);
                Container.Register<IConfigAppService, ConfigAppService>(Lifestyle.Singleton);

                // RECURSOS 
                Container.Register<ISeguranca>(() => new Seguranca(caminhoDestino), Lifestyle.Singleton);

                Container.Options.EnableAutoVerification = false;

                IniciarBaseDeDados();
            }
            catch (SqlException ex)
            {
                throw new Exception("Erro ao se conectar ao banco de dados.\n", ex);
            }
            catch (IOException ex)
            {
                throw new Exception("Erro ao acessar arquivos de configuração.\n", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro inesperado.\n", ex);
            }
        }
        private static void IniciarBaseDeDados()
        {
            var uow = Container.GetInstance<IUnitOfWork>();

            CriarTabelas(uow);
        }
        private static void CriarTabelas(IUnitOfWork uow)
        {
            bool gSCategoria = false;
            bool gSCredencial = false;
            bool gSUsuario = false;

            try
            {
                gSUsuario = uow.Connection.VerificarTabelaExistente<GSUsuario>();
                gSCategoria = uow.Connection.VerificarTabelaExistente<GSCategoria>();
                gSCredencial = uow.Connection.VerificarTabelaExistente<GSCredencial>();
            }
            catch (SqlException ex)
            {
                throw new Exception("Erro ao verificar a existência das tabelas", ex);
            }

            if (gSUsuario && gSCategoria && gSCredencial)
                return;

            try
            {
                uow.Begin();

                if (!gSUsuario)
                    uow.Connection.CriarTabela<GSUsuario>(uow.Transaction);

                if (!gSCategoria)
                    uow.Connection.CriarTabela<GSCategoria>(uow.Transaction);

                if (!gSCredencial)
                    uow.Connection.CriarTabela<GSCredencial>(uow.Transaction);

                uow.Commit();
            }
            catch (SqlException ex)
            {
                uow.Rollback();
                throw new Exception("Erro ao criar as tabelas no banco de dados", ex);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw new Exception("Erro inesperado ao criar as tabelas", ex);
            }
        }
        private static void InserirInformacoesIniciais(IUnitOfWork uow)
        {
            var gSCategoriaRepository = Container.GetInstance<IGSCategoriaRepository>();

            try
            {
                if (gSCategoriaRepository.ObterLista().ToList().Count() > 0)
                    return;

                var categorias = new string[]
                {
                    "Redes Sociais",
                    "Bancos e Finanças",
                    "E-commerce",
                    "Email",
                    "Trabalho",
                    "Streaming",
                    "Jogos",
                    "Lojas de Aplicativos",
                    "Fóruns",
                    "Plataformas de Ensino e Cursos",
                    "Celulares e Dispositivos Móveis",
                    "Computadores e Sistemas Operacionais",
                    "VPNs e Proxy",
                    "Carteiras Digitais",
                    "Serviços de Backup",
                };

                uow.Begin();

                for (int i = 0; i < categorias.Length; i++)
                    gSCategoriaRepository.Adicionar(new GSCategoria { Categoria = categorias[i] });

                uow.Commit();
            }
            catch (SqlException ex)
            {
                uow.Rollback();
                throw new Exception("Erro ao inserir informações iniciais", ex);
            }
            catch (IOException ex)
            {
                uow.Rollback();
                throw new Exception("Erro ao acessar arquivos durante a inserção de dados", ex);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw new Exception("Erro inesperado ao inserir informações iniciais", ex);
            }
        }
    }
}

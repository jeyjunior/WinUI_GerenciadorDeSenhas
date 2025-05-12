using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.NET.CrossData;
using JJ.NET.CrossData.Enumerador;
using Microsoft.Identity.Client;
using Windows.Storage;
using SimpleInjector;
using JJ.NET.Data;
using JJ.NET.Data.Interfaces;
using Microsoft.Data.SqlClient;
using System.IO;
using GerenciarSenhas.InfraData.Repository;
using GerenciarSenhas.Domain.Interfaces;
using JJ.NET.CrossData.Atributo;
using JJ.NET.CrossData.Extensao;
using System.Data;
using System.Reflection;
using GerenciarSenhas.Domain.Entidades;
using Dapper;

namespace GerenciarSenhas.Services
{
    public static class Bootstrap
    {
        public static Container Container { get; private set; }

        public static async Task IniciarAsync()
        {
            try
            {
                ConfiguracaoBancoDados.IniciarConfiguracao(Conexao.SQLite, ObterCaminhoPadrao());
                var dbConnection = ConfiguracaoBancoDados.ObterConexao();

                Container = new Container();
                Container.Options.DefaultLifestyle = Lifestyle.Scoped;

                Container.Register<IUnitOfWork>(() => new UnitOfWork(dbConnection), Lifestyle.Singleton);

                //REPOSITORIOS
                Container.Register<IGSCategoriaRepository, GSCategoriaRepository>(Lifestyle.Singleton);
                Container.Register<IGSCredencialRepository, GSCredencialRepository>(Lifestyle.Singleton);
                Container.Register<IGSLoginRepository, GSLoginRepository>(Lifestyle.Singleton);
                Container.Register<IGSParametroRepository, GSParametroRepository>(Lifestyle.Singleton);
                Container.Register<IGSSenhaRepository, GSSenhaRepository>(Lifestyle.Singleton);

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

        private static string ObterCaminhoPadrao()
        {
            var pastaApp = ApplicationData.Current.LocalFolder.CreateFolderAsync(
                "gs",
                CreationCollisionOption.OpenIfExists
            ).GetAwaiter().GetResult();

            return pastaApp.Path;
        }

        private static void IniciarBaseDeDados()
        {
            var uow = Container.GetInstance<IUnitOfWork>();

            CriarTabelas(uow);
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
        private static void CriarTabelas(IUnitOfWork uow)
        {
            var entidades = ObterEntidadesMapeadas();
            var tabelasExistentes = VerificarTabelasExistentes(uow.Connection, entidades);

            if (tabelasExistentes.Any(kvp => !kvp.Value))
            {
                try
                {
                    uow.Begin();

                    foreach (var entidade in tabelasExistentes.Where(e => !e.Value))
                    {
                        uow.Connection.CriarTabela(entidade.Key, uow.Transaction);
                    }

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
        }
        private static Dictionary<Type, bool> VerificarTabelasExistentes(IDbConnection connection, IEnumerable<Type> entidades)
        {
            var resultado = new Dictionary<Type, bool>();

            foreach (var entidade in entidades)
            {
                try
                {
                    resultado[entidade] = connection.VerificarTabelaExistente(entidade);
                }
                catch
                {
                    // Se houver erro, assumimos que a tabela não existe
                    resultado[entidade] = false;
                }
            }

            return resultado;
        }
        private static IEnumerable<Type> ObterEntidadesMapeadas()
        {
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies();

            var assTip = assemblies.SelectMany(assembly => assembly.GetTypes());
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetCustomAttribute<EntidadeAttribute>() != null && type.IsClass && !type.IsAbstract);
        }
        public static bool VerificarTabelaExistente(this IDbConnection connection, Type entidade)
        {
            string tabela = entidade.Name;
            string query = ConfiguracaoBancoDados.TipoConexaoSelecionada switch
            {
                Conexao.SQLite => $"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tabela}';",
                Conexao.SQLServer => $"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tabela}'",
                Conexao.MySql => $"SELECT count(*) FROM information_schema.tables WHERE table_name = '{tabela}'",
                _ => throw new InvalidOperationException("Banco de dados não suportado")
            };

            return connection.ExecuteScalar<int>(query) > 0;
        }
    }
}

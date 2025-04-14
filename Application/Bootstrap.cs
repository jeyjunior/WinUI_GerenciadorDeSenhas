using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SimpleInjector;
using JJ.NET.Data;
using JJ.NET.CrossData;
using JJ.NET.CrossData.Extensao;
using JJ.NET.CrossData.Enumerador;
using JJ.NET.Data.Interfaces;
using Domain.Interfaces;
using Domain.Entidades;
using InfraData.Repository;
using Application.Services;
using Application.Interfaces;
using Microsoft.UI.Xaml.Controls;


namespace Application
{
    public class Bootstrap
    {
        public static Container Container { get; private set; }
        public static void Inicializar()
        {
            try
            {
                string caminhoDestino = AppDomain.CurrentDomain.BaseDirectory;
                ConfiguracaoBancoDados.IniciarConfiguracao(Conexao.SQLite, "Gerenciador de Senhas", caminhoDestino);

                Container = new Container();
                Container.Options.DefaultLifestyle = Lifestyle.Scoped;

                Container.Register<IUnitOfWork>(() => new UnitOfWork(ConfiguracaoBancoDados.ObterConexao()), Lifestyle.Singleton);

                // REPOSITORIOS
                Container.Register<IGSCategoriaRepository, GSCategoriaRepository>(Lifestyle.Singleton);
                Container.Register<IGSCredencialRepository, GSCredencialRepository>(Lifestyle.Singleton);

                // APP SERVICE
                Container.Register<ICredencialAppService, CredencialAppService>(Lifestyle.Singleton);
                Container.Register<ICategoriaAppService, CategoriaAppService>(Lifestyle.Singleton);
                Container.Register<INotificationService, NotificationService>(Lifestyle.Singleton);

                // VIEW MODELS

                Container = Bootstrap.Container;
                Container.Verify();

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
            InserirInformacoesIniciais(uow);
            //InserirInformacoesTeste();
        }
        private static void CriarTabelas(IUnitOfWork uow)
        {
            bool gSCategoria = false;
            bool gSCredencial = false;

            try
            {
                gSCategoria = uow.Connection.VerificarTabelaExistente<GSCategoria>();
                gSCredencial = uow.Connection.VerificarTabelaExistente<GSCredencial>();
            }
            catch (SqlException ex)
            {
                throw new Exception("Erro ao verificar a existência das tabelas", ex);
            }

            if (gSCategoria && gSCredencial)
                return;

            try
            {
                uow.Begin();

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

        #region Teste
        public static void InserirInformacoesTeste()
        {
            var credencialAppService = Container.GetInstance<ICredencialAppService>();
            var random = new Random();

            try
            {
                for (int i = 0; i < 50; i++)
                {
                    var gSCredencial = new GSCredencial
                    {
                        Credencial = GerarCredencial(random),
                        Senha = GerarSenha(random),
                        FK_GSCategoria = random.Next(1, 15),
                        DataCriacao = GerarDataCriacao(random),
                        DataModificacao = random.NextDouble() > 0.5 ? (DateTime?)GerarDataCriacao(random) : null
                    };

                    credencialAppService.SalvarCredencial(gSCredencial);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Erro ao inserir informações iniciais", ex);
            }
            catch (IOException ex)
            {
                throw new Exception("Erro ao acessar arquivos durante a inserção de dados", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro inesperado ao inserir informações iniciais", ex);
            }
        }
        private static string GerarCredencial(Random random)
        {
            int tipoCredencial = random.Next(1, 4);  // 1 = Login, 2 = Email, 3 = CPF

            switch (tipoCredencial)
            {
                case 1:
                    return GerarLogin(random);
                case 2:
                    return GerarEmail(random);
                case 3:
                    return GerarCPF(random);
                default:
                    return GerarLogin(random);
            }
        }
        private static string GerarLogin(Random random)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int length = random.Next(5, 15);
            var login = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                login.Append(chars[random.Next(chars.Length)]);
            }

            return login.ToString();
        }
        private static string GerarEmail(Random random)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            var localPart = new StringBuilder();
            int localLength = random.Next(5, 10);
            for (int i = 0; i < localLength; i++)
            {
                localPart.Append(chars[random.Next(chars.Length)]);
            }

            return localPart + "@gmail.com";
        }
        private static string GerarSenha(Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_-+=<>?";
            int senhaLength = random.Next(4, 33);
            var senha = new StringBuilder();

            for (int i = 0; i < senhaLength; i++)
            {
                senha.Append(chars[random.Next(chars.Length)]);
            }

            return senha.ToString();
        }
        private static DateTime GerarDataCriacao(Random random)
        {
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2025, 1, 1);
            int range = (endDate - startDate).Days;

            return startDate.AddDays(random.Next(range));
        }
        private static string GerarCPF(Random random)
        {
            int[] cpf = new int[11];

            for (int i = 0; i < 9; i++)
            {
                cpf[i] = random.Next(0, 10);
            }

            cpf[9] = GerarDigitoVerificador(cpf, 10);

            cpf[10] = GerarDigitoVerificador(cpf, 11);

            return string.Join("", cpf.Take(3)) + "." + string.Join("", cpf.Skip(3).Take(3)) + "." + string.Join("", cpf.Skip(6).Take(3)) + "-" + string.Join("", cpf.Skip(9));
        }
        private static int GerarDigitoVerificador(int[] cpf, int peso)
        {
            int soma = 0;
            for (int i = 0; i < peso - 1; i++)
            {
                soma += cpf[i] * (peso - i);
            }

            int digito = soma % 11;
            return digito < 2 ? 0 : 11 - digito;
        }
        #endregion
    }
}

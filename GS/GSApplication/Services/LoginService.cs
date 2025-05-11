using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JJ.NET.Core.Extensoes;
using JJ.NET.Core.Validador;
using JJ.NET.CrossData;
using JJ.NET.Cryptography;
using JJ.NET.Data;
using GSApplication.Interfaces;
using GSDomain.Entidades;
using GSDomain.Interfaces;
using GSInfraData.Repository;

namespace GSApplication.Services
{
    public class LoginService : ILoginService
    {
        #region Interfaces
        private readonly ICredencialAppService credencialAppService;
        private readonly IGSUsuarioRepository gSUsuarioRepository;
        private readonly IConfigAppService configAppService;
        #endregion

        #region Propriedades
        public object App { get; private set; }
        #endregion

        #region Construtor
        public LoginService()
        {
            credencialAppService = Bootstrap.Container.GetInstance<ICredencialAppService>();
            gSUsuarioRepository = Bootstrap.Container.GetInstance<IGSUsuarioRepository>();
            configAppService = Bootstrap.Container.GetInstance<IConfigAppService>();
        }
        #endregion

        #region Eventos
        #endregion

        #region Metodos
        public int Entrar(GSUsuarioRequest gSUsuarioRequest)
        {
            if (gSUsuarioRequest == null)
                return -1;

            gSUsuarioRequest.ValidarResultado = new ValidarResultado();

            try
            {
                var gSUsuarios = gSUsuarioRepository.ObterLista("Login = @Login", new { Login = gSUsuarioRequest.Login.ObterValorOuPadrao("").Trim() }).ToList();

                if (gSUsuarios == null || gSUsuarios.Count <= 0)
                    return -1;

                foreach (var item in gSUsuarios)
                {
                    var criptografiaResult = configAppService.Descriptografar(new GSDomain.DTO.CriptografiaRequisicao { Valor = item.Senha, Salt = item.Salt, PK_GSUsuario = item.PK_GSUsuario });

                    if (criptografiaResult.ValidarResultado.ObterPrimeiroErro().ObterValorOuPadrao("").Trim() != "")
                        continue;

                    if (gSUsuarioRequest.Senha == criptografiaResult.Valor)
                        return item.PK_GSUsuario;
                }
            }
            catch (Exception ex)
            {
                gSUsuarioRequest.ValidarResultado.Adicionar(ex.Message);
                return -1;
            }

            gSUsuarioRequest.ValidarResultado.Adicionar("Login ou senha inválido.");
            return -1;
        }
        public bool Registrar(GSUsuarioRequest gSUsuarioRequest)
        {
            if (gSUsuarioRequest == null)
                return false;

            gSUsuarioRequest.ValidarResultado = new ValidarResultado();

            if (gSUsuarioRequest.Login.ObterValorOuPadrao("").Trim() == "")
            {
                gSUsuarioRequest.ValidarResultado.Adicionar("Login é um campo obrigatório.");
                return false;
            }
            else if (gSUsuarioRequest.Email.ObterValorOuPadrao("").Trim() == "")
            {
                gSUsuarioRequest.ValidarResultado.Adicionar("E-mail é um campo obrigatório.");
                return false;
            }
            else if (gSUsuarioRequest.Senha.ObterValorOuPadrao("").Trim() == "")
            {
                gSUsuarioRequest.ValidarResultado.Adicionar("Senha é um campo obrigatório.");
                return false;
            }

            var gSUsuarioExistente = gSUsuarioRepository.ObterLista("Login = @Login", new { Login = gSUsuarioRequest.Login }).FirstOrDefault();

            if (gSUsuarioExistente != null)
            {
                gSUsuarioRequest.ValidarResultado.Adicionar("Login já existe.");
                return false;
            }

            using (var uow = new UnitOfWork(ConfiguracaoBancoDados.ObterConexao()))
            {
                var _gSUsuarioRepository = new GSUsuarioRepository(uow);
                try
                {
                    uow.Begin();

                    var gSUsuario = new GSUsuario
                    {
                        PK_GSUsuario = 0,
                        Login = gSUsuarioRequest.Login.ObterValorOuPadrao("").Trim(),
                        Email = gSUsuarioRequest.Email.ObterValorOuPadrao("").Trim(),
                        Senha = "",
                        Salt = "",
                        ValidarResultado = new ValidarResultado()
                    };

                    int PK_GSUsuario = _gSUsuarioRepository.Adicionar(gSUsuario);

                    var ret = configAppService.GerarChavePrincipal(PK_GSUsuario);
                    
                    if (ret.ObterValorOuPadrao("").Trim() == "")
                    {
                        gSUsuarioRequest.ValidarResultado.Adicionar("Não foi possível gerar uma Chave Principal para o registro do usuário.");
                        return false;
                    }

                    var criptografarResult = configAppService.Criptografar(gSUsuarioRequest.Senha, PK_GSUsuario);

                    if (!criptografarResult.ValidarResultado.EhValido)
                    {
                        gSUsuarioRequest.ValidarResultado.Adicionar(criptografarResult.ValidarResultado.ObterPrimeiroErro());
                        return false;
                    }

                    gSUsuario = new GSUsuario
                    {
                        PK_GSUsuario = PK_GSUsuario,
                        Login = gSUsuarioRequest.Login.ObterValorOuPadrao("").Trim(),
                        Email = gSUsuarioRequest.Email.ObterValorOuPadrao("").Trim(),
                        Senha = criptografarResult.Valor.ObterValorOuPadrao("").Trim(),
                        Salt = criptografarResult.Salt.ObterValorOuPadrao("").Trim(),
                        ValidarResultado = new ValidarResultado()
                    };

                    _gSUsuarioRepository.Atualizar(gSUsuario);

                    uow.Commit();

                    return PK_GSUsuario > 0;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
        public void DeletarUsuarioLembrado()
        {
            try
            {
                string localPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
                var arquivo = Path.Combine(localPath, "lembrarUsuario.json");
                File.Delete(arquivo);
            }
            catch
            {
            }
        }
        public async void SalvarUsuarioLembrado(string usuario)
        {
            try
            {
                var json = JsonSerializer.Serialize(new GSUsuarioLembrar { Login = usuario });
                string localPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
                var arquivo = Path.Combine(localPath, "lembrarUsuario.json");
                await File.WriteAllTextAsync(arquivo, json);
            }
            catch
            {
            }
        }
        public async Task<string> ObterUsuarioLembrado()
        {
            try
            {
                string localPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
                var arquivo = Path.Combine(localPath, "lembrarUsuario.json");
                if (!File.Exists(arquivo))
                    return null;

                var json = await File.ReadAllTextAsync(arquivo);
                var obj = JsonSerializer.Deserialize<GSUsuarioLembrar>(json);
                return obj?.Login;
            }
            catch
            {
                return "";
            }
        }
        public GSUsuario ObterUsuario(int pK_GSUsuario)
        {
            return gSUsuarioRepository.Obter(pK_GSUsuario);
        }
        public bool AtualizarUsuario(GSUsuario gSUsuario)
        {
            if (gSUsuario == null)
                return false;

            gSUsuario.ValidarResultado = new ValidarResultado();

            if (gSUsuario.PK_GSUsuario <= 0)
            {
                gSUsuario.ValidarResultado.Adicionar("Não foi possível identificar o usuário para atualizar.");
                return false;
            }

            if (gSUsuario.Email.ObterValorOuPadrao("").Trim() == "" || gSUsuario.Login.ObterValorOuPadrao("").Trim() == "" || gSUsuario.Senha.ObterValorOuPadrao("").Trim() == "" || gSUsuario.Salt.ObterValorOuPadrao("").Trim() == "")
            {
                gSUsuario.ValidarResultado.Adicionar("É necessário informar todos os dados do usuário (E-mail, Login e Senha).");
                return false;
            }

            using (var uow = new UnitOfWork(ConfiguracaoBancoDados.ObterConexao()))
            {
                var _gSUsuarioRepository = new GSUsuarioRepository(uow);
                try
                {
                    uow.Begin();

                    int PK_GSUsuario = _gSUsuarioRepository.Atualizar(gSUsuario);

                    uow.Commit();

                    return PK_GSUsuario > 0;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
        #endregion
    }
}

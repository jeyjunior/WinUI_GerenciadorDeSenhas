using System;
using System.Collections.Generic;
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
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly ICredencialAppService credencialAppService;
        private readonly IGSUsuarioRepository gSUsuarioRepository;

        public LoginService()
        {
            credencialAppService = Bootstrap.Container.GetInstance<ICredencialAppService>();
            gSUsuarioRepository = Bootstrap.Container.GetInstance<IGSUsuarioRepository>();
        }

        public int Entrar(GSUsuarioRequest gSUsuarioRequest)
        {
            if (gSUsuarioRequest == null)
                return -1;

            gSUsuarioRequest.ValidarResultado = new ValidarResultado();

            if (gSUsuarioRequest.Usuario.ObterValorOuPadrao("").Trim() == "")
            {
                gSUsuarioRequest.ValidarResultado.Adicionar("Usuário inválido.");
                return -1;
            }

            if (gSUsuarioRequest.Senha.ObterValorOuPadrao("").Trim() == "")
            {
                gSUsuarioRequest.ValidarResultado.Adicionar("Senha inválida.");
                return -1;
            }

            try
            {
                var gSUsuarios = gSUsuarioRepository.ObterLista("Usuario = @Usuario", new { Usuario = gSUsuarioRequest.Usuario }).ToList();

                if (gSUsuarios == null || gSUsuarios.Count <= 0)
                    return -1;

                foreach (var item in gSUsuarios)
                {
                    string valor = credencialAppService.Descriptografar(item.Senha, item.IVSenha);

                    if (valor.ObterValorOuPadrao("").Trim() == "")
                        continue;

                    if (gSUsuarioRequest.Senha == valor)
                        return item.PK_GSUsuario;
                }
            }
            catch (Exception ex)
            {
                gSUsuarioRequest.ValidarResultado.Adicionar(ex.Message);
                return -1;
            }

            gSUsuarioRequest.ValidarResultado.Adicionar("Usuário ou senha inválido.");
            return -1;
        }

        public bool Registrar(GSUsuarioRequest gSUsuarioRequest)
        {
            if (gSUsuarioRequest == null)
                return false;

            gSUsuarioRequest.ValidarResultado = new ValidarResultado();
            
            if (gSUsuarioRequest.Usuario.ObterValorOuPadrao("").Trim() == "")
            {
                gSUsuarioRequest.ValidarResultado.Adicionar("Usuário é um campo obrigatório.");
                return false;
            }
            else if (gSUsuarioRequest.Nome.ObterValorOuPadrao("").Trim() == "")
            {
                gSUsuarioRequest.ValidarResultado.Adicionar("Nome é um campo obrigatório.");
                return false;
            }
            else if (gSUsuarioRequest.Senha.ObterValorOuPadrao("").Trim() == "")
            {
                gSUsuarioRequest.ValidarResultado.Adicionar("Senha é um campo obrigatório.");
                return false;
            }

            var gSUsuarioExistente = gSUsuarioRepository.ObterLista("Usuario = @Usuario", new { Usuario = gSUsuarioRequest.Usuario }).FirstOrDefault();

            if (gSUsuarioExistente != null)
            {
                gSUsuarioRequest.ValidarResultado.Adicionar("Usuário já existe.");
                return false;
            }

            var criptografarRequest = new JJ.NET.Cryptography.CriptografiaRequest
            {
                TipoCriptografia = JJ.NET.Cryptography.Enumerador.TipoCriptografia.AES,
                Valor = gSUsuarioRequest.Senha,
                IV = "",
            };

            var criptografarResult = Criptografia.Criptografar(criptografarRequest);

            if (criptografarResult.Erro.ObterValorOuPadrao("").Trim() != "")
            {
                gSUsuarioRequest.ValidarResultado.Adicionar(criptografarResult.Erro);
                return false;
            }

            var gSUsuario = new GSUsuario
            {
                PK_GSUsuario = 0,
                Usuario = gSUsuarioRequest.Usuario.ObterValorOuPadrao("").Trim(),
                Nome = gSUsuarioRequest.Nome.ObterValorOuPadrao("").Trim(),
                Senha = criptografarResult.Valor.ObterValorOuPadrao("").Trim(),
                IVSenha = criptografarResult.IV.ObterValorOuPadrao("").Trim(),
                ValidarResultado = new ValidarResultado()
            };

            using (var uow = new UnitOfWork(ConfiguracaoBancoDados.ObterConexao()))
            {
                var _gSUsuarioRepository = new GSUsuarioRepository(uow);
                try
                {
                    uow.Begin();

                    int PK_GSUsuario = _gSUsuarioRepository.Adicionar(gSUsuario);

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
    }
}

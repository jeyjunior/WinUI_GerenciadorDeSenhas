using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Application;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Entidades;
using Application.Services;
using JJ.NET.Core.Extensoes;


namespace Presentation.Views
{
    public sealed partial class ConfiguracaoDialog : ContentDialog
    {
        #region Interfaces
        private readonly ILoginService loginService;
        private readonly INotificationService notificationService;
        private readonly IConfigAppService configAppService;
        #endregion

        #region Propriedades
        private GSUsuario gSUsuarioAtivo;
        #endregion

        #region Construtor
        public ConfiguracaoDialog()
        {
            this.InitializeComponent();

            loginService = Bootstrap.Container.GetInstance<ILoginService>();
            notificationService = Bootstrap.Container.GetInstance<INotificationService>();
            configAppService = Bootstrap.Container.GetInstance<IConfigAppService>();
        }
        #endregion
        #region Eventos
        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoUsuario(false);
            HabilitarEdicaoSenha(false);
            HabilitarEdicaoNome(false);

            Pesquisar();
        }
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
        private void btnAlterarNome_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoUsuario(false);
            HabilitarEdicaoSenha(false);

            HabilitarEdicaoNome(true);
        }
        private void btnCancelarNome_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoNome(false);
            txtNome.Text = gSUsuarioAtivo.Nome;
        }
        private void btnSalvarNome_Click(object sender, RoutedEventArgs e)
        {
            if (txtNome.Text.ObterValorOuPadrao("").Trim() == "")
                return;

            try
            {
                GSUsuario gSUsuario = gSUsuarioAtivo.DeepCopy();
                gSUsuario.Nome = txtNome.Text.ObterValorOuPadrao("").Trim();
                
                loginService.AtualizarUsuario(gSUsuario);
                
                if (!gSUsuario.ValidarResultado.EhValido)
                    notificationService.EnviarNotificacao(gSUsuario.ValidarResultado.ObterPrimeiroErro());
                
                HabilitarEdicaoNome(false);
                Pesquisar();
            }
            catch (Exception ex)
            {
                notificationService.EnviarNotificacao(ex.Message);
            }
        }
        private void btnAlterarUsuario_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoNome(false);
            HabilitarEdicaoSenha(false);

            HabilitarEdicaoUsuario(true);
        }
        private void btnCancelarUsuario_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoUsuario(false);
            txtUsuario.Text = gSUsuarioAtivo.Usuario;
        }
        private void btnSalvarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (txtUsuario.Text.ObterValorOuPadrao("").Trim() == "")
                return;

            try
            {
                GSUsuario gSUsuario = gSUsuarioAtivo.DeepCopy();
                gSUsuario.Usuario = txtUsuario.Text.ObterValorOuPadrao("").Trim();

                loginService.AtualizarUsuario(gSUsuario);

                if (!gSUsuario.ValidarResultado.EhValido)
                    notificationService.EnviarNotificacao(gSUsuario.ValidarResultado.ObterPrimeiroErro());

                HabilitarEdicaoUsuario(false);
                Pesquisar();
            }
            catch (Exception ex)
            {
                notificationService.EnviarNotificacao(ex.Message);
            }
        }
        private void btnAlterarSenha_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoNome(false);
            HabilitarEdicaoUsuario(false);

            HabilitarEdicaoSenha(true);
        }
        private void btnCancelarSenha_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoSenha(false);

            var criptografiaResult = configAppService.Descriptografar(gSUsuarioAtivo.Senha, gSUsuarioAtivo.IVSenha);
            passSenha.Password = criptografiaResult.Valor;
        }
        private void btnSalvarSenha_Click(object sender, RoutedEventArgs e)
        {
            if (passSenha.Password.ObterValorOuPadrao("").Trim() == "")
                return;

            try
            {
                GSUsuario gSUsuario = gSUsuarioAtivo.DeepCopy();
                gSUsuario.Senha = passSenha.Password.ObterValorOuPadrao("").Trim();
                gSUsuario.IVSenha = "";

                var criptografiaResult = configAppService.Criptografar(gSUsuario.Senha, gSUsuario.IVSenha);

                if (criptografiaResult.Erro.ObterValorOuPadrao("").Trim() != "")
                {
                    notificationService.EnviarNotificacao(criptografiaResult.Erro);
                    return;
                }

                gSUsuario.Senha = criptografiaResult.Valor;
                gSUsuario.IVSenha = criptografiaResult.IV;

                loginService.AtualizarUsuario(gSUsuario);

                if (!gSUsuario.ValidarResultado.EhValido)
                    notificationService.EnviarNotificacao(gSUsuario.ValidarResultado.ObterPrimeiroErro());

                HabilitarEdicaoSenha(false);
                Pesquisar();
            }
            catch (Exception ex)
            {
                notificationService.EnviarNotificacao(ex.Message);
            }
        }
        #endregion

        #region Metodos
        private void HabilitarEdicaoNome(bool habilitar)
        {
            btnAlterarNome.Visibility = (habilitar ? Visibility.Collapsed : Visibility.Visible);
            btnCancelarNome.Visibility = (habilitar ? Visibility.Visible : Visibility.Collapsed);
            btnSalvarNome.Visibility = (habilitar ? Visibility.Visible : Visibility.Collapsed);

            txtNome.IsEnabled = habilitar;
        }
        private void HabilitarEdicaoUsuario(bool habilitar)
        {
            btnAlterarUsuario.Visibility = (habilitar ? Visibility.Collapsed : Visibility.Visible);
            btnCancelarUsuario.Visibility = (habilitar ? Visibility.Visible : Visibility.Collapsed);
            btnSalvarUsuario.Visibility = (habilitar ? Visibility.Visible : Visibility.Collapsed);

            txtUsuario.IsEnabled = habilitar;
        }
        private void HabilitarEdicaoSenha(bool habilitar)
        {
            btnAlterarSenha.Visibility = (habilitar ? Visibility.Collapsed : Visibility.Visible);
            btnCancelarSenha.Visibility = (habilitar ? Visibility.Visible : Visibility.Collapsed);
            btnSalvarSenha.Visibility = (habilitar ? Visibility.Visible : Visibility.Collapsed);

            passSenha.IsEnabled = habilitar;
        }
        private void Pesquisar()
        {
            try
            {
                gSUsuarioAtivo = loginService.ObterUsuario(App.PK_GESUsuarioAtivo);

                if (gSUsuarioAtivo == null)
                {
                    notificationService.EnviarNotificacao("Não foi possível identificar o usuário ativo.");
                    this.Hide();
                }

                BindPrincipal();
            }
            catch (Exception ex)
            {
                notificationService.EnviarNotificacao(ex.Message);
            }
        }
        private void BindPrincipal()
        {
            txtNome.Text = gSUsuarioAtivo.Nome;
            txtUsuario.Text = gSUsuarioAtivo.Usuario;

            var criptografiaResult = configAppService.Descriptografar(gSUsuarioAtivo.Senha, gSUsuarioAtivo.IVSenha);
            passSenha.Password = criptografiaResult.Valor;
        }
        #endregion
    }
}

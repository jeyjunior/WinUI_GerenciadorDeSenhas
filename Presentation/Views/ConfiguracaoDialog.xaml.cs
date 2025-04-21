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


namespace Presentation.Views
{
    public sealed partial class ConfiguracaoDialog : ContentDialog
    {
        #region Interfaces
        private readonly ILoginService loginService;
        private readonly INotificationService notificationService;
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
        }

        private void btnSalvarNome_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoNome(false);
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
        }

        private void btnSalvarUsuario_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoUsuario(false);
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
        }

        private void btnSalvarSenha_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoSenha(false);
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
            passSenha.Password = gSUsuarioAtivo.Senha;
        }
        #endregion
    }
}

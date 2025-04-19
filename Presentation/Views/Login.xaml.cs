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
using JJ.NET.Core.Extensoes;
using Domain.Entidades;
using Application;
using Application.Interfaces;
using Application.Services;

namespace Presentation.Views
{
    public sealed partial class Login : Page
    {
        #region Interfaces
        private readonly ILoginService loginService;
        private readonly INotificationService notificationService;
        #endregion

        #region Construtor
        public Login()
        {
            this.InitializeComponent();

            loginService = Bootstrap.Container.GetInstance<ILoginService>();
            notificationService = Bootstrap.Container.GetInstance<INotificationService>();
        }
        #endregion

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var gSUsuarioRequest = new GSUsuarioRequest
                {
                    Usuario = txtUsuario.Text.ObterValorOuPadrao("").Trim(),
                    Senha = passBoxSenha.Password.ObterValorOuPadrao("").Trim()
                };

                int PK_GSUsuario = loginService.Entrar(gSUsuarioRequest);

                if (!gSUsuarioRequest.ValidarResultado.EhValido)
                {
                    txtUsuario.Focus(FocusState.Keyboard);
                    notificationService.EnviarNotificacao(gSUsuarioRequest.ValidarResultado.ObterPrimeiroErro());
                    return;
                }
                else if (PK_GSUsuario <= 0)
                {
                    txtUsuario.Focus(FocusState.Keyboard);
                    notificationService.EnviarNotificacao("Não foi possível logar.");
                    return;
                }

                App.PK_GESUsuarioAtivo = PK_GSUsuario;
                NavigationService.NavegarPara(typeof(Principal));
            }
            catch (Exception ex)
            {
                notificationService.EnviarNotificacao(ex.Message);
            }
        }

        private void passBoxSenha_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                btnEntrar_Click(null, null);
        }

        private async void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new RegistrarLoginDialog();
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                notificationService.EnviarNotificacao(ex.Message);
            }
        }
    }
}

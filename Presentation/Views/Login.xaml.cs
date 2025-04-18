using Application;
using Application.Interfaces;
using Application.Services;
using Domain.Entidades;
using JJ.NET.Core.Extensoes;
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

        private void btnLoginGoogle_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            var gSUsuarioRequest = new GSUsuarioRequest
            {
                Usuario = txtUsuario.Text.ObterValorOuPadrao("").Trim(),
                Senha = passBoxSenha.Password.ObterValorOuPadrao("").Trim()
            };

            int PK_GSUsuario = loginService.Entrar(gSUsuarioRequest);

            if (!gSUsuarioRequest.ValidarResultado.EhValido)
            {
                notificationService.EnviarNotificacao(gSUsuarioRequest.ValidarResultado.ObterPrimeiroErro());
                return;
            }
            else if (PK_GSUsuario <= 0)
            {
                notificationService.EnviarNotificacao("Não foi possível logar.");
                return;
            }

            App.PK_GESUsuarioAtivo = PK_GSUsuario;
            NavigationService.NavegarPara(typeof(Principal));
        }

        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            var gSUsuarioRequest = new GSUsuarioRequest
            {
                Nome = "José Junior",
                Senha = "Teste@123",
                Usuario = "jeyjunior"
            };

            var ret = loginService.Registrar(gSUsuarioRequest);

            if (!gSUsuarioRequest.ValidarResultado.EhValido)
            {
                notificationService.EnviarNotificacao(gSUsuarioRequest.ValidarResultado.ObterPrimeiroErro());
                return;
            }
            else if (!ret)
            {
                notificationService.EnviarNotificacao("Não foi possível registrar.");
                return;
            }

            notificationService.EnviarNotificacao("Usuário registrado com sucesso.");
        }
    }
}

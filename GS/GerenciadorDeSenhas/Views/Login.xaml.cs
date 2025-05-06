
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using JJ.NET.Core.Extensoes;
using GSApplication;
using GSApplication.Interfaces;
using GSApplication.Services;
using GSDomain.Entidades;
using GerenciadorDeSenhas.Controls;

namespace GerenciadorDeSenhas.Views
{
    public sealed partial class Login : Page
    {
        #region Interfaces
        private readonly ILoginService loginService;
        #endregion

        #region Construtor
        public Login()
        {
            this.InitializeComponent();

            loginService = Bootstrap.Container.GetInstance<ILoginService>();
        }
        #endregion

        #region Eventos
        private async void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtUsuario.Text.ObterValorOuPadrao("").Trim() == "")
                {
                    txtUsuario.Focus(FocusState.Keyboard);
                    await Mensagem.AvisoAsync("Usuário inválido.", this.XamlRoot);
                    return;
                }
                else if (passBoxSenha.Password.ObterValorOuPadrao("").Trim() == "")
                {
                    passBoxSenha.Focus(FocusState.Keyboard);
                    await Mensagem.AvisoAsync("Senha inválida.", this.XamlRoot);
                    return;
                }

                var gSUsuarioRequest = new GSUsuarioRequest
                {
                    Usuario = txtUsuario.Text.ObterValorOuPadrao("").Trim(),
                    Senha = passBoxSenha.Password.ObterValorOuPadrao("").Trim()
                };

                int PK_GSUsuario = loginService.Entrar(gSUsuarioRequest);

                if (!gSUsuarioRequest.ValidarResultado.EhValido)
                {
                    txtUsuario.Focus(FocusState.Keyboard);
                    await Mensagem.ErroAsync(gSUsuarioRequest.ValidarResultado.ObterPrimeiroErro(), this.XamlRoot);
                    return;
                }
                else if (PK_GSUsuario <= 0)
                {
                    txtUsuario.Focus(FocusState.Keyboard);
                    await Mensagem.AvisoAsync("Não foi possível logar.", this.XamlRoot);
                    return;
                }

                loginService.DeletarUsuarioLembrado();

                if (chkLembrarUsuario.IsChecked == true)
                    loginService.SalvarUsuarioLembrado(gSUsuarioRequest.Usuario);

                App.PK_GESUsuarioAtivo = PK_GSUsuario;
                NavigationService.NavegarPara(typeof(Principal));
            }
            catch (Exception ex)
            {
                await Mensagem.ErroAsync(ex.Message, this.XamlRoot);
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
                await Mensagem.ErroAsync(ex.Message, this.XamlRoot);
            }
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var usuario = await loginService.ObterUsuarioLembrado();

            if (usuario.ObterValorOuPadrao("").Trim() != "")
            {
                txtUsuario.Text = usuario;
                txtUsuario.SelectAll();
                chkLembrarUsuario.IsChecked = true;
            }
        }
        private async void hlbEsqueciSenha_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new EsqueciSenhaDialog();
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                await Mensagem.ErroAsync(ex.Message, this.XamlRoot);
            }
        }
        #endregion

        #region Metodos
        #endregion
    }
}

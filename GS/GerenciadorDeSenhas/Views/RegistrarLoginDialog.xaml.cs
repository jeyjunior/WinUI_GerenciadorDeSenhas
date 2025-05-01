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
using JJ.NET.Core.DTO;
using GSApplication;
using GSApplication.Interfaces;
using GSApplication.Services;
using GSDomain.Entidades;
using GSDomain.Enumeradores;
using GerenciadorDeSenhas.ViewModel;
using GerenciadorDeSenhas.Controls;


namespace GerenciadorDeSenhas.Views
{
    public sealed partial class RegistrarLoginDialog : ContentDialog
    {
        #region Interfaces
        private readonly ILoginService loginService;
        #endregion

        #region Propriedades
        private int qtdMinimaNome = 3;
        private int qtdMinimaUsuario = 3;
        private int qtdMinimaSenha = 3;
        #endregion

        #region Construtor
        public RegistrarLoginDialog()
        {
            this.InitializeComponent();

            loginService = Bootstrap.Container.GetInstance<ILoginService>();
        }
        #endregion

        #region Eventos
        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private async void SalvarCredencial_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                if (txtNome.Text.ObterValorOuPadrao("").Trim() == "" || txtNome.Text.Length < qtdMinimaNome)
                {
                    args.Cancel = true;
                    txtNome.Focus(FocusState.Keyboard);
                    AlterarIconeValidacao(true, fontIconNome);
                    await Mensagem.AvisoAsync("Nome de usuário inválido.", this.XamlRoot);
                    return;
                }
                else if (txtUsuario.Text.ObterValorOuPadrao("").Trim() == "" || txtUsuario.Text.Length < qtdMinimaUsuario)
                {
                    args.Cancel = true;
                    txtUsuario.Focus(FocusState.Keyboard);
                    AlterarIconeValidacao(true, fontIconUsuario);
                    await Mensagem.AvisoAsync("Usuário inválido.", this.XamlRoot);
                    return;
                }
                else if (passBoxSenha.Password.ObterValorOuPadrao("").Trim() == "" || passBoxSenha.Password.Length < qtdMinimaSenha)
                {
                    args.Cancel = true;
                    passBoxSenha.Focus(FocusState.Keyboard);
                    AlterarIconeValidacao(true, fontIconSenha);
                    await Mensagem.AvisoAsync("Senha inválido.", this.XamlRoot);
                    return;
                }

                var gSUsuarioRequest = new GSUsuarioRequest
                {
                    Nome = txtNome.Text.Trim(),
                    Usuario = txtUsuario.Text.Trim(),
                    Senha = passBoxSenha.Password.Trim()
                };

                var ret = loginService.Registrar(gSUsuarioRequest);

                if (!gSUsuarioRequest.ValidarResultado.EhValido)
                {
                    args.Cancel = true;
                    await Mensagem.AvisoAsync(gSUsuarioRequest.ValidarResultado.ObterPrimeiroErro(), this.XamlRoot);
                    return;
                }
                else if (!ret)
                {
                    args.Cancel = true;
                    await Mensagem.AvisoAsync("Não foi possível registrar.", this.XamlRoot);
                    return;
                }

                await Mensagem.SucessoAsync("Usuário registrado com sucesso.", this.XamlRoot);
            }
            catch (Exception ex)
            {
                await Mensagem.SucessoAsync(ex.Message, this.XamlRoot);
            }
        }
        private void txtNome_TextChanged(object sender, TextChangedEventArgs e)
        {
            fontIconNome.Visibility = Visibility.Collapsed;

            if (txtNome.Text.Length <= 0)
                return;

            AlterarIconeValidacao((txtNome.Text.Length < qtdMinimaNome), fontIconNome);
        }
        private void txtUsuario_TextChanged(object sender, TextChangedEventArgs e)
        {
            fontIconUsuario.Visibility = Visibility.Collapsed;

            if (txtUsuario.Text.Length <= 0)
                return;

            AlterarIconeValidacao((txtUsuario.Text.Length < qtdMinimaUsuario), fontIconUsuario);
        }
        private void passBoxSenha_PasswordChanged(object sender, RoutedEventArgs e)
        {
            fontIconSenha.Visibility = Visibility.Collapsed;

            if (passBoxSenha.Password.Length <= 0)
                return;

            AlterarIconeValidacao((passBoxSenha.Password.Length < qtdMinimaSenha), fontIconSenha);
        }
        #endregion

        #region Metodos
        private void AlterarIconeValidacao(bool exibirIconeAlerta, FontIcon fontIcon)
        {
            if (fontIcon == null)
                return;

            if (exibirIconeAlerta)
            {
                fontIcon.Glyph = "\uE783";
                fontIcon.Foreground = (Brush)App.Current.Resources["Amarelo"];
            }
            else
            {
                fontIcon.Glyph = "\uE73E";
                fontIcon.Foreground = (Brush)App.Current.Resources["Verde1"];
            }

            fontIcon.Visibility = Visibility.Visible;
        }
        #endregion
    }
}

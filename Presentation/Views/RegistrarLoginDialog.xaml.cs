using Application;
using Application.Interfaces;
using Domain.Entidades;
using JJ.NET.Core.Extensoes;
using Microsoft.UI;
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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Presentation.Views
{
    public sealed partial class RegistrarLoginDialog : ContentDialog
    {
        #region Interfaces
        private readonly ILoginService loginService;
        private readonly INotificationService notificationService;
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
            notificationService = Bootstrap.Container.GetInstance<INotificationService>();
        }
        #endregion

        #region Eventos
        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void SalvarCredencial_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                if (txtNome.Text.ObterValorOuPadrao("").Trim() == "" || txtNome.Text.Length < qtdMinimaNome)
                {
                    txtNome.Focus(FocusState.Keyboard);
                    AlterarIconeValidacao(true, fontIconNome);
                    notificationService.EnviarNotificacao("Nome de usuário inválido.");
                    args.Cancel = true;
                    return;
                }
                else if (txtUsuario.Text.ObterValorOuPadrao("").Trim() == "" || txtUsuario.Text.Length < qtdMinimaUsuario)
                {
                    txtUsuario.Focus(FocusState.Keyboard);
                    AlterarIconeValidacao(true, fontIconUsuario);
                    notificationService.EnviarNotificacao("Usuário inválido.");
                    args.Cancel = true;
                    return;
                }
                else if (passBoxSenha.Password.ObterValorOuPadrao("").Trim() == "" || passBoxSenha.Password.Length < qtdMinimaSenha)
                {
                    passBoxSenha.Focus(FocusState.Keyboard);
                    AlterarIconeValidacao(true, fontIconSenha);
                    notificationService.EnviarNotificacao("Senha inválido.");
                    args.Cancel = true;
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
                    notificationService.EnviarNotificacao(gSUsuarioRequest.ValidarResultado.ObterPrimeiroErro());
                    args.Cancel = true;
                    return;
                }
                else if (!ret)
                {
                    notificationService.EnviarNotificacao("Não foi possível registrar.");
                    args.Cancel = true;
                    return;
                }

                notificationService.EnviarNotificacao("Usuário registrado com sucesso.");
            }
            catch (Exception ex)
            {
                notificationService.EnviarNotificacao(ex.Message);
            }
        }
        #endregion

        #region Metodos

        #endregion

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
    }
}

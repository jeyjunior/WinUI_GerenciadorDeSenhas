using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Application.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using JJ.NET.Core.DTO;
using JJ.NET.Core.Extensoes;
using Presentation.ViewModel;
using Domain.Entidades;
using Domain.Enumeradores;
using Application;
using Application.Interfaces;

namespace Presentation.Views
{
    public sealed partial class Principal : Page
    {
        #region Interfaces
        private readonly ICredencialAppService credencialAppService;
        private readonly INotificationService notificationService;
        #endregion

        #region Propriedades
        private List<GSCredencial> gSCredencials;
        private DirecaoOrdenacao direcaoOrdenacao;
        private MainWindowViewModel ViewModel;
        #endregion

        #region Construtor
        public Principal()
        {
            this.InitializeComponent();

            credencialAppService = Bootstrap.Container.GetInstance<ICredencialAppService>();
            notificationService = Bootstrap.Container.GetInstance<INotificationService>();

            ViewModel = new MainWindowViewModel();

            this.cboTipoDeOrdenacao.DataContext = ViewModel;
            this.cboTipoDePesquisa.DataContext = ViewModel;
            this.listaCredenciais.DataContext = ViewModel;

            Load();
        }
        #endregion

        #region Eventos
        private void btnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            Pesquisar();
        }
        private async void btnOrdenar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OrdenarLista();
                BindPrincipal();
            }
            catch (Exception ex)
            {
                await notificationService.ExibirErroAsync(ex.Message, this.Content.XamlRoot);
            }
        }
        private async void btnCopiarCredencial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button == null)
                    return;

                var credencialViewModel = ObterCredencialViewModel(sender);
                if (credencialViewModel == null)
                    return;

                CopiarParaClipboard(credencialViewModel.Credencial);

                await AlterarIconeBtn("\uE73E", "\uE8C8", button);
            }
            catch (Exception ex)
            {
                await notificationService.ExibirErroAsync(ex.Message, this.Content.XamlRoot);
            }
        }
        private async void btnExibirSenha_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button == null)
                    return;

                var credencialViewModel = ObterCredencialViewModel(sender);

                if (credencialViewModel == null)
                    return;

                var credencial = gSCredencials.Where(i => i.PK_GSCredencial == credencialViewModel.PK_GSCredencial).FirstOrDefault();

                credencialViewModel.ExibirSenha = !credencialViewModel.ExibirSenha;

                if (credencialViewModel.ExibirSenha)
                {
                    credencialViewModel.Senha = credencialAppService.Descriptografar(credencial.Senha, credencial.IVSenha);
                    credencialViewModel.BotaoStyle = (Style)App.Current.Resources["DefaultButtonStyle"];

                    if (button.Content is FontIcon icon)
                        icon.Glyph = "\uE7B3";
                }
                else
                {
                    credencialViewModel.Senha = this.OcultarSenha(credencial.Senha, credencial.IVSenha);
                    credencialViewModel.BotaoStyle = (Style)App.Current.Resources["AlternateCloseButtonStyle"];
                    
                    if (button.Content is FontIcon icon)
                        icon.Glyph = "\uE890";
                }
            }
            catch (Exception ex)
            {
                await notificationService.ExibirErroAsync(ex.Message, this.Content.XamlRoot);
            }
        }
        private async void btnCopiarSenha_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button == null)
                    return;

                var credencialViewModel = ObterCredencialViewModel(sender);

                if (credencialViewModel == null)
                    return;

                var gSCredencial = gSCredencials.Where(i => i.PK_GSCredencial.Equals(credencialViewModel.PK_GSCredencial)).FirstOrDefault();

                if (gSCredencials == null)
                    return;

                var senha = credencialAppService.Descriptografar(gSCredencial.Senha, gSCredencial.IVSenha);

                if (senha.ObterValorOuPadrao("").Trim() == "")
                    return;

                CopiarParaClipboard(senha);

                await AlterarIconeBtn("\uE73E", "\uE8C8", button);
            }
            catch (Exception ex)
            {
                await notificationService.ExibirErroAsync(ex.Message, this.Content.XamlRoot);
            }
        }
        private async void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new ContentDialog
                {
                    Title = "Confirmação",
                    Content = "Deseja excluir?",
                    PrimaryButtonText = "Sim",
                    CloseButtonText = "Não",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.Content.XamlRoot
                };

                var result = await dialog.ShowAsync();

                if (result != ContentDialogResult.Primary)
                    return;

                var credencial = ObterCredencialViewModel(sender);
                if (credencial == null)
                {
                    notificationService.EnviarNotificacao("Não foi possível encontrar credencial para excluir.");
                    return;
                }

                var ret = credencialAppService.DeletarCredencial(credencial.PK_GSCredencial);

                if (!ret)
                {
                    notificationService.EnviarNotificacao("Não foi possível deletar credencial.");
                    return;
                }

                ViewModel.Credenciais.Remove(credencial);
}
            catch (Exception ex)
            {
                await notificationService.ExibirErroAsync(ex.Message, this.Content.XamlRoot);
            }
            finally
            {
                AtualizarStatus();
            }
        }
        private void btnAlterar_Click(object sender, RoutedEventArgs e)
        {
            AbrirTelaAdicionarCredencial(sender);
        }
        private void btnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            AbrirTelaAdicionarCredencial();
        }
        private void txtPesquisa_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                btnPesquisar_Click(null, null);
        }
        #endregion

        #region Metodos
        private void Load()
        {
            ViewModel.TipoDeOrdenacao = credencialAppService.ObterTipoDeOrdenacao();
            ViewModel.TipoDePesquisa = credencialAppService.ObterTipoDePesquisa();

            ViewModel.SelecionarTipoDeOrdenacao(0);
            ViewModel.SelecionarTipoDePesquisa(0);

            Pesquisar();

            txtPesquisa.Focus(FocusState.Keyboard);
        }
        private async void Pesquisar()
        {
            try
            {
                Item tipoDeOrdenacao = this.ViewModel.TipoDeOrdenacaoSelecionado;
                Item tipoDePesquisa = this.ViewModel.TipoDePesquisaSelecionado;

                var requisicao = new GSCredencialPesquisaRequest
                {
                    Valor = txtPesquisa.Text.ObterValorOuPadrao(""),
                    TipoDePesquisa = (TipoDePesquisa)tipoDePesquisa.ID.ConverterParaInt32(0),
                    TipoDeOrdenacao = (TipoDeOrdenacao)tipoDeOrdenacao.ID.ConverterParaInt32(0)
                };

                gSCredencials = credencialAppService.Pesquisar(requisicao).ToList();

                OrdenarLista();
                BindPrincipal();
                AtualizarStatus();

                direcaoOrdenacao = DirecaoOrdenacao.Crescente;
            }
            catch (Exception ex)
            {
                await notificationService.ExibirErroAsync(ex.Message, this.Content.XamlRoot);
            }
        }
        private void BindPrincipal()
        {
            ViewModel.Credenciais.Clear();

            if (gSCredencials != null && gSCredencials.Count() > 0)
            {
                ViewModel.Credenciais = new ObservableCollection<CredencialViewModel>(
                    gSCredencials.Select(item => new CredencialViewModel
                    {
                        PK_GSCredencial = item.PK_GSCredencial,
                        Categoria = item.GSCategoria?.Categoria ?? "",
                        Modificacao = item.DataModificacao?.ToShortDateString() ?? "",
                        Credencial = item.Credencial,
                        Senha = OcultarSenha(item.Senha, item.IVSenha),
                        ExibirSenha = false,
                        BotaoStyle = (Style)App.Current.Resources["AlternateCloseButtonStyle"]
                    })
                    .ToList()
                );
            }
        }
        private string OcultarSenha(string senha, string iv)
        {
            return credencialAppService.Descriptografar(senha, iv).Trim().Ocultar();
        }
        private void OrdenarLista()
        {
            var tipoOrdenacao = ObterTipoDeOrdenacaoSelecionada(ViewModel.TipoDeOrdenacaoSelecionado);

            Func<GSCredencial, object> keySelector;

            switch (tipoOrdenacao)
            {
                case TipoDeOrdenacao.Cadastro:
                    keySelector = cred => cred.DataCriacao;
                    break;
                case TipoDeOrdenacao.Modificação:
                    keySelector = cred => cred.DataModificacao ?? DateTime.MinValue;
                    break;
                case TipoDeOrdenacao.Categoria:
                    keySelector = cred => cred.GSCategoria?.Categoria ?? string.Empty;
                    break;
                case TipoDeOrdenacao.Credencial:
                    keySelector = cred => cred.Credencial;
                    break;
                default:
                    keySelector = cred => cred.DataCriacao;
                    break;
            }

            gSCredencials = direcaoOrdenacao == DirecaoOrdenacao.Crescente
                ? gSCredencials.OrderBy(keySelector).ToList()
                : gSCredencials.OrderByDescending(keySelector).ToList();

            direcaoOrdenacao = direcaoOrdenacao == DirecaoOrdenacao.Crescente
                ? DirecaoOrdenacao.Decrescente
                : DirecaoOrdenacao.Crescente;
        }
        public TipoDeOrdenacao ObterTipoDeOrdenacaoSelecionada(Item? item)
        {
            return (TipoDeOrdenacao)item.ID.ObterValorOuPadrao(0);
        }
        private void AtualizarStatus()
        {
            txtStatus.Text = $"Credenciais: " + ViewModel.Credenciais.Count().ToString("N0");
        }
        private void CopiarParaClipboard(string texto)
        {
            var pacote = new DataPackage();
            pacote.SetText(texto);
            Clipboard.SetContent(pacote);
        }
        private CredencialViewModel ObterCredencialViewModel(object sender)
        {
            if (sender is not Button)
                return null;

            Button btn = (Button)sender;

            CredencialViewModel gSCredencial = (CredencialViewModel)btn.DataContext;

            if (gSCredencial == null)
                return null;

            return gSCredencial;
        }
        private async Task AlterarIconeBtn(string iconeInicial, string iconeFinal, Button button)
        {
            var icon = button.Content as FontIcon;
            if (icon != null)
                icon.Glyph = iconeInicial;

            button.IsEnabled = false;

            await Task.Delay(2000);

            if (icon != null)
                icon.Glyph = iconeFinal;

            button.IsEnabled = true;
        }
        private async void AbrirTelaAdicionarCredencial(object credencial = null)
        {
            AdicionarCredencialDialog dialog = null;
            GSCredencial? gSCredencial = null;

            var credencialViewModel = ObterCredencialViewModel(credencial);
            if (credencialViewModel != null)
                gSCredencial = credencialAppService.PesquisarPorID(credencialViewModel.PK_GSCredencial);

            dialog = new AdicionarCredencialDialog(gSCredencial);
            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();

            Pesquisar();
        }
        #endregion
    }
}

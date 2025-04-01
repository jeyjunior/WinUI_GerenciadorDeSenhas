using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.ApplicationModel.DataTransfer;
using Presentation.ViewModel;
using Windows.UI.Notifications;
using JJ.NET.Core.DTO;
using JJ.NET.Core.Extensoes;
using JJ.NET.Cryptography;
using Domain.Entidades;
using Domain.Enumeradores;
using Application;
using Application.Interfaces;

namespace Presentation
{
    public sealed partial class MainWindow : Window
    {
        #region Interfaces
        private readonly ICredencialAppService credencialAppService;
        #endregion

        #region Propriedades
        private IEnumerable<GSCredencial> gSCredencials;
        private DirecaoOrdenacao direcaoOrdenacao;
        private MainWindowViewModel ViewModel;
        #endregion

        #region Construtor 
        public MainWindow()
        {
            this.InitializeComponent();

            credencialAppService = Bootstrap.Container.GetInstance<ICredencialAppService>();

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
        private void btnOrdenar_Click(object sender, RoutedEventArgs e)
        {
            OrdenarLista();
            BindPrincipal();
        }
        private void btnCopiarCredencial_Click(object sender, RoutedEventArgs e)
        {
            var credencialViewModel = ObterCredencialViewModel(sender);

            if (credencialViewModel == null)
                return;

            CopiarParaClipboard(credencialViewModel.Credencial);
        }
        private void btnExibirSenha_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnCopiarSenha_Click(object sender, RoutedEventArgs e)
        {
            var credencialViewModel = ObterCredencialViewModel(sender);

            if (credencialViewModel == null)
                return;

            var gSCredencial = gSCredencials.Where(i => i.PK_GSCredencial.Equals(credencialViewModel.PK_GSCredencial)).FirstOrDefault();

            if (gSCredencials == null)
                return;

            var descriptografarRequest = new DescriptografarRequest { Valor = gSCredencial.Senha, IV = gSCredencial.IVSenha, TipoCriptografia = JJ.NET.Cryptography.Enumerador.TipoCriptografia.AES };
            var senha = Criptografia.Descriptografar(descriptografarRequest);

            if (senha == null)
                return;

            if (senha.Erro.Length > 0)
            {
                EnviarNotificacao("Erro", senha.Erro);
                return;
            }

            CopiarParaClipboard(senha.Valor.ObterValorOuPadrao("").Trim());
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

                gSCredencials = credencialAppService.Pesquisar(requisicao);
                
                OrdenarLista();
                BindPrincipal();
                AtualizarStatus();

                direcaoOrdenacao = DirecaoOrdenacao.Crescente;
            }
            catch (Exception ex)
            {
                await ExibirErro(ex.Message);
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
                        Senha = OcultarSenha(item.Senha, item.IVSenha)//.Ocultar()
                    }).ToList()
                );
            }
        }

        private string OcultarSenha(string senha, string iv)
        {
            var descriptografarRequest = new DescriptografarRequest { Valor = senha, IV = iv, TipoCriptografia = JJ.NET.Cryptography.Enumerador.TipoCriptografia.AES };
            var ret = Criptografia.Descriptografar(descriptografarRequest);

            if (ret == null || ret.Erro.Length > 0)
                return "";

            return ret.Valor.ObterValorOuPadrao("").Trim().Ocultar();
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
                ? gSCredencials.OrderBy(keySelector)
                : gSCredencials.OrderByDescending(keySelector);

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
            txtStatus.Text = $"Credenciais: " + gSCredencials.Count().ToString("N0");
        }
        private async Task ExibirErro(string mensagem)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Erro",
                Content = mensagem,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot 
            };

            await dialog.ShowAsync();
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
        private void EnviarNotificacao(string titulo, string mensagem)
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            var toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(titulo));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(mensagem));

            var toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Presentation.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Application.Interfaces;
using Application;
using System.Collections.ObjectModel;
using Domain.Entidades;
using Domain.Enumeradores;
using JJ.NET.Core.DTO;
using JJ.NET.Core.Extensoes;

namespace Presentation
{
    public sealed partial class MainWindow : Window
    {
        #region Interfaces
        private readonly ICredencialAppService credencialAppService;
        #endregion

        #region Propriedades
        private IEnumerable<GSCredencial> gSCredencials;
        private TipoDeOrdenacao ultimaOrdenacao;
        private DirecaoOrdenacao direcaoOrdenacao;
        #endregion

        private MainWindowViewModel ViewModel;
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

        private void Load()
        {
            ViewModel.TipoDeOrdenacao = credencialAppService.ObterTipoDeOrdenacao();
            ViewModel.TipoDePesquisa = credencialAppService.ObterTipoDePesquisa();

            ViewModel.SelecionarTipoDeOrdenacao(0);
            ViewModel.SelecionarTipoDePesquisa(0);
        }

        private void btnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            Pesquisar();
        }

        private void Pesquisar()
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
                BindPrincipal();
                // AtualizarUltimaOrdenacao();
                // AtualizarStatus();

                direcaoOrdenacao = DirecaoOrdenacao.Crescente;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }
        private void BindPrincipal()
        {
            ViewModel.Credenciais.Clear();

            if (gSCredencials != null && gSCredencials.Count() > 0)
            {
                foreach (var item in gSCredencials)
                {
                    ViewModel.Credenciais.Add(new CredencialViewModel
                    {
                        Categoria = item.GSCategoria?.Categoria ?? "",
                        Modificacao = item.DataModificacao?.ToShortDateString() ?? "",
                        Credencial = item.Credencial,
                        Senha = "Teste"//OcultarSenha(item.Senha, item.IVSenha)
                    });
                }

            }
        }
    }
}

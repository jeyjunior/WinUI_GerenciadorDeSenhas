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

namespace Presentation
{
    public sealed partial class MainWindow : Window
    {
        #region Interfaces
        private readonly ICredencialAppService credencialAppService;
        #endregion

        private MainWindowViewModel ViewModel;
        public MainWindow()
        {
            this.InitializeComponent();

            credencialAppService = Bootstrap.Container.GetInstance<ICredencialAppService>();

            ViewModel = new MainWindowViewModel();

            this.cboTipoDeOrdenacao.DataContext = ViewModel;
            this.cboTipoDePesquisa.DataContext = ViewModel;

            Load();
        }

        private void Load()
        {
            ViewModel.TipoDeOrdenacao = credencialAppService.ObterTipoDeOrdenacao();
            ViewModel.TipoDePesquisa = credencialAppService.ObterTipoDePesquisa();

            ViewModel.SelecionarTipoDeOrdenacao(0);
            ViewModel.SelecionarTipoDePesquisa(0);
        }
    }
}

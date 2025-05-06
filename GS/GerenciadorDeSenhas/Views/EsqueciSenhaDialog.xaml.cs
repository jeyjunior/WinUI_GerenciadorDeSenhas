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
    public sealed partial class EsqueciSenhaDialog : ContentDialog
    {
        #region Interfaces
        #endregion

        #region Propriedades

        #endregion

        #region Construtor
        public EsqueciSenhaDialog()
        {
            this.InitializeComponent();

        }
        #endregion

        #region Eventos
        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Metodos
        #endregion
    }
}

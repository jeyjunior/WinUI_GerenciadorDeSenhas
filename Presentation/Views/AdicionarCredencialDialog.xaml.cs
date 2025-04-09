using Application;
using Application.Interfaces;
using JJ.NET.Core.Extensoes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Presentation.ViewModel;
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
    public sealed partial class AdicionarCredencialDialog : ContentDialog
    {
        #region Interfaces
        private readonly ICredencialAppService credencialAppService;
        #endregion

        #region Propriedades
        private AdicionarCredencialDialogViewModel ViewModel;
        #endregion

        #region Construtor
        public AdicionarCredencialDialog()
        {
            this.InitializeComponent();

            credencialAppService = Bootstrap.Container.GetInstance<ICredencialAppService>();

            ViewModel = new AdicionarCredencialDialogViewModel();

            this.cboCategoria.DataContext = ViewModel;
        }
        #endregion

        #region Eventos
        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Categoria = credencialAppService.ObterCategoriasObservableCollection();

            var gSCategoria = ViewModel.Categoria.FirstOrDefault();
            ViewModel.SelecionarCategoria(gSCategoria.PK_GSCategoria);
        }
        private async void btnConfigCategoria_Click(object sender, RoutedEventArgs e)
        {
            bool isVisible = CategoriaExpander.Visibility == Visibility.Visible;

            CategoriaExpander.Visibility = isVisible ? Visibility.Collapsed : Visibility.Visible;

            if (btnConfigCategoria.Content is FontIcon icon)
                icon.Glyph = ObterIconeCampoExpandido(isVisible);

            await Task.Delay(50);

            MoverScrollParaAreaExpandida(spPrincipal.TransformToVisual(MainScrollViewer));
        }
        private async void btnConfigCredencial_Click(object sender, RoutedEventArgs e)
        {
            bool isVisible = CredencialExpander.Visibility == Visibility.Visible;

            CredencialExpander.Visibility = isVisible ? Visibility.Collapsed : Visibility.Visible;

            if (btnConfigCredencial.Content is FontIcon icon)
                icon.Glyph = ObterIconeCampoExpandido(isVisible);

            await Task.Delay(50);

            MoverScrollParaAreaExpandida(spCredencial.TransformToVisual(MainScrollViewer));
        }
        private async void btnConfigSenha_Click(object sender, RoutedEventArgs e)
        {
            bool isVisible = SenhaExpander.Visibility == Visibility.Visible;

            SenhaExpander.Visibility = isVisible ? Visibility.Collapsed : Visibility.Visible;

            if (btnConfigSenha.Content is FontIcon icon)
                icon.Glyph = ObterIconeCampoExpandido(isVisible);

            await Task.Delay(50);

            MoverScrollParaAreaExpandida(btnGerarSenha.TransformToVisual(MainScrollViewer));
        }
        private async void btnExcluirCategoria_Click(object sender, RoutedEventArgs e)
        {
            
        }
        #endregion

        #region Metodos
        // Scroll behavior
        private void MoverScrollParaAreaExpandida(GeneralTransform transform)
        {
            Point position = transform.TransformPoint(new Point(0, 0));

            MainScrollViewer.ChangeView(null, position.Y, null, true);
        }
        private string ObterIconeCampoExpandido(bool expandido)
        {
            return (expandido) ? "\uE972" : "\uE971";
        }
        #endregion
    }
}

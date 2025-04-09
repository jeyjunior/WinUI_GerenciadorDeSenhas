using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Application;
using Application.Interfaces;
using Domain.Entidades;
using JJ.NET.Core.Extensoes;
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

        private void btnGerarCredencial_Click(object sender, RoutedEventArgs e)
        {
            var incluirMinusculas = chkLetraMinuscula.IsChecked == true;
            var incluirMaiusculas = chkLetraMaiuscula.IsChecked == true;
            var incluirNumeros = chkNumeros.IsChecked == true;
            var incluirSimbolos = chkSimbolos.IsChecked == true;

            var caracteres = "";

            if (incluirMinusculas)
                caracteres += ObterLetras(true);

            if (incluirMaiusculas)
                caracteres += ObterLetras(false);

            if (incluirNumeros)
                caracteres += ObterNumeros();

            if (incluirSimbolos)
                caracteres += ObterSimbolos();

            int min = (int)nbQuantidadeMinimaCredencial.Value;
            int max = (int)nbQuantidadeMaximaCredencial.Value;

            int tamanho = new Random().Next(min, max + 1);
            var random = new Random();
            var credencial = new StringBuilder();

            for (int i = 0; i < tamanho; i++)
            {
                int index = random.Next(caracteres.Length);
                credencial.Append(caracteres[index]);
            }

            txtCredencial.Text = credencial.ToString();
        }

        private void nbQuantidadeMaximaCredencial_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (double.IsNaN(sender.Value))
            {
                sender.Value = sender.Minimum;
            }
        }

        private void nbQuantidadeMinimaCredencial_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (double.IsNaN(sender.Value))
            {
                sender.Value = sender.Minimum;
            }
        }

        private string ObterLetras(bool minuscula)
        {
            string caracteres = "abcdefghijklmnopqrstuvwxyz";

            if (minuscula)
                return caracteres.ToLower();

            return caracteres.ToUpper();
        }

        private string ObterNumeros()
        {
            return "0123456789";
        }

        private string ObterSimbolos()
        {
            return "@#$&*_-";
        }

        private void btnGerarSenha_Click(object sender, RoutedEventArgs e)
        {
            var incluirMinusculas = chkLetraMinusculaSenha.IsChecked == true;
            var incluirMaiusculas = chkLetraMaiusculaSenha.IsChecked == true;
            var incluirNumeros = chkNumerosSenha.IsChecked == true;
            var incluirSimbolos = chkSimbolosSenha.IsChecked == true;

            var caracteres = "";

            if (incluirMinusculas)
                caracteres += ObterLetras(true);

            if (incluirMaiusculas)
                caracteres += ObterLetras(false);

            if (incluirNumeros)
                caracteres += ObterNumeros();

            if (incluirSimbolos)
                caracteres += ObterSimbolos();

            int min = (int)nbQuantidadeMinimaSenha.Value;
            int max = (int)nbQuantidadeMaximaSenha.Value;

            int tamanho = new Random().Next(min, max + 1);
            var random = new Random();
            var senha = new StringBuilder();

            for (int i = 0; i < tamanho; i++)
            {
                int index = random.Next(caracteres.Length);
                senha.Append(caracteres[index]);
            }

            txtSenha.Text = senha.ToString();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var categoria = ViewModel.CategoriaSelecionada;

            string credencial = txtCredencial.Text.ObterValorOuPadrao("").Trim();
            string senha = txtSenha.Text.ObterValorOuPadrao("").Trim();

            var gSCredencial = new GSCredencial
            {
                Credencial = credencial,
                Senha = senha,
                FK_GSCategoria = categoria.PK_GSCategoria,
            };

            var ret = credencialAppService.SalvarCredencial(gSCredencial);

            if ((int)ret > 0)
            {
                
            }
        }
    }
}

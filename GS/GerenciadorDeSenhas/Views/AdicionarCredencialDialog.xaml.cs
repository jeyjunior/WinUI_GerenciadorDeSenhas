using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using JJ.NET.Core.DTO;
using JJ.NET.Core.Extensoes;
using JJ.NET.Core.Enumeradores;
using GSApplication;
using GSApplication.Interfaces;
using GSApplication.Services;
using GSDomain.Entidades;
using GSDomain.Enumeradores;
using GerenciadorDeSenhas.ViewModel;
using GerenciadorDeSenhas.Controls;

namespace GerenciadorDeSenhas.Views
{
    public sealed partial class AdicionarCredencialDialog : ContentDialog
    {
        #region Interfaces
        private readonly ICredencialAppService credencialAppService;
        private readonly ICategoriaAppService categoriaAppService;
        private readonly IConfigAppService configAppService;
        #endregion

        #region Propriedades
        private AdicionarCredencialDialogViewModel ViewModel;
        private ModoEdicao modoEdicaoCategoria;
        private ModoEdicao modoEdicaoTela;
        private GSCredencial gSCredencialSelecionada;
        #endregion

        #region Construtor
        public AdicionarCredencialDialog(GSCredencial gSCredencial = null)
        {
            this.InitializeComponent();

            credencialAppService = Bootstrap.Container.GetInstance<ICredencialAppService>();
            categoriaAppService = Bootstrap.Container.GetInstance<ICategoriaAppService>();
            configAppService = Bootstrap.Container.GetInstance<IConfigAppService>();

            this.modoEdicaoTela = (gSCredencial == null ? ModoEdicao.Novo : ModoEdicao.Editar);
            this.gSCredencialSelecionada = gSCredencial;

            ViewModel = new AdicionarCredencialDialogViewModel();
            this.cboCategoria.DataContext = ViewModel;
        }
        #endregion

        #region Eventos
        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            BindTela();
            modoEdicaoCategoria = ModoEdicao.Nenhum;
        }

        private async void btnConfigCategoria_Click(object sender, RoutedEventArgs e)
        {
            bool isVisible = CategoriaExpander.Visibility == Visibility.Visible;

            CategoriaExpander.Visibility = isVisible ? Visibility.Collapsed : Visibility.Visible;

            if (btnConfigCategoria.Content is FontIcon icon)
                icon.Glyph = ObterIconeCampoExpandido(isVisible);

            if (ViewModel.CategoriaSelecionada != null)
                txtCategoria.Text = ViewModel.CategoriaSelecionada.Categoria;

            HabilitarEdicaoCategoria(false);

            await Task.Delay(50);
            MoverScrollParaAreaExpandida(spPrincipal.TransformToVisual(MainScrollViewer));
        }
        private void cboCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtCategoria.Text = "";

            if (ViewModel.CategoriaSelecionada != null && txtCategoria != null)
                txtCategoria.Text = ViewModel.CategoriaSelecionada.Categoria;

            if (CategoriaExpander.Visibility == Visibility.Visible)
            {
                modoEdicaoCategoria = ModoEdicao.Nenhum;
                HabilitarEdicaoCategoria(false);
                cboCategoria.Focus(FocusState.Keyboard);
            }
        }
        private void btnExcluirCategoria_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CategoriaSelecionada == null)
                return;

            modoEdicaoCategoria = ModoEdicao.Excluir;
            HabilitarEdicaoCategoria(true);
            btnCancelarCategoria.Focus(FocusState.Keyboard);
        }
        private void btnAlterarCategoria_Click(object sender, RoutedEventArgs e)
        {
            modoEdicaoCategoria = ModoEdicao.Editar;
            HabilitarEdicaoCategoria(true);
        }
        private void btnNovaCategoria_Click(object sender, RoutedEventArgs e)
        {
            txtCategoria.Text = "";
            modoEdicaoCategoria = ModoEdicao.Novo;
            HabilitarEdicaoCategoria(true);
        }
        private void btnCancelarCategoria_Click(object sender, RoutedEventArgs e)
        {
            modoEdicaoCategoria = ModoEdicao.Nenhum;
            HabilitarEdicaoCategoria(false);
            cboCategoria.Focus(FocusState.Keyboard);
            cboCategoria_SelectionChanged(null, null);
        }
        private async void btnSalvarCategoria_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (modoEdicaoCategoria)
                {
                    case ModoEdicao.Nenhum:
                        break;
                    case ModoEdicao.Novo:
                        var gSCategoriaNovo = new GSCategoria
                        {
                            PK_GSCategoria = 0,
                            Categoria = txtCategoria.Text.ObterValorOuPadrao("").Trim(),
                            FK_GSUsuario = App.PK_GESUsuarioAtivo
                        };

                        int pK_GSCategoria = categoriaAppService.SalvarCategoria(gSCategoriaNovo);
                        if (pK_GSCategoria <= 0)
                        {
                            BindCategoria();
                            await Mensagem.AvisoAsync("Não foi possível adicionar uma nova categoria.", this.XamlRoot);
                            return;
                        }

                        BindCategoria(pK_GSCategoria);
                        break;
                    case ModoEdicao.Editar:
                        var gSCategoriaAtualizar = new GSCategoria
                        {
                            PK_GSCategoria = ViewModel.CategoriaSelecionada.PK_GSCategoria,
                            Categoria = txtCategoria.Text.ObterValorOuPadrao("").Trim(),
                            FK_GSUsuario = App.PK_GESUsuarioAtivo
                        };

                        int categoriaAtualizada = categoriaAppService.SalvarCategoria(gSCategoriaAtualizar);

                        if (categoriaAtualizada <= 0)
                        {
                            await Mensagem.AvisoAsync("Não foi possível atualizar a categoria selecionada.", this.XamlRoot);
                            return;
                        }

                        BindCategoria(gSCategoriaAtualizar.PK_GSCategoria);
                        break;
                    case ModoEdicao.Excluir:
                        var ret = categoriaAppService.DeletarCategoria(ViewModel.CategoriaSelecionada.PK_GSCategoria);

                        if (!ret)
                        {
                            await Mensagem.AvisoAsync("Não foi possível deletar a categoria selecionada.", this.XamlRoot);
                            return;
                        }
                        BindCategoria();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                await Mensagem.ErroAsync(ex.Message, this.XamlRoot);
            }
            finally
            {
                modoEdicaoCategoria = ModoEdicao.Nenhum;
                HabilitarEdicaoCategoria(false);
                cboCategoria.Focus(FocusState.Keyboard);
                cboCategoria_SelectionChanged(null, null);
            }
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
        private async void btnGerarCredencial_Click(object sender, RoutedEventArgs e)
        {
            try
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

                if (caracteres.ObterValorOuPadrao("").Trim() == "")
                {
                    await Mensagem.InformacaoAsync("É necessário marcar alguma das opções para gerar credencial.", this.XamlRoot);
                    return;
                }

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
            catch (Exception ex)
            {
                await Mensagem.ErroAsync(ex.Message, this.XamlRoot);
            }
        }
        private void CheckCredencial_Checked(object sender, RoutedEventArgs e)
        {
            HabilitarBtnGerarCredencial();
        }
        private void CheckCredencial_Unchecked(object sender, RoutedEventArgs e)
        {
            HabilitarBtnGerarCredencial();
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
        private async void btnGerarSenha_Click(object sender, RoutedEventArgs e)
        {
            try
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

                if (caracteres.ObterValorOuPadrao("").Trim() == "")
                {
                    await Mensagem.InformacaoAsync("É necessário marcar alguma das opções para gerar uma senha.", this.XamlRoot);
                    return;
                }

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
            catch (Exception ex)
            {
                await Mensagem.ErroAsync(ex.Message, this.XamlRoot);
            }
        }
        private void CheckSenha_Checked(object sender, RoutedEventArgs e)
        {
            HabilitarBtnGerarSenha();
        }
        private void CheckSenha_Unchecked(object sender, RoutedEventArgs e)
        {
            HabilitarBtnGerarSenha();
        }

        private void QuantidadeMaxima_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (double.IsNaN(sender.Value))
            {
                sender.Value = sender.Minimum;
            }
        }
        private void QuantidadeMinima_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (double.IsNaN(sender.Value))
            {
                sender.Value = sender.Minimum;
            }
        }

        private async void SalvarCredencial_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var categoria = ViewModel.CategoriaSelecionada;
            string credencial = txtCredencial.Text.ObterValorOuPadrao("").Trim();
            string senha = txtSenha.Text.ObterValorOuPadrao("").Trim();

            var gSCredencial = new GSCredencial
            {
                Credencial = credencial,
                Senha = senha,
                FK_GSCategoria = categoria?.PK_GSCategoria,
                DataCriacao = DateTime.Now,
                FK_GSUsuario = App.PK_GESUsuarioAtivo
            };

            if (modoEdicaoTela == ModoEdicao.Editar)
            {
                gSCredencial.PK_GSCredencial = gSCredencialSelecionada.PK_GSCredencial;
                gSCredencial.DataCriacao = gSCredencialSelecionada.DataCriacao;
            }

            var ret = credencialAppService.SalvarCredencial(gSCredencial);

            if ((int)ret > 0)
                await Mensagem.SucessoAsync("Credencial foi salva com sucesso.", this.XamlRoot);
        }
        #endregion

        #region Metodos
        // operações categoria
        private void HabilitarEdicaoCategoria(bool habilitar)
        {
            btnNovaCategoria.Visibility = Visibility.Visible;
            btnAlterarCategoria.Visibility = Visibility.Visible;
            btnExcluirCategoria.Visibility = Visibility.Visible;

            btnSalvarCategoria.Visibility = Visibility.Collapsed;
            btnCancelarCategoria.Visibility = Visibility.Collapsed;
            btnNovaCategoria.IsEnabled = true;

            if (ViewModel.CategoriaSelecionada != null && modoEdicaoCategoria != ModoEdicao.Novo)
            {
                if (ViewModel.CategoriaSelecionada.PK_GSCategoria <= 0)
                {
                    btnAlterarCategoria.IsEnabled = false;
                    btnExcluirCategoria.IsEnabled = false;
                    return;
                }
                else
                {
                    btnAlterarCategoria.IsEnabled = true;
                    btnExcluirCategoria.IsEnabled = true;
                }
            }

            btnNovaCategoria.Visibility = habilitar ? Visibility.Collapsed : Visibility.Visible;
            btnAlterarCategoria.Visibility = habilitar ? Visibility.Collapsed : Visibility.Visible;
            btnExcluirCategoria.Visibility = habilitar ? Visibility.Collapsed : Visibility.Visible;

            btnSalvarCategoria.Visibility = habilitar ? Visibility.Visible : Visibility.Collapsed;
            btnCancelarCategoria.Visibility = habilitar ? Visibility.Visible : Visibility.Collapsed;

            txtCategoria.IsEnabled = (modoEdicaoCategoria == ModoEdicao.Editar || modoEdicaoCategoria == ModoEdicao.Novo);
            txtCategoria.Focus(FocusState.Keyboard);
        }

        // Gerar credencial e senha
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
        private bool IsCheckBoxChecked(CheckBox? checkBox)
        {
            if (checkBox == null)
                return false;

            if (checkBox.IsChecked == null)
                return false;

            return checkBox.IsChecked.Value;
        }
        private void HabilitarBtnGerarCredencial()
        {
            if (CredencialExpander.Visibility == Visibility.Collapsed)
                return;

            btnGerarCredencial.IsEnabled =
                IsCheckBoxChecked(chkLetraMinuscula) ||
                IsCheckBoxChecked(chkLetraMaiuscula) ||
                IsCheckBoxChecked(chkNumeros) ||
                IsCheckBoxChecked(chkSimbolos);
        }
        private void HabilitarBtnGerarSenha()
        {
            if (SenhaExpander.Visibility == Visibility.Collapsed)
                return;

            btnGerarSenha.IsEnabled =
                IsCheckBoxChecked(chkLetraMinusculaSenha) ||
                IsCheckBoxChecked(chkLetraMaiusculaSenha) ||
                IsCheckBoxChecked(chkNumerosSenha) ||
                IsCheckBoxChecked(chkSimbolosSenha);
        }

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

        // Binding
        private void BindTela()
        {
            BindCategoria();

            if (modoEdicaoTela == ModoEdicao.Editar)
            {
                txtCredencial.Text = gSCredencialSelecionada.Credencial.ObterValorOuPadrao("");

                var criptografiaRequisicao = new GSDomain.DTO.CriptografiaRequisicao { Valor = gSCredencialSelecionada.Senha, Salt = gSCredencialSelecionada.Salt };
                var criptografiaResult = configAppService.Descriptografar(criptografiaRequisicao);
                txtSenha.Text = criptografiaResult.Valor;
            }
            else
            {
                txtCredencial.Text = "";
                txtSenha.Text = "";
            }
        }
        private void BindCategoria()
        {
            ViewModel.Categoria = categoriaAppService.ObterCategoriasObservableCollection(App.PK_GESUsuarioAtivo);

            if (modoEdicaoTela == ModoEdicao.Editar && gSCredencialSelecionada.FK_GSCategoria.ObterValorOuPadrao(0) != 0)
            {
                ViewModel.SelecionarCategoria(gSCredencialSelecionada.FK_GSCategoria.ObterValorOuPadrao(0));
            }
            else
            {
                ViewModel.SelecionarCategoriaPorIndice(0);
            }
        }
        private void BindCategoria(int pK_GSCategoria)
        {
            ViewModel.Categoria = categoriaAppService.ObterCategoriasObservableCollection(App.PK_GESUsuarioAtivo);
            ViewModel.SelecionarCategoria(pK_GSCategoria);
        }
        #endregion
    }
}

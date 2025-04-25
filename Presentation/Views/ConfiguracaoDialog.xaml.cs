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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Application;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Entidades;
using Application.Services;
using JJ.NET.Core.Extensoes;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;


namespace Presentation.Views
{
    public sealed partial class ConfiguracaoDialog : ContentDialog, IDisposable
    {
        #region Interfaces
        private readonly ILoginService loginService;
        private readonly INotificationService notificationService;
        private readonly IConfigAppService configAppService;
        private readonly ICategoriaAppService categoriaAppService;
        private readonly ICredencialAppService credencialAppService;
        #endregion

        #region Propriedades
        private GSUsuario gSUsuarioAtivo;
        #endregion

        #region Construtor
        public ConfiguracaoDialog()
        {
            this.InitializeComponent();

            loginService = Bootstrap.Container.GetInstance<ILoginService>();
            notificationService = Bootstrap.Container.GetInstance<INotificationService>();
            configAppService = Bootstrap.Container.GetInstance<IConfigAppService>();
            categoriaAppService = Bootstrap.Container.GetInstance<ICategoriaAppService>();
            credencialAppService = Bootstrap.Container.GetInstance<ICredencialAppService>();
        }
        #endregion
        #region Eventos
        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoUsuario(false);
            HabilitarEdicaoSenha(false);
            HabilitarEdicaoNome(false);

            Pesquisar();
        }
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
        private void btnAlterarNome_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoUsuario(false);
            HabilitarEdicaoSenha(false);

            HabilitarEdicaoNome(true);
        }
        private void btnCancelarNome_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoNome(false);
            txtNome.Text = gSUsuarioAtivo.Nome;
        }
        private void btnSalvarNome_Click(object sender, RoutedEventArgs e)
        {
            if (txtNome.Text.ObterValorOuPadrao("").Trim() == "")
                return;

            try
            {
                GSUsuario gSUsuario = gSUsuarioAtivo.DeepCopy();
                gSUsuario.Nome = txtNome.Text.ObterValorOuPadrao("").Trim();
                
                loginService.AtualizarUsuario(gSUsuario);
                
                if (!gSUsuario.ValidarResultado.EhValido)
                    notificationService.EnviarNotificacao(gSUsuario.ValidarResultado.ObterPrimeiroErro());
                
                HabilitarEdicaoNome(false);
                Pesquisar();
            }
            catch (Exception ex)
            {
                notificationService.EnviarNotificacao(ex.Message);
            }
        }
        private void btnAlterarUsuario_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoNome(false);
            HabilitarEdicaoSenha(false);

            HabilitarEdicaoUsuario(true);
        }
        private void btnCancelarUsuario_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoUsuario(false);
            txtUsuario.Text = gSUsuarioAtivo.Usuario;
        }
        private void btnSalvarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (txtUsuario.Text.ObterValorOuPadrao("").Trim() == "")
                return;

            try
            {
                GSUsuario gSUsuario = gSUsuarioAtivo.DeepCopy();
                gSUsuario.Usuario = txtUsuario.Text.ObterValorOuPadrao("").Trim();

                loginService.AtualizarUsuario(gSUsuario);

                if (!gSUsuario.ValidarResultado.EhValido)
                    notificationService.EnviarNotificacao(gSUsuario.ValidarResultado.ObterPrimeiroErro());

                HabilitarEdicaoUsuario(false);
                Pesquisar();
            }
            catch (Exception ex)
            {
                notificationService.EnviarNotificacao(ex.Message);
            }
        }
        private void btnAlterarSenha_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoNome(false);
            HabilitarEdicaoUsuario(false);

            HabilitarEdicaoSenha(true);
        }
        private void btnCancelarSenha_Click(object sender, RoutedEventArgs e)
        {
            HabilitarEdicaoSenha(false);

            var criptografiaResult = configAppService.Descriptografar(gSUsuarioAtivo.Senha, gSUsuarioAtivo.IVSenha);
            passSenha.Password = criptografiaResult.Valor;
        }
        private void btnSalvarSenha_Click(object sender, RoutedEventArgs e)
        {
            if (passSenha.Password.ObterValorOuPadrao("").Trim() == "")
                return;

            try
            {
                GSUsuario gSUsuario = gSUsuarioAtivo.DeepCopy();
                gSUsuario.Senha = passSenha.Password.ObterValorOuPadrao("").Trim();
                gSUsuario.IVSenha = "";

                var criptografiaResult = configAppService.Criptografar(gSUsuario.Senha, gSUsuario.IVSenha);

                if (criptografiaResult.Erro.ObterValorOuPadrao("").Trim() != "")
                {
                    notificationService.EnviarNotificacao(criptografiaResult.Erro);
                    return;
                }

                gSUsuario.Senha = criptografiaResult.Valor;
                gSUsuario.IVSenha = criptografiaResult.IV;

                loginService.AtualizarUsuario(gSUsuario);

                if (!gSUsuario.ValidarResultado.EhValido)
                    notificationService.EnviarNotificacao(gSUsuario.ValidarResultado.ObterPrimeiroErro());

                HabilitarEdicaoSenha(false);
                Pesquisar();
            }
            catch (Exception ex)
            {
                notificationService.EnviarNotificacao(ex.Message);
            }
        }
        private void btnDesconectar_Click(object sender, RoutedEventArgs e)
        {
            Dispose();
            this.Hide();

            NavigationService.NavegarPara(typeof(Login));
        }
        #endregion

        #region Metodos
        private void HabilitarEdicaoNome(bool habilitar)
        {
            btnAlterarNome.Visibility = (habilitar ? Visibility.Collapsed : Visibility.Visible);
            btnCancelarNome.Visibility = (habilitar ? Visibility.Visible : Visibility.Collapsed);
            btnSalvarNome.Visibility = (habilitar ? Visibility.Visible : Visibility.Collapsed);

            txtNome.IsEnabled = habilitar;
        }
        private void HabilitarEdicaoUsuario(bool habilitar)
        {
            btnAlterarUsuario.Visibility = (habilitar ? Visibility.Collapsed : Visibility.Visible);
            btnCancelarUsuario.Visibility = (habilitar ? Visibility.Visible : Visibility.Collapsed);
            btnSalvarUsuario.Visibility = (habilitar ? Visibility.Visible : Visibility.Collapsed);

            txtUsuario.IsEnabled = habilitar;
        }
        private void HabilitarEdicaoSenha(bool habilitar)
        {
            btnAlterarSenha.Visibility = (habilitar ? Visibility.Collapsed : Visibility.Visible);
            btnCancelarSenha.Visibility = (habilitar ? Visibility.Visible : Visibility.Collapsed);
            btnSalvarSenha.Visibility = (habilitar ? Visibility.Visible : Visibility.Collapsed);

            passSenha.IsEnabled = habilitar;
        }
        private void Pesquisar()
        {
            try
            {
                gSUsuarioAtivo = loginService.ObterUsuario(App.PK_GESUsuarioAtivo);

                if (gSUsuarioAtivo == null)
                {
                    notificationService.EnviarNotificacao("Não foi possível identificar o usuário ativo.");
                    this.Hide();
                }

                BindPrincipal();
            }
            catch (Exception ex)
            {
                notificationService.EnviarNotificacao(ex.Message);
            }
        }
        private void BindPrincipal()
        {
            txtNome.Text = gSUsuarioAtivo.Nome;
            txtUsuario.Text = gSUsuarioAtivo.Usuario;

            var criptografiaResult = configAppService.Descriptografar(gSUsuarioAtivo.Senha, gSUsuarioAtivo.IVSenha);
            passSenha.Password = criptografiaResult.Valor;
        }
        public void Dispose()
        {
            (loginService as IDisposable)?.Dispose();
            (notificationService as IDisposable)?.Dispose();
            (configAppService as IDisposable)?.Dispose();
        }
        private void MoverScrollParaAreaExpandida(GeneralTransform transform)
        {
            Point position = transform.TransformPoint(new Point(0, 0));

            MainScrollViewer.ChangeView(null, position.Y, null, true);
        }
        #endregion

        private async void btnDeletarConta_Click(object sender, RoutedEventArgs e)
        {
            FecharPanels();

            spConfirmacaoContaDelete.Visibility = spConfirmacaoContaDelete.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            if (spConfirmacaoContaDelete.Visibility == Visibility.Visible)
                txtConfirmarContaUsuario.Text = $"Essa operação não poderá ser desfeita.\n" +
                    $"Todos os dados vinculados à sua conta serão permanentemente excluídos.\n" +
                    $"Para confirmar, digite seu nome de usuário: {gSUsuarioAtivo.Usuario}";

            await Task.Delay(50);
            MoverScrollParaAreaExpandida(btnDeletarConta.TransformToVisual(MainScrollViewer));
        }

        private void txtUsuarioContaConfirmacao_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnConfirmarContaExclusao.IsEnabled = (txtUsuarioContaConfirmacao.Text.ObterValorOuPadrao("").Trim() != "");
        }
        private void btnConfirmarContaExclusao_Click(object sender, RoutedEventArgs e)
        {
            if (txtUsuarioContaConfirmacao.Text.ObterValorOuPadrao("").Trim() != gSUsuarioAtivo.Usuario.Trim())
            {
                notificationService.EnviarNotificacao("Usuário inválido.");
                return;
            }

            var ret = configAppService.DeletarContaUsuarioLogado(gSUsuarioAtivo.PK_GSUsuario);

            if (ret)
            {
                notificationService.EnviarNotificacao("Conta do usuário deletada com sucesso.");
                btnDesconectar_Click(null, null);
                return;
            }

            notificationService.EnviarNotificacao("Não foi possível deletar a conta do usuário ativo.", "Tente fechar e abrir novamente o programa.");
        }

        private async void btnDeletarCategoria_Click(object sender, RoutedEventArgs e)
        {
            FecharPanels();

            spConfirmacaoCategoriaDelete.Visibility = spConfirmacaoCategoriaDelete.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            if (spConfirmacaoCategoriaDelete.Visibility == Visibility.Visible)
                txtConfirmarCategoriaUsuario.Text = $"Essa operação não poderá ser desfeita.\n" +
                    $"Todos os dados vinculados à sua conta serão permanentemente excluídos.\n" +
                    $"Para confirmar, digite seu nome de usuário: {gSUsuarioAtivo.Usuario}";

            await Task.Delay(50);
            MoverScrollParaAreaExpandida(btnDeletarCategoria.TransformToVisual(MainScrollViewer));
        }

        private void txtUsuarioCategoriaConfirmacao_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnConfirmarCategoriaExclusao.IsEnabled = (txtUsuarioCategoriaConfirmacao.Text.ObterValorOuPadrao("").Trim() != "");
        }

        private void btnConfirmarCategoriaExclusao_Click(object sender, RoutedEventArgs e)
        {
            if (txtUsuarioCategoriaConfirmacao.Text.ObterValorOuPadrao("").Trim() != gSUsuarioAtivo.Usuario.Trim())
            {
                notificationService.EnviarNotificacao("Usuário inválido.");
                return;
            }

            var ret = categoriaAppService.DeletarCategoriaPorUsuario(gSUsuarioAtivo.PK_GSUsuario);

            if (ret)
            {
                notificationService.EnviarNotificacao("Categorias deletadas com sucesso.");
                return;
            }

            notificationService.EnviarNotificacao("Não foi possível deletar as categorias do usuário ativo.", "Tente fechar e abrir novamente o programa.");
        }


        private async void btnDeletarCredencial_Click(object sender, RoutedEventArgs e)
        {
            FecharPanels();

            spConfirmacaoCredencialDelete.Visibility = spConfirmacaoCredencialDelete.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            if (spConfirmacaoCredencialDelete.Visibility == Visibility.Visible)
                txtConfirmarCredencialUsuario.Text = $"Essa operação não poderá ser desfeita.\n" +
                    $"Todos os dados vinculados à sua conta serão permanentemente excluídos.\n" +
                    $"Para confirmar, digite seu nome de usuário: {gSUsuarioAtivo.Usuario}";

            await Task.Delay(50);
            MoverScrollParaAreaExpandida(btnDeletarCredencial.TransformToVisual(MainScrollViewer));
        }

        private void txtUsuarioCredencialConfirmacao_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnConfirmarCredencialExclusao.IsEnabled = (txtUsuarioCredencialConfirmacao.Text.ObterValorOuPadrao("").Trim() != "");
        }

        private void btnConfirmarCredencialExclusao_Click(object sender, RoutedEventArgs e)
        {
            if (txtUsuarioCredencialConfirmacao.Text.ObterValorOuPadrao("").Trim() != gSUsuarioAtivo.Usuario.Trim())
            {
                notificationService.EnviarNotificacao("Usuário inválido.");
                return;
            }

            var ret = credencialAppService.DeletarCredencialPorUsuario(gSUsuarioAtivo.PK_GSUsuario);

            if (ret)
            {
                notificationService.EnviarNotificacao("Credenciais deletadas com sucesso.");
                return;
            }

            notificationService.EnviarNotificacao("Não foi possível deletar as credenciais do usuário ativo.", "Tente fechar e abrir novamente o programa.");
        }

        private async void btnDeletarBaseDados_Click(object sender, RoutedEventArgs e)
        {
            FecharPanels();
            
            spConfirmacaoBaseDadosDelete.Visibility = spConfirmacaoBaseDadosDelete.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            if (spConfirmacaoBaseDadosDelete.Visibility == Visibility.Visible)
                txtConfirmarBaseDadosUsuario.Text = $"Essa operação não poderá ser desfeita.\n" +
                    $"Todos os dados serão permanentemente excluídos.\n" +
                    $"Para confirmar, digite seu nome de usuário: {gSUsuarioAtivo.Usuario}";

            await Task.Delay(50);
            MoverScrollParaAreaExpandida(btnDeletarBaseDados.TransformToVisual(MainScrollViewer));
        }

        private void txtUsuarioBaseDadosConfirmacao_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnConfirmarBaseDadosExclusao.IsEnabled = (txtUsuarioBaseDadosConfirmacao.Text.ObterValorOuPadrao("").Trim() != "");
        }

        private void btnConfirmarBaseDadosExclusao_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FecharPanels()
        {
            spConfirmacaoCredencialDelete.Visibility = Visibility.Collapsed;
            spConfirmacaoCategoriaDelete.Visibility = Visibility.Collapsed;
            spConfirmacaoContaDelete.Visibility = Visibility.Collapsed;
            spConfirmacaoBaseDadosDelete.Visibility = Visibility.Collapsed;
        }
    }
}

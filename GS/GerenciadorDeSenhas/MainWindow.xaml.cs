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
using GerenciadorDeSenhas.Views;
using GSApplication;
using GSApplication.Services;
using Windows.UI.Popups;
using Windows.Storage;

namespace GerenciadorDeSenhas
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            Logar();
        }

        public async void Logar()
        {
            try
            {
                Bootstrap.Inicializar();

                NavigationService.MainFrame = this.MainFrame;
                NavigationService.NavegarPara(typeof(Login));
            }
            catch (Exception ex)
            {
                try
                {
                    // Gravar erro no arquivo
                    string logPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "error_log.txt");
                    string logContent = $"[{DateTime.Now}] {ex}\n";

                    File.AppendAllText(logPath, logContent);
                }
                catch
                {
                    // Se falhar o log, não podemos fazer nada
                }

                // Mostrar erro na tela
                ContentDialog errorDialog = new ContentDialog()
                {
                    Title = "Erro",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot,
                };

                await errorDialog.ShowAsync();
            }

        }
    }
}

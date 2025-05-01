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

using Microsoft.UI;           // Needed for WindowId.
using Microsoft.UI.Windowing; // Needed for AppWindow.
using WinRT.Interop;          // Needed for XAML/HWND interop.

namespace GerenciadorDeSenhas
{
    public sealed partial class MainWindow : Window
    {
        private AppWindow m_AppWindow;
        public MainWindow()
        {
            this.InitializeComponent();

            m_AppWindow = GetAppWindowForCurrentWindow();
            m_AppWindow.Title = "Gerenciador de Senhas";
            m_AppWindow.SetIcon("Assets/gerenciador_de_senhas.ico");

            NavigationService.MainFrame = this.MainFrame;
            NavigationService.NavegarPara(typeof(Login));
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }
    }
}

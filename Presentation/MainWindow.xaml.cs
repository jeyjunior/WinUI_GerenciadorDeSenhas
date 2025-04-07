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
using Domain.Entidades;
using Domain.Enumeradores;
using Application;
using Application.Interfaces;
using Presentation.Views;
using Application.Services;

namespace Presentation
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            NavigationService.MainFrame = this.MainFrame;
            NavigationService.NavegarPara(typeof(Principal));
        }
    }
}

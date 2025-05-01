using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using GSApplication.Interfaces;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using GSDomain.Enumeradores;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System.Drawing;
using System.Reflection;
using JJ.NET.Core.Extensoes;

namespace GSApplication.Services
{
    public class NotificationService : INotificationService
    {
        public NotificationService()
        {
        }

        public async Task ExibirErroAsync(string mensagem, XamlRoot xamlRoot)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Erro",
                Content = mensagem,
                CloseButtonText = "OK",
                XamlRoot = xamlRoot
            };

            await dialog.ShowAsync();
        }

        public void EnviarNotificacao(string titulo)
        {
            EnviarNotificacao(titulo, "");
        }
        public void EnviarNotificacao(string titulo, string mensagem, bool duracaoRapida = true)
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            var toastElement = (Windows.Data.Xml.Dom.XmlElement)toastXml.SelectSingleNode("/toast");
            toastElement.SetAttribute("duration", (duracaoRapida) ? "short" : "long");

            var toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(titulo));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(mensagem));

            var toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }


    }
}

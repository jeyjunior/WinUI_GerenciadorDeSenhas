using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task ExibirErroAsync(string mensagem, XamlRoot xamlRoot);
        void EnviarNotificacao(string titulo, string mensagem, bool duracaoRapida = true);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entidades;

namespace Application.Interfaces
{
    public interface ILoginService
    {
        int Entrar(GSUsuarioRequest gSUsuario);
        bool Registrar(GSUsuarioRequest gSUsuarioRequest);
        void DeletarUsuarioLembrado();
        void SalvarUsuarioLembrado(string usuario);
        Task<string> ObterUsuarioLembrado();
        GSUsuario ObterUsuario(int pK_GSUsuario);
        bool AtualizarUsuario(GSUsuario gSUsuario);
    }
}

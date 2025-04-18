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
    }
}

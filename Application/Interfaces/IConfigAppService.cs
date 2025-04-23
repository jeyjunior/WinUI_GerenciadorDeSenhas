using JJ.NET.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IConfigAppService
    {
        CriptografiaResult Descriptografar(string valor, string iv);
        CriptografiaResult Criptografar(string valor, string iv);
        bool DeletarContaUsuarioLogado(int PK_GSUsuario);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSDomain.Entidades;
namespace GSApplication.Interfaces
{
    public interface IConfiguracaoAppService
    {
        string Descriptografar(CriptografiaRequest criptografiaRequest);
        string Criptografar(CriptografiaRequest criptografiaRequest);
    }
}

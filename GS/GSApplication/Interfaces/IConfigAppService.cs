using GSDomain.DTO;
using JJ.NET.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSApplication.Interfaces
{
    public interface IConfigAppService
    {
        DescriptografiaResultado Descriptografar(CriptografiaRequisicao criptoRequest);
        CriptografiaResultado Criptografar(string valor, int pk_GESUsuario);
        bool DeletarContaUsuarioLogado(int PK_GSUsuario);
        string GerarChavePrincipal(int PK_GSUsuario);
    }
}

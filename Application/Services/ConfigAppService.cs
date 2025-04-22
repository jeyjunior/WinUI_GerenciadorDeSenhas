using Application.Interfaces;
using JJ.NET.Core.Extensoes;
using JJ.NET.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ConfigAppService : IConfigAppService
    {
        public CriptografiaResult Descriptografar(string valor, string iv)
        {
            var criptografiaRequest = new JJ.NET.Cryptography.CriptografiaRequest
            {
                IV = iv.ObterValorOuPadrao("").Trim(),
                Valor = valor.ObterValorOuPadrao("").Trim(),
                TipoCriptografia = JJ.NET.Cryptography.Enumerador.TipoCriptografia.AES,
            };

            var criptografiaResult = Criptografia.Descriptografar(criptografiaRequest);

            if (criptografiaResult.Erro.ObterValorOuPadrao("").Trim() != "")
                return new CriptografiaResult { Valor = "", IV = "", Erro = criptografiaResult.Erro };

            return criptografiaResult;
        }
        public CriptografiaResult Criptografar(string valor, string iv)
        {
            var criptografiaRequest = new JJ.NET.Cryptography.CriptografiaRequest
            {
                IV = iv.ObterValorOuPadrao("").Trim(),
                Valor = valor.ObterValorOuPadrao("").Trim(),
                TipoCriptografia = JJ.NET.Cryptography.Enumerador.TipoCriptografia.AES,
            };

            var criptografiaResult = Criptografia.Criptografar(criptografiaRequest);

            if (criptografiaResult.Erro.ObterValorOuPadrao("").Trim() != "")
                return new CriptografiaResult { Valor = "", IV = "", Erro = criptografiaResult.Erro };

            return criptografiaResult;
        }
    }
}

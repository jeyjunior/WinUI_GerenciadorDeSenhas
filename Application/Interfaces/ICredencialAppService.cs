using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entidades;
using JJ.NET.Core.DTO;

namespace Application.Interfaces
{
    public interface ICredencialAppService
    {
        IEnumerable<GSCredencial> Pesquisar(GSCredencialPesquisaRequest requisicao);
        GSCredencial PesquisarPorID(int PK_GSCredencial);
        ObservableCollection<Item> ObterTipoDePesquisa();
        ObservableCollection<Item> ObterTipoDeOrdenacao();
        IEnumerable<GSCategoria> ObterCategorias();
        ObservableCollection<Item> ObterCategoriasObservableCollection();
        int SalvarCredencial(GSCredencial gSCredencial);
        bool DeletarCredencial(int PK_GSCredencial);
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entidades;

namespace Application.Interfaces
{
    public interface ICategoriaAppService
    {
        IEnumerable<GSCategoria> ObterCategorias();
        ObservableCollection<GSCategoria> ObterCategoriasObservableCollection();
        bool DeletarCategoria(int PK_GSCategoria);
        int SalvarCategoria(GSCategoria gSCategoria);
    }
}

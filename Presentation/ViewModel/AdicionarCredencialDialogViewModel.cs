using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entidades;
using JJ.NET.Core.DTO;
using JJ.NET.Core.Extensoes;

namespace Presentation.ViewModel
{
    public class AdicionarCredencialDialogViewModel : INotifyPropertyChanged
    {
        #region Propriedades
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Categoria
        private ObservableCollection<GSCategoria> _categoria;
        private GSCategoria _categoriaSelecionada;

        public ObservableCollection<GSCategoria> Categoria
        {
            get => _categoria;
            set
            {
                _categoria = value;
                OnPropertyChanged(nameof(Categoria));
            }
        }

        public GSCategoria CategoriaSelecionada
        {
            get => _categoriaSelecionada;
            set
            {
                _categoriaSelecionada = value;
                OnPropertyChanged(nameof(CategoriaSelecionada));
            }
        }
        public bool SelecionarCategoria(object id)
        {
            var categoria_ = Categoria.ObterPorID(id, i => i.PK_GSCategoria);

            if (categoria_ == null)
                return false;

            CategoriaSelecionada = categoria_;

            return true;
        }

        public bool SelecionarCategoriaPorIndice(int indice)
        {
            if (indice < 0 || indice >= Categoria.Count)
                return false;

            CategoriaSelecionada = Categoria[indice];
            return true;
        }
        #endregion

        #region Metodos
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

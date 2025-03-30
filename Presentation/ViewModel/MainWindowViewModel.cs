using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.NET.Core.DTO;
using JJ.NET.Core.Extensoes;

namespace Presentation.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Propriedades
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region TipoDeOrdenacao
        private ObservableCollection<Item> _tipoDeOrdenacao;
        private Item _tipoDeOrdenacaoSelecionado;

        public ObservableCollection<Item> TipoDeOrdenacao
        {
            get => _tipoDeOrdenacao;
            set
            {
                _tipoDeOrdenacao = value;
                OnPropertyChanged(nameof(TipoDeOrdenacao));
            }
        }

        public Item TipoDeOrdenacaoSelecionado
        {
            get => _tipoDeOrdenacaoSelecionado;
            set
            {
                _tipoDeOrdenacaoSelecionado = value;
                OnPropertyChanged(nameof(TipoDeOrdenacaoSelecionado));
            }
        }
        public bool SelecionarTipoDeOrdenacao(object id)
        {
            var tipoDeOrdenacao = TipoDeOrdenacao.ObterPorID(id);

            if (tipoDeOrdenacao == null)
                return false;

            TipoDeOrdenacaoSelecionado = tipoDeOrdenacao;

            return true;
        }
        #endregion

        #region TipoDePesquisa
        private ObservableCollection<Item> _tipoDePesquisa;
        private Item _tipoDePesquisaSelecionado;
        public ObservableCollection<Item> TipoDePesquisa
        {
            get => _tipoDePesquisa;
            set
            {
                _tipoDePesquisa = value;
                OnPropertyChanged(nameof(TipoDePesquisa));
            }
        }
        public Item TipoDePesquisaSelecionado
        {
            get => _tipoDePesquisaSelecionado;
            set
            {
                _tipoDePesquisaSelecionado = value;
                OnPropertyChanged(nameof(TipoDePesquisaSelecionado));
            }
        }

        public bool SelecionarTipoDePesquisa(object id)
        {
            var tipoDePesquisa = TipoDePesquisa.ObterPorID(id);

            if (tipoDePesquisa == null)
                return false;

            TipoDePesquisaSelecionado = tipoDePesquisa;

            return true;
        }
        #endregion

        #region Credencial
        private ObservableCollection<CredencialViewModel> _credenciais;
        public ObservableCollection<CredencialViewModel> Credenciais
        {
            get => _credenciais;
            set
            {
                _credenciais = value;
                OnPropertyChanged(nameof(Credenciais));
            }
        }
        #endregion

        #region Construtor
        public MainWindowViewModel()
        {
            Credenciais = new ObservableCollection<CredencialViewModel>();
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

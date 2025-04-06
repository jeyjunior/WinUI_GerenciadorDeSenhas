using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.ViewModel
{
    public class CredencialViewModel : INotifyPropertyChanged
    {
        #region Propriedades
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region PK_GSCredencial
        private int _pk_GSCredencial;
        public int PK_GSCredencial
        {
            get => _pk_GSCredencial;
            set
            {
                _pk_GSCredencial = value;
                OnPropertyChanged(nameof(PK_GSCredencial));
            }
        }
        #endregion

        #region Categoria
        private string _categoria;
        public string Categoria
        {
            get => _categoria;
            set
            {
                _categoria = value;
                OnPropertyChanged(nameof(Categoria));
            }
        }
        #endregion

        #region DataModificacao
        private string _modificacao;
        public string Modificacao
        {
            get => _modificacao;
            set
            {
                _modificacao = value;
                OnPropertyChanged(nameof(Modificacao));
            }
        }
        #endregion

        #region Credencial
        private string _credencial;
        public string Credencial
        {
            get => _credencial;
            set
            {
                _credencial = value;
                OnPropertyChanged(nameof(Credencial));
            }
        }
        #endregion

        #region Senha
        private string _senha;
        private bool _exibirSenha;

        public string Senha
        {
            get => _senha;
            set
            {
                _senha = value;
                OnPropertyChanged(nameof(Senha));
            }
        }

        public bool ExibirSenha
        {
            get => _exibirSenha;
            set
            {
                _exibirSenha = value;
                OnPropertyChanged(nameof(ExibirSenha));
            }
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

using MaliyetApp.Libs.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MaliyetApp.ViewModels
{
    public class MaterialListViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SaleDetail> _sale;
        private ObservableCollection<Sale> _saleList;

        public ObservableCollection<SaleDetail> Sale
        {
            get => _sale;
            set
            {
                _sale = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Sale> SaleList
        {
            get => _saleList;
            set
            {
                _saleList = value;
                OnPropertyChanged();
            }
        }
        public MaterialListViewModel()
        {
            Sale = new ObservableCollection<SaleDetail>();
            SaleList = new ObservableCollection<Sale>();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

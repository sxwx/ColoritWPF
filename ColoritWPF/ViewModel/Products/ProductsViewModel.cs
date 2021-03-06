﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ColoritWPF.Common;
using ColoritWPF.Views.Products;
using GalaSoft.MvvmLight.Command;

namespace ColoritWPF.ViewModel.Products
{
    public class ProductsViewModel : BaseVmWithBlls
    {
        public ProductsViewModel()
        {
            GetData();
            InitializeCommands();

            SaleProductsList.CollectionChanged += CollectionChanged;
        }
        
        #region Properties

        public ObservableCollection<Client> Clients { get; set; }
        public ObservableCollection<Sale> SaleProductsList { get; set; }
        public ObservableCollection<SaleDocument> SaleDocuments { get; set; }

        private Client _currentClient;
        public Client CurrentClient
        {
            get
            {
                if (CurrentSaleDocument == null)
                    return Clients.First(client => client.PrivatePerson);
                return _currentClient;
            }
            set
            {
                CurrentSaleDocument.Client = value;
                _currentClient = value;
                CurrentDiscount = _currentClient.Discount;
                OnPropertyChanged("CurrentClient");
                UpdateDiscountValue();
            }
        }

        private bool _includeClientBalanceToTotal;
        public bool IncludeClientBalanceToTotal
        {
            get { return _includeClientBalanceToTotal; }
            set
            {
                _includeClientBalanceToTotal = value;
                OnPropertyChanged("IncludeClientBalanceToTotal");
                Recalc();
            }
        }

        public double CurrentDiscount
        {
            get
            {
                if (CurrentSaleDocument == null)
                    return 0;
                return CurrentSaleDocument.Discount;
            }
            set
            {
                CurrentSaleDocument.Discount = value;
                OnPropertyChanged("CurrentDiscount");
                UpdateDiscountValue();
            }
        }

        private bool _prepay;
        public bool Prepay
        {
            get { return _prepay; }
            set
            {
                _prepay = value;
                OnPropertyChanged("Prepay");
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        private bool _confirmed;
        public bool Confirmed
        {
            get { return _confirmed; }
            set
            {
                _confirmed = value;
                OnPropertyChanged("Confirmed");
            }
        }

        private SaleDocument _currentSaleDocument;
        public SaleDocument CurrentSaleDocument
        {
            get { return _currentSaleDocument; }
            set 
            { 
                _currentSaleDocument = value;
                OnPropertyChanged("CurrentSaleDocument");
                if (value != null)
                {
                    CurrentClient = _currentSaleDocument.Client;

                    _confirmButtonContent = value.Confirmed ? "Разпровести" : "Провести";
                    OnPropertyChanged("ConfirmButtonContent");

                    //При смене документа отчищаем список продуктов и перезаполняем.. да, ужас.. но я пока хз как лучше сделать
                    //а все потому что на изменения SaleProductsList подписка слушателя, которая сбрасывается
                    //если оставить как было: SaleProductsList = new ObservableCollection<Sale>(_currentSaleDocument.Sale.ToList());
                    SaleProductsList.Clear();
                    var tempCollection = new ObservableCollection<Sale>(_currentSaleDocument.Sale.ToList());
                    foreach (Sale sale in tempCollection)
                    {
                        SaleProductsList.Add(sale);
                    }

                    OnPropertyChanged("SaleProductsList");

                    IsEnabled = !_currentSaleDocument.Confirmed;
                }
            }
        }

        private Sale _currentSale;
        public Sale CurrentSale
        {
            get { return _currentSale; }
            set
            {
                _currentSale = value;
                OnPropertyChanged("CurrentSale");
            }
        }

        private string _confirmButtonContent = "Провести";
        public string ConfirmButtonContent
        {
            get
            {
                return _confirmButtonContent;
            }
            set { 
                    _confirmButtonContent = value;
                    OnPropertyChanged("ConfirmButtonContent");
                }
        }

        #endregion

        #region Methods

        private void GetData()
        {
            Clients = new ObservableCollection<Client>(ClientsBll.GetClients());
            SaleDocuments = new ObservableCollection<SaleDocument>(DocumentsBll.GetSaleDocument());
            SaleProductsList = new ObservableCollection<Sale>();
        }

        //Этот метод позволяет узнать когда изменился элемент коллекции
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Sale item in e.OldItems)
                {
                    //Removed items
                    item.PropertyChanged -= Recalc;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Sale item in e.NewItems)
                {
                    //Added items
                    item.PropertyChanged += Recalc;
                }
            }
            Recalc();
            UpdateDiscountValue();
        }
        //А этот метод уже делает что надо когда узнает что элемент был изменен
        public void Recalc(object sender, PropertyChangedEventArgs e)
        {
            CurrentSaleDocument.CleanTotal = 0;
            CurrentSaleDocument.Total = 0;
            foreach (Sale product in SaleProductsList)
            {
                CurrentSaleDocument.CleanTotal += product.CleanTotal;
                CurrentSaleDocument.Total += product.Total;
            }
        }
        public void Recalc()
        {
            if (CurrentSaleDocument != null)
            {
                CurrentSaleDocument.CleanTotal = 0;
                CurrentSaleDocument.Total = 0;
                foreach (Sale saleProduct in SaleProductsList)
                {
                    CurrentSaleDocument.CleanTotal += saleProduct.CleanTotal;
                    CurrentSaleDocument.Total += saleProduct.Total;
                }
                if (IncludeClientBalanceToTotal)
                    CurrentSaleDocument.Total -= CurrentClient.Balance;
            }
        }

        /// <summary>
        /// Обновляет значение скидки для каждого продукта
        /// </summary>
        private void UpdateDiscountValue()
        {
            if (CurrentClient == null)
                return;

            foreach (Sale product in SaleProductsList)
            {
                product.CurrentDiscount = CurrentDiscount;
            }
        }

        //Удаляет из списка продуктов выбранный элемент
        private void RemoveSaleItemFromList(IEnumerable<Sale> selectedItemsToRemove)
        {
            if (selectedItemsToRemove == null) return;
            
            var tempList = new List<Sale>(selectedItemsToRemove);

            foreach (var sale in tempList)
            {
                DocumentsBll.RemoveItemFromSaleDocument(sale.ID);
                        
                if (SaleProductsList.Contains(sale))
                    SaleProductsList.Remove(sale);
            }
        }

        private void RemoveSaleItemFromList(Sale selectedItemToRemove)
        {
            if (selectedItemToRemove == null) return;

            DocumentsBll.RemoveItemFromSaleDocument(selectedItemToRemove.ID);
                    
            SaleProductsList.Remove(selectedItemToRemove);
        }

        #endregion

        #region Commands

        public RelayCommand OpenSelectionCommand
        {
            get;
            private set;
        }

        public RelayCommand PrintDocumentCommand
        {
            get;
            private set;
        }


        public RelayCommand ConfirmDocumentCommand
        {
            get;
            private set;
        }

        public RelayCommand PrepayDocumentCommand
        {
            get;
            private set;
        }

        public RelayCommand MoveProductDlgCommand
        {
            get;
            private set;
        }

        public RelayCommand RemoveProductFromListCommand
        {
            get
            {
                return new RelayCommand(() => RemoveSaleItemFromList(CurrentSale), () => IsEnabled);
            }
        }


        public RelayCommand RemoveProductDocumentFromListCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MessageBox.Show("Вы уверены что хотите удалить документ?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                    {
                        return;
                    }
                    
                    DocumentsBll.RemoveSaleDocument(CurrentSaleDocument.Id);
                    SaleDocuments.Remove(CurrentSaleDocument);

                }, () => IsEnabled);
            }
        }

        public RelayCommand<Visual> PrintCommand
        {
            get
            {
                return new RelayCommand<Visual>(v => PrintHelper.PrintSaleDocument(v, CurrentSaleDocument), visual => CurrentSaleDocument != null);
            }
        }


        private void InitializeCommands()
        {
            OpenSelectionCommand = new RelayCommand(OpenSelection);            
            ConfirmDocumentCommand = new RelayCommand(ConfirmDocument, ConfirmDocumentCanExecute);
            MoveProductDlgCommand = new RelayCommand(MoveProductDlg);
            PrepayDocumentCommand = new RelayCommand(PrepayDocument);
        }

        private bool ConfirmDocumentCanExecute()
        {
            return CurrentSaleDocument != null;
        }

        private void MoveProductDlg()
        {
            var transferProductView = new TransferProductView();
            transferProductView.ShowDialog();
        }

        private void PrepayDocument()
        {
            Prepay = true;
            Confirmed = false;
            SaveDocument();
        }

        private void ConfirmDocument()
        {
            if (MessageBox.Show("Вы уверены что хотите "+ ConfirmButtonContent.ToLower()+" документ?",
                                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (!CurrentSaleDocument.Confirmed)
                {
                    Prepay = false;
                    Confirmed = true;

                    SaveDocument();
                    IsEnabled = false;
                }
                else
                {
                    Prepay = true;
                    Confirmed = false;
                    UnConfirmDocument();
                    IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Используется при проведении и сохранении документа, в зависимости от значений Confirmed и Prepay
        /// </summary>
        private void SaveDocument()
        {
            if (ProductsBll.IsAllProductInStock(SaleProductsList) == false)
                return;

            if(CurrentSaleDocument == null)
                return;         

            CurrentSaleDocument.ClientId = CurrentClient.ID;
            CurrentSaleDocument.Prepay = Prepay;
            CurrentSaleDocument.Confirmed = Confirmed;
            
            if (IncludeClientBalanceToTotal && Confirmed && !CurrentClient.PrivatePerson)
            {
                //TODO: Проверка что у клиента баланс не больше суммы которую надо оплатить,
                //иначе получится что баланс обнулится когда должно было что-то остаться
                CurrentSaleDocument.ClientBalancePartInTotal = CurrentClient.Balance;
                ClientsBll.SetBalanceToZero(CurrentClient.ID);
            }

            DocumentsBll.UpdateSaleProducts(SaleProductsList, Confirmed);
            
            if (Confirmed)
            {
                SettingsBll.AddCashToStorageBalance(CurrentSaleDocument.Total);
            }

            DocumentsBll.UpdateSaleDocument(CurrentSaleDocument);

        }

        /// <summary>
        /// Используется при раcпроведении документа
        /// </summary>
        private void UnConfirmDocument()
        {
            CurrentSaleDocument.Prepay = Prepay;
            CurrentSaleDocument.Confirmed = Confirmed;

            //Возвращаем продукты на полки
            ProductsBll.ReturnProductToStorage(SaleProductsList);

            //Достаем деньги из кассы
            SettingsBll.ReturnCashFromStorageBalance(CurrentSaleDocument.Total);

            //Возвращаем деньги клиенту
            if (CurrentSaleDocument.ClientBalancePartInTotal != 0)
            {
                ClientsBll.ReturnCash(CurrentSaleDocument.ClientId, CurrentSaleDocument.ClientBalancePartInTotal);
            }

            //Обновляем значения для документа
            DocumentsBll.UpdateSaleDocument(CurrentSaleDocument);
        }
        
        private void OpenSelection()
        {
            var dlg = new UniProductSelectorView();
            dlg.Show();
            dlg.Closed += DlgOnClosed;
        }

        private void DlgOnClosed(object sender, EventArgs eventArgs)
        {
            var dlg = sender as UniProductSelectorView;
            if (dlg == null) return;

            var vm = dlg.DataContext as UniProductSelectorViewModel;
            if (vm == null || vm.SelectedProducts.Count == 0)
                return;

            //Создаем в базе новый документ
            var newSaleDocument = DocumentsBll.AddSaleDocument(CurrentClient.ID);

            //Добавляем в документ продукты для продажи
            DocumentsBll.AddProductsToSaleDocument(newSaleDocument.Id, vm.SelectedProducts);

            //Добавляем в список документов на UI новый док
            SaleDocuments.Add(newSaleDocument);
            CurrentSaleDocument = newSaleDocument;
        }

        #endregion
    }
}

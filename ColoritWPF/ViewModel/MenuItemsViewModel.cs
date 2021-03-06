﻿using ColoritWPF.Views;
using ColoritWPF.Views.Products;
using ColoritWPF.Views.Statistics;
using GalaSoft.MvvmLight.Command;

namespace ColoritWPF.ViewModel
{
    public class MenuItemsViewModel
    {
        public MenuItemsViewModel()
        {
            AddNewClientCommand = new RelayCommand(AddNewClientCmd);
            AddNewCarModelCommand = new RelayCommand(AddNewCarModelCmd);
            EditClientCommand = new RelayCommand(EditClientCmd);
            EditPaintsCommand = new RelayCommand(EditPaintsCmd);
            SettingsCommand = new RelayCommand(SettingsCmd);
            PaintsSalesWatcherCommand = new RelayCommand(PaintsSalesWatcherCmd);
            AddNewDensityCommand = new RelayCommand(AddNewDensityCmd);
            AddNewProductCommand = new RelayCommand(AddNewProductCmd);
            EditProductCommand = new RelayCommand(EditProductCmd);
            PurchaseProductCommand = new RelayCommand(PurchaseProductCmd);
            ShowSalesStatsCommand = new RelayCommand(ShowStats);
            ShowImportFromExcelCommand = new RelayCommand(ShowImportFromExcelCmd);
        }



        #region fields

        public RelayCommand AddPaintCommand { get; private set; }

        public RelayCommand AddNewClientCommand { get; private set; }

        public RelayCommand EditClientCommand { get; private set; }

        public RelayCommand AddNewCarModelCommand { get; private set; }

        public RelayCommand EditPaintsCommand { get; private set; }

        public RelayCommand SettingsCommand { get; private set; }

        public RelayCommand PaintsSalesWatcherCommand { get; private set; }

        public RelayCommand AddNewDensityCommand { get; private set; }

        public RelayCommand AddNewProductCommand { get; private set; }

        public RelayCommand EditProductCommand { get; private set; }

        public RelayCommand PurchaseProductCommand { get; private set; }

        public RelayCommand ShowSalesStatsCommand { get; private set; }

        public RelayCommand ShowImportFromExcelCommand { get; private set; }

        #endregion

        private void ShowImportFromExcelCmd()
        {
            var dlg = new ImportFromExcel();
            dlg.ShowDialog();
        }

        private void PurchaseProductCmd()
        {
            PurchaseProductView purchaseProductView = new PurchaseProductView();
            purchaseProductView.ShowDialog();
        }

        private void EditProductCmd()
        {
            EditProductsView editProductsView = new EditProductsView();
            editProductsView.ShowDialog();
        }

        private void AddNewProductCmd()
        {
            AddNewProductView addNewProduct = new AddNewProductView();
            addNewProduct.ShowDialog();
        }
        
        private void AddNewDensityCmd()
        {
            AddNewDensityItem addNewDensity = new AddNewDensityItem();
            addNewDensity.ShowDialog();
        }

        private void PaintsSalesWatcherCmd()
        {
            PaintsSalesWatcherView paintsSalesWatcherView = new PaintsSalesWatcherView();
            paintsSalesWatcherView.ShowDialog();
        }

        private void SettingsCmd()
        {
            SettingsView settingsView = new SettingsView();
            settingsView.ShowDialog();
        }

        private void EditPaintsCmd()
        {
            PaintsEditor paintsEditor = new PaintsEditor();
            paintsEditor.ShowDialog();
        }
        
        private void EditClientCmd()
        {
            ClientEditor clientEditor = new ClientEditor();
            clientEditor.ShowDialog();
        }

        private void AddNewCarModelCmd()
        {
            AddNewCarModel addNewCar = new AddNewCarModel();
            addNewCar.ShowDialog();
        }

        private void AddNewClientCmd()
        {
            AddNewClient addNewClient = new AddNewClient();
            addNewClient.ShowDialog();
        }

        private void ShowStats()
        {
            var dlg = new SalesStats();
            dlg.ShowDialog();
        }
    }
}

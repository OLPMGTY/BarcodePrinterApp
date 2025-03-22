using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ZXing;
using ZXing.Common;
using System.Drawing;
using System.Windows.Interop;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BarcodePrinterApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly CsvRepository _repository = new CsvRepository();
        private ObservableCollection<Asset> _assets;
        private bool? _isAllSelected;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        public bool? IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (_isAllSelected == value) return;
                _isAllSelected = value;
                OnPropertyChanged(nameof(IsAllSelected));
                SetAllSelectionState(value ?? false);
            }
        }

        private void LoadData()
        {
            var assets = _repository.LoadAssets();
            _assets = new ObservableCollection<Asset>(assets);
            _assets.CollectionChanged += (s, e) => UpdateAllSelectionState();
            AssetsGrid.ItemsSource = _assets;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            int newId = _assets.Any() ? _assets.Max(a => a.Id) + 1 : 1;
            var newAsset = new Asset { Id = newId, InventoryNumber = "[Новый номер]", Name = "[Новое наименование]" };
            _assets.Add(newAsset);
            AssetsGrid.ScrollIntoView(newAsset);
            _repository.SaveAssets(_assets.ToList());
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (AssetsGrid.SelectedItem is Asset selectedAsset)
            {
                _assets.Remove(selectedAsset);
                _repository.SaveAssets(_assets.ToList());
            }
        }

        private BitmapSource GenerateBarcode(string inventoryNumber)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions { Height = 120, Width = 350 }
            };

            using (var bitmap = writer.Write(inventoryNumber))
            {
                return Imaging.CreateBitmapSourceFromHBitmap(
                    bitmap.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                );
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedAssets = _assets.Where(a => a.IsSelected).ToList();
            if (!selectedAssets.Any())
            {
                MessageBox.Show("Выберите хотя бы одну запись!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                foreach (var asset in selectedAssets)
                {
                    var label = CreateBarcodeLabel(asset);
                    printDialog.PrintVisual(label, $"Печать {asset.InventoryNumber}");
                }
            }
        }

        private BarcodeLabel CreateBarcodeLabel(Asset asset)
        {
            var label = new BarcodeLabel
            {
                DataContext = asset,
                Width = 300,
                Height = 250
            };
            label.BarcodeImage.Source = GenerateBarcode(asset.InventoryNumber);
            label.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            label.Arrange(new Rect(label.DesiredSize));
            return label;
        }

        public void UpdatePreview()
        {
            var selectedItems = _assets.Where(a => a.IsSelected).ToList();
            PreviewContainer.ItemsSource = selectedItems;

            foreach (var item in PreviewContainer.Items)
            {
                if (item is Asset asset)
                {
                    var container = PreviewContainer.ItemContainerGenerator.ContainerFromItem(item);
                    if (container is ContentPresenter presenter && presenter.FindName("BarcodeImage") is System.Windows.Controls.Image barcodeImage)
                    {
                        barcodeImage.Source = GenerateBarcode(asset.InventoryNumber);
                    }
                }
            }
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e) => SetAllSelectionState(true);

        private void DeselectAll_Click(object sender, RoutedEventArgs e) => SetAllSelectionState(false);

        private void SetAllSelectionState(bool isSelected)
        {
            foreach (var asset in _assets)
            {
                asset.IsSelected = isSelected;
            }
        }

        private void UpdateAllSelectionState()
        {
            if (_assets == null || !_assets.Any())
            {
                IsAllSelected = false;
                return;
            }

            var selectedCount = _assets.Count(a => a.IsSelected);
            IsAllSelected = selectedCount switch
            {
                0 => false,
                _ when selectedCount == _assets.Count => true,
                _ => null
            };
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Asset.IsSelected))
            {
                Dispatcher.BeginInvoke(new Action(UpdateAllSelectionState), DispatcherPriority.Background);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}



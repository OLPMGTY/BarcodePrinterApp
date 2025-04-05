using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;
using System.Windows.Interop;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BarcodePrinterApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region [Поля]
        private readonly XlsxRepository repository = new XlsxRepository(); // Репозиторий для загрузки данных
        private ObservableCollection<Asset> assets; // Коллекция активов
        private bool? isAllSelected; // Состояние выбора всех элементов
        #endregion

        #region [События]
        public event PropertyChangedEventHandler PropertyChanged; // Событие для уведомления об изменении свойств
        #endregion

        #region [Конструктор]
        public MainWindow()
        {
            InitializeComponent(); // Инициализация компонентов окна
            LoadData(); // Загрузка данных при создании окна
        }
        #endregion

        #region [Свойства]
        public bool? IsAllSelected
        {
            get => isAllSelected; // Получение состояния выбора всех элементов
            set
            {
                if (isAllSelected == value) return; // Если значение не изменилось, выходим
                isAllSelected = value; // Устанавливаем новое значение
                OnPropertyChanged(nameof(IsAllSelected)); // Уведомляем об изменении свойства
                SetAllSelectionState(value ?? false); // Устанавливаем состояние выбора для всех элементов
            }
        }
        #endregion

        #region [Публичные методы]
        public void UpdatePreview()
        {
            var selectedItems = assets.Where(a => a.IsSelected).ToList(); // Получаем выбранные элементы
            PreviewContainer.ItemsSource = selectedItems; // Обновляем источник данных для контейнера предпросмотра

            foreach (var asset in selectedItems)
            {
                var container = PreviewContainer.ItemContainerGenerator.ContainerFromItem(asset);
                if (container is ContentPresenter presenter && presenter.FindName("BarcodeImage") is System.Windows.Controls.Image barcodeImage)
                {
                    barcodeImage.Source = BarcodeGenerator.GenerateBarcode(asset.InventoryNumber); // Генерируем и устанавливаем штрих-код для выбранного элемента

                }
            }
        }
        #endregion

        #region [Обработчики событий]
        private void HeaderCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                SetAllSelectionState(checkBox.IsChecked == true); // Устанавливаем состояние выбора для всех элементов в зависимости от состояния CheckBox
            }
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                repository.SetFilePath(openFileDialog.FileName); // Устанавливаем путь к файлу
                LoadData(); // Перезагружаем данные
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (RadioButtonOS.IsChecked == true)
            {
                // Выбран "ОС"
                UpdateLabelText("ОС");
            }
            else if (RadioButtonTMS.IsChecked == true)
            {
                // Выбран "ТМС"
                UpdateLabelText("ТМЦ");
            }
        }

        //private void PrintButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // Получаем выбранные элементы
        //    var selectedItems = assets.Where(a => a.IsSelected).ToList();

        //    // Создаем контейнер для печати
        //    var printContainer = new StackPanel();

        //    // Создаем метки для каждого выбранного актива
        //    foreach (var asset in selectedItems)
        //    {
        //        var label = CreateBarcodeLabel(asset);
        //        printContainer.Children.Add(label);
        //    }

        //    // Создаем диалог печати
        //    var printDialog = new PrintDialog();
        //    if (printDialog.ShowDialog() == true)
        //    {
        //        // Устанавливаем размер страницы для печати
        //        printContainer.Measure(new System.Windows.Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight));
        //        printContainer.Arrange(new Rect(new System.Windows.Point(0, 0), printContainer.DesiredSize));

        //        // Печатаем содержимое
        //        printDialog.PrintVisual(printContainer, "Печать штрих-кодов");
        //    }
        //}
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранные элементы
            var selectedItems = assets.Where(a => a.IsSelected).ToList();

            // Создаем диалог печати
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true)
                return;

            // Печатаем каждый штрих-код на отдельной странице
            foreach (var asset in selectedItems)
            {
                // Создаем контейнер для печати (одна страница)
                var printContainer = new StackPanel();

                // Создаем метку для текущего актива
                var label = CreateBarcodeLabel(asset);
                printContainer.Children.Add(label);

                // Устанавливаем размер страницы для печати
                printContainer.Measure(new System.Windows.Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight));
                printContainer.Arrange(new Rect(new System.Windows.Point(0, 0), printContainer.DesiredSize));

                // Печатаем текущую страницу
                printDialog.PrintVisual(printContainer, $"Печать штрих-кода {asset.InventoryNumber}");
            }
        }
        #endregion

        #region [Приватные методы]

        private BarcodeLabel CreateBarcodeLabel(Asset asset)
        {
            var label = new BarcodeLabel
            {
                DataContext = asset,
                Width = 258,
                Height = 105
            };
            label.BarcodeImage.Source = BarcodeGenerator.GenerateBarcode(asset.InventoryNumber);
            label.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            label.Arrange(new Rect(label.DesiredSize));
            return label;
        }

        //private BarcodeLabel CreateBarcodeLabel(Asset asset)
        //{
        //    var label = new BarcodeLabel
        //    {
        //        DataContext = asset, // Устанавливаем контекст данных для метки
        //        /*Width = 300, // Устанавливаем ширину метки
        //        Height = 140 // Устанавливаем высоту метки*/
        //        Width = 258,
        //        Height = 105
        //    };
        //    label.BarcodeImage.Source = BarcodeGenerator.GenerateBarcode(asset.InventoryNumber); // Используем статический метод
        //    label.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity)); // Измеряем метку
        //    label.Arrange(new Rect(label.DesiredSize)); // Упорядочиваем метку
        //    return label; // Возвращаем созданную метку
        //}

        private void LoadData()
        {
            var assets = repository.LoadAssets(); // Загружаем данные из репозитория
            this.assets = new ObservableCollection<Asset>(assets); // Инициализируем коллекцию активов
            this.assets.CollectionChanged += (s, e) => UpdateAllSelectionState(); // Подписываемся на событие изменения коллекции
            AssetsGrid.ItemsSource = this.assets; // Устанавливаем источник данных для DataGrid
            UpdatePreview();
        }

        private void SetAllSelectionState(bool isSelected)
        {
            foreach (var asset in assets)
            {
                asset.IsSelected = isSelected; // Устанавливаем состояние выбора для каждого элемента
            }
        }

        private void UpdateAllSelectionState()
        {
            if (assets == null || !assets.Any())
            {
                IsAllSelected = false; // Если коллекция пуста, сбрасываем состояние выбора
                return;
            }

            var selectedCount = assets.Count(a => a.IsSelected); // Считаем количество выбранных элементов
            IsAllSelected = selectedCount switch
            {
                0 => false, // Ни один элемент не выбран
                _ when selectedCount == assets.Count => true, // Все элементы выбраны
                _ => null // Некоторые элементы выбраны, но не все
            };
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // Уведомляем об изменении свойства
        }

        private void UpdateLabelText(string labelText)
        {
            if (PreviewContainer == null || PreviewContainer.ItemsSource == null || !PreviewContainer.ItemsSource.OfType<object>().Any())
                return;

            foreach (var item in PreviewContainer.Items)
            {
                if (item is Asset asset)
                {
                    asset.LabelText = labelText;
                }
            }
        }
        #endregion
    }
}


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZXing.Common;
using ZXing;

namespace BarcodePrinterApp
{
    /// <summary>
    /// Interaction logic for BarcodeLabel.xaml
    /// </summary>
    public partial class BarcodeLabel : UserControl
    {
        public BarcodeLabel()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Обновляем штрих-код при изменении DataContext
            if (e.NewValue is Asset asset)
            {
                BarcodeImage.Source = BarcodeGenerator.GenerateBarcode(asset.InventoryNumber);

                // Отладочная информация
                Console.WriteLine($"DataContext changed for {asset.InventoryNumber}");
            }
        }
    }
}
                                            
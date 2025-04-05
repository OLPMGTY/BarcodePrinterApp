using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;

namespace BarcodePrinterApp
{
    public static class BarcodeGenerator
    {
        public static BitmapSource GenerateBarcode(string inventoryNumber)
        {
            try
            {
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions { Height = 90, Width = 370, PureBarcode = true}
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка генерации штрих-кода: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
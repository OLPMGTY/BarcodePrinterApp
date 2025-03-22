using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

public class Asset : INotifyPropertyChanged
{
    private string _name;
    private string _inventoryNumber;

    public int Id { get; set; }

    public string InventoryNumber
    {
        get => _inventoryNumber;
        set
        {
            _inventoryNumber = value;
            OnPropertyChanged();
        }
    }



    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();

            // Автоматически обновляем предпросмотр
            Application.Current.Dispatcher.Invoke(() =>
                (Application.Current.MainWindow as BarcodePrinterApp.MainWindow)?.UpdatePreview());
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

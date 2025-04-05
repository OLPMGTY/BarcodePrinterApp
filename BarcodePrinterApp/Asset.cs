using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

public class Asset : INotifyPropertyChanged
{
    private string name;
    private string inventoryNumber;

    public int Id { get; set; }

    public string InventoryNumber
    {
        get => inventoryNumber;
        set
        {
            inventoryNumber = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => name;
        set
        {
            name = value;
            OnPropertyChanged();
        }
    }

    private bool isSelected;
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
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

    private string _labelText = "ОС"; // По умолчанию "ОС"

    public string LabelText
    {
        get => _labelText;
        set
        {
            _labelText = value;
            OnPropertyChanged();
        }
    }
}

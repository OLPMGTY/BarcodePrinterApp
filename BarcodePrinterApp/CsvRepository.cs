using BarcodePrinterApp;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.IO;

public class CsvRepository
{
    private readonly string _csvPath = "assets.csv";

    public List<Asset> LoadAssets()
    {
        var assets = new List<Asset>();
        if (!File.Exists(_csvPath)) return assets;

        using (var parser = new TextFieldParser(_csvPath))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                if (fields.Length != 3) continue;

                assets.Add(new Asset
                {
                    Id = int.Parse(fields[0]),
                    InventoryNumber = fields[1],
                    Name = fields[2]
                });
            }
        }
        return assets;
    }

    public void SaveAssets(List<Asset> assets)
    {
        using (var writer = new StreamWriter(_csvPath))
        {
            foreach (var asset in assets)
            {
                writer.WriteLine($"{asset.Id},{asset.InventoryNumber},{asset.Name}");
            }
        }
    }
}
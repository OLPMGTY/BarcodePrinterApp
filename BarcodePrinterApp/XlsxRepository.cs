using BarcodePrinterApp;
using Microsoft.VisualBasic.FileIO;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

public class XlsxRepository
{
    private string _xlsxPath;

    public XlsxRepository()
    {
        _xlsxPath = "assets.xlsx"; // Путь по умолчанию
    }

    public void SetFilePath(string filePath)
    {
        _xlsxPath = filePath;
    }

    public List<Asset> LoadAssets()
    {
        var assets = new List<Asset>();
        if (!File.Exists(_xlsxPath)) return assets;

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context

        using (var package = new ExcelPackage(new FileInfo(_xlsxPath)))
        {
            var worksheet = package.Workbook.Worksheets[0]; // Получаем первый лист
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) // Начинаем с 2, если первая строка - заголовки
            {
                int id;
                if (int.TryParse(worksheet.Cells[row, 1].Text, out id))
                {
                    var inventoryNumber = worksheet.Cells[row, 2].GetValue<string>();
                    var name = worksheet.Cells[row, 3].GetValue<string>();

                    assets.Add(new Asset
                    {
                        Id = id,
                        InventoryNumber = inventoryNumber,
                        Name = name
                    });
                }
                else
                {
                    // Handle the case where the cell value is not a valid integer
                    // You can log the error or skip the row
                    continue;
                }
            }
        }
        return assets;
    }

    public void SaveAssets(List<Asset> assets)
    {
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Assets");

            // Записываем заголовки
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Инвентарный номер";
            worksheet.Cells[1, 3].Value = "Наименование";

            for (int i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                worksheet.Cells[i + 2, 1].Value = asset.Id;
                worksheet.Cells[i + 2, 2].Value = asset.InventoryNumber;
                worksheet.Cells[i + 2, 3].Value = asset.Name;
            }

            package.SaveAs(new FileInfo(_xlsxPath));
        }
    }
}
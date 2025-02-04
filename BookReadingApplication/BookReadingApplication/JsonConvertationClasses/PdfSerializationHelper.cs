using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace BookReadingApplication.JsonConvertationClasses;

/* Класс, сериализающий и десериализующий указанные файлы в указанную директорию */
public static class PdfSerializationHelper
{
    public static void SerializePdfFile(string pdfFilePath, string outputPath)
    {
        try
        {
            PdfFileInfo pdfInfo = new()
            {
                FileName = Path.GetFileName(pdfFilePath),
                FilePath = pdfFilePath,
                CreatedDate = File.GetCreationTime(pdfFilePath)
            };

            string json = pdfInfo.SerializeToJson();
            string outputFilePath = Path.Combine(outputPath, $"{pdfInfo.FileName}.json");

            File.WriteAllText(outputFilePath, json);
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
            return;
        }
    }

    public static PdfFileInfo DeserializePdfFile(string filePath)
    {
        string json = File.ReadAllText(filePath);

        return PdfFileInfo.DeserializeFromJson(json);
    }
}

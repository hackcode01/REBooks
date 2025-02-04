using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace BookReadingApplication.JsonConvertationClasses;

/* Класс создающий представление PDF файла в представление, достаточное для нашей задачи */
public class PdfFileInfo
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public DateTime CreatedDate { get; set; }

    public string? SerializeToJson()
    {
        try
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }

        return null;
    }

    public static PdfFileInfo DeserializeFromJson(string fileJson)
    {
        string json;
        using StreamReader readder = new(@fileJson);
        json = readder.ReadToEnd();

        return JsonConvert.DeserializeObject<PdfFileInfo>(json);
    }
}

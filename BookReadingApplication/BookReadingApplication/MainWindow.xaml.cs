using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using BookReadingApplication.Properties;

using BookReadingApplication.JsonConvertationClasses;
using System.Windows.Media;
using System.Threading.Tasks;

namespace BookReadingApplication;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        if (Settings.Default.LastOpenedPdfFile != "")
        {
            MessageBoxResult result = MessageBox.Show("Желаете загрузить последний " +
                "активный pdf файл?", "Подтверждение желания",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    {
                        MessageBox.Show("Загружаем последний активный файл");
                        pdfViewer.Load(Settings.Default.LastOpenedPdfFile);
                        break;
                    }
                case MessageBoxResult.No:
                    {
                        MessageBox.Show("Желаем приятного чтения");
                        break;
                    }
            }
        }
    }

    /* Выбор светлой темы */
    private void SelectLightTheme_Click(object sender, RoutedEventArgs args)
    {
        SelectTheme.ChangeTheme(new("Themes/LightTheme.xaml", UriKind.Relative));
    }

    /* Выбор темной темы */
    private void SelectDarkTheme_Click(object sender, RoutedEventArgs args)
    {
        SelectTheme.ChangeTheme(new("Themes/DarkTheme.xaml", UriKind.Relative));
    }

    /* Загрузка PDF файлов */
    private void LoadPdf_Click(object sender, RoutedEventArgs args)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "PDF файлы (*.pdf)|*.pdf"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            /* Асинхронный вызов */
            pdfViewer.Load(openFileDialog.FileName);

            string relativePath = "..\\..\\..\\JsonFiles\\";
            string fullPath = Path.GetFullPath(relativePath);
            string[] files = Directory.GetFiles(fullPath);

            foreach (var file in files)
            {
                PdfFileInfo pdf = PdfFileInfo.DeserializeFromJson(file);

                if (pdf.FileName == pdfViewer.DocumentInfo.FileName)
                {
                    StatusPDF.Content = "Файл уже открывался";
                    StatusColor.Color = Colors.Yellow;
                    return;
                }
            }

            StatusPDF.Content = "Файл еще не открывался";
            StatusColor.Color = Colors.Green;
        }
    }

    /* Открытие директории с сохраненными файлами */
    private async void OpenDirectory_Click(object sender, RoutedEventArgs args)
    {
        /* Получение относительного пути к диретории */
        string relativePath = "..\\..\\..\\SaveBooks\\";

        /* Получение абсолютного пути до директории */
        string fullPath = Path.GetFullPath(relativePath);

        if (Directory.Exists(fullPath))
        {
            /* Асинхронный вызов */
            await Task.Run(() => Process.Start("explorer.exe", fullPath));
        }
        else
        {
            Directory.CreateDirectory(fullPath);
            await Task.Run(() => Process.Start("explorer.exe", fullPath));
        }
    }

    /* Сохранение файла в директорию */
    private void UploadPdf_click(object sender, RoutedEventArgs args)
    {
        if (pdfViewer.IsLoaded)
        {
            string relativePath = "..\\..\\..\\SaveBooks\\";
            string fullPath = Path.GetFullPath(relativePath);

            string filePath = $"{pdfViewer.DocumentInfo.FilePath}" +
                $"{pdfViewer.DocumentInfo.FileName}";
            string newFilePath = $"{fullPath}\\{pdfViewer.DocumentInfo.FileName}";

            FileInfo fileInfo = new(filePath);

            if (!File.Exists(newFilePath))
            {
                fileInfo.CopyTo(newFilePath);
            }

            MessageBox.Show("PDF файл был успешно загружен");
        }
        else
        {
            MessageBox.Show("PDF файл не загружен");
        }
    }

    /* Выход из приложения через меню */
    private void Exit_Click(object sender, RoutedEventArgs args)
    {
        MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти?",
            "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question);

        switch (result)
        {
            case MessageBoxResult.Yes:
                {
                    string pdfFilePath = $"{pdfViewer.DocumentInfo.FilePath}" +
                        $"{pdfViewer.DocumentInfo.FileName}";

                    string relativePath = "..\\..\\..\\JsonFiles\\";
                    string fullPath = Path.GetFullPath(relativePath);

                    PdfSerializationHelper.SerializePdfFile(pdfFilePath, fullPath);

                    /* Сохранение последнего открытого файла в параметры */
                    Settings.Default.LastOpenedPdfFile = pdfFilePath;
                    Settings.Default.Save();

                    MessageBox.Show("Сериализация PDF файла прошла успешно");

                    Application.Current.Shutdown();
                    break;
                }
            case MessageBoxResult.No:
                {
                    break;
                }
        }
    }

    /* Выход из приложения нажатием на крестик окна */
    protected override void OnClosing(CancelEventArgs args)
    {
        if (pdfViewer.IsLoaded && (pdfViewer.DocumentInfo.FileName == null))
        {
            string pdfFilePath = $"{pdfViewer.DocumentInfo.FilePath}" +
                $"{pdfViewer.DocumentInfo.FileName}";

            string relativePath = "..\\..\\..\\JsonFiles\\";
            string fullPath = Path.GetFullPath(relativePath);

            PdfSerializationHelper.SerializePdfFile(pdfFilePath, fullPath);

            /* Сохранение последнего открытого файла в параметры */
            Settings.Default.LastOpenedPdfFile = pdfFilePath;
            Settings.Default.Save();

            MessageBox.Show("Сериализация PDF файла прошла успешно");
        }
    }
}

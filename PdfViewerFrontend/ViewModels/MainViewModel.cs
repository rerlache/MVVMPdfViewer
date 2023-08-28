using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using PdfViewerFrontend.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfViewerFrontend.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly string _pdfDirectory = @"C:\temp\PDFs\";
        private List<PdfDocument> _pdfFiles;
        public List<PdfDocument> PdfFiles
        {
            get { return _pdfFiles; }
            set { _pdfFiles = value; OnPropertyChanged(); }
        }
        private PdfDocument _selectedPdfFile;
        public PdfDocument SelectedPdfFile
        {
            get { return _selectedPdfFile; }
            set
            {
                _selectedPdfFile = value;
                CurrentPdfViewerWindow.Title = value.FullPath;
                if (CurrentPdfViewerWindow.PdfViewer != null)
                {
                    CurrentPdfViewerWindow.PdfViewer.Reload();
                    CurrentPdfViewerWindow.PdfViewer.Source = new Uri(value.FullPath);
                }
                OnPropertyChanged();
            }
        }
        public MainWindow CurrentPdfViewerWindow { get; set; }


        public MainViewModel()
        {
            MainWindow mainWindow = App.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            CurrentPdfViewerWindow = mainWindow == null ? new() : mainWindow;
            GetPdfFiles();
            SelectedPdfFile = PdfFiles.First();
        }

        private void GetPdfFiles()
        {
            PdfFiles = new();
            foreach (string file in System.IO.Directory.GetFiles(_pdfDirectory))
            {
                PdfDocument pdfDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                pdfDocument.Info.Title = file.Substring(file.LastIndexOf('\\') + 1);
                PdfFiles.Add(pdfDocument);
            }
        }
    }
}

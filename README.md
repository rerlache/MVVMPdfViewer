# MVVMPdfViewer
As i needed a PDF Previewer within my WPF application, i tried to find some kind of guide/how-to to solve that request. I wasn't able to find a working one, so i tried to figure out how i could build that myself.

While researching, trying out a lot of suggested solutions for the "WPF-PDF Viewer" and a lot of struggles, i decided that i want to share my solution with the community, to give something back. So i created this demo project and documentation ;)

This is my first kind of demo project i want to share especially with that kind of documentation.

This Demo project has some potential for upgrades, especially with the pdffile directory, i know that, but it is quite similar to the solution i implemented in my Project. I wanted it to work as expected, so i don't put work into the xaml to make it nice and the hard coded directory path also works fine (for now), as i create the pdf's with the same application in the same folder ;)

I hope this helps anyone save some time :)

## Needed Packages
* Microsoft.Web.WebView2
* PdfSharpCore

## Instructions
1. create a new Project
2. add the nuget packages mentioned above
3. create the folder MVVM structure (Models not neede in this demo project)
4. to keep as near as possible to the MVVM pattern, i moved the MainWindow.xaml file into the Views folder and adapted the StartupUri in the App.xaml file
5. create the BaseViewModel.cs and implement INotifyPropertyChanged
6. create the MainViewModel.cs file and inherit from BaseViewModel
### MainViewModel.cs
7. implement the installed packages:
    ```
    using PdfSharpCore.Pdf;
    using PdfSharpCore.Pdf.IO;
    using PdfViewerFrontend.Views;
    ```
8. create the needed properties
   1. _pdfDirectory<br>`private readonly string _pdfDirectory = @"C:\temp\PDFs\";`
   2. PdfFiles<br>
        ```
        private List<PdfDocument> _pdfFiles;
        public List<PdfDocument> PdfFiles
        {
            get { return _pdfFiles; }
            set { _pdfFiles = value; OnPropertyChanged(); }
        }
        ```
   3. SelectedPdfFile<br>
        ```
        private PdfDocument _selectedPdfFile;
        public PdfDocument SelectedPdfFile
        {
            get { return _selectedPdfFile; }
            set { _selectedPdfFile = value; OnPropertyChanged(); }
        }
        ```
   4. CurrentPdfViewerWindow<br>`public MainWindow CurrentPdfViewerWindow { get; set; }`
9. create a function to fill the PdfFiles<br>
    ```
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
    ```
10. implement the constructor like this<br>
    ```
    public MainViewModel()
    {
        MainWindow mainWindow = App.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        CurrentPdfViewerWindow = mainWindow == null ? new() : mainWindow;
        GetPdfFiles();
        SelectedPdfFile = PdfFiles.First();
    }
    ```
11. last but not least, update the setter of the SelectedPdfFile<br>
    ```
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
    ```
### MainWindow.xaml
12. add namespaces to the MainWindow.xaml
    ```
    xmlns:Wpf="clr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.Wpf"
    xmlns:vm="clr-namespace:PdfViewerFrontend.ViewModels"
    ```
13. connect the MainViewModel as DataContext to the MainWindow and add a basic grid layout<br>
    ```
    <Window.DataContext>
        <vm:MainViewModel />
    </Window.DataContext>
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="1" />
            <ColumnDefinition Width="4*" />
        </Grid.ColumnDefinitions>
        <ListView Grid.Column="0" ItemsSource="{Binding PdfFiles}" SelectedItem="{Binding SelectedPdfFile, UpdateSourceTrigger=PropertyChanged}" DisplayMemberPath="Info.Title" />
        <GridSplitter Grid.Column="1" HorizontalAlignment="Stretch" Background="Black" />
        <Wpf:WebView2 x:Name="PdfViewer" Grid.Column="2" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Source="{Binding SelectedPdfFile.FullPath, UpdateSourceTrigger=PropertyChanged}" />
    </Grid>
    ```
14. feel free to refactor the code.
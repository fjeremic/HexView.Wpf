namespace HexView.Wpf.Demo
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private BinaryReader binaryReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Generate random data so we display something right out of the box without forcing the user to open a file
            var rand = new Random();

            // 10 MB of random data
            var bytes = new byte[10 * 1024 * 1024];
            rand.NextBytes(bytes);

            Reader = new BinaryReader(new MemoryStream(bytes));
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the source of the data to display using the <see cref="HexViewer"/> control.
        /// </summary>
        public BinaryReader Reader
        {
            get => binaryReader;

            set
            {
                binaryReader = value;

                OnPropertyChanged();
            }
        }

        private void MenuItem_Open(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                var file = File.Open(openFileDialog.FileName, FileMode.Open);
                Reader = new BinaryReader(file);
            }
        }

        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

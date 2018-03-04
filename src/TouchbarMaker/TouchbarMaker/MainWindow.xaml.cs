using System.Windows;
using TouchbarMaker.ViewModels;

namespace TouchbarMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel MainViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MainViewModel = new MainViewModel(TreeView) {ApplicationName = "devenv.exe"};
            DataContext = MainViewModel;
        }
    }
}

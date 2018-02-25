using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using TouchbarMaker.Core;

namespace TouchbarMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select a picture";
            fileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphic (*.png)|*.png";

            if (fileDialog.ShowDialog() == true)
            {
                var image = new BitmapImage(new Uri(fileDialog.FileName));
                var bitmap = Tools.Converter.ConvertBitmapImage(image);

                if (!bitmap.IsAcceptedIconSize())
                {
                    MessageBox.Show("The image has a bad format.");
                    return;
                }

                var encoded = bitmap.ToEncodedIconFromBitmap();
            }
        }
    }
}

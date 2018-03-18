using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace TouchbarMaker.Views
{
    public partial class ChangeNameDialog : MetroWindow
    {
        public string ElementName { get; set; }

        public ChangeNameDialog(string name)
        {
            InitializeComponent();
            ElementName = name;
            DataContext = this;
            NameInput.Focus();
            NameInput.Loaded += (sender, args) => NameInput.SelectAll();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void OnKeyInTextEntryPressed(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    this.DialogResult = true;
                    break;
                case Key.Escape:
                    this.DialogResult = false;
                    break;
                default:
                    break;
            }
        }
    }
}

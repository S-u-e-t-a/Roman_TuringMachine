using System.Windows;

namespace Turing
{
    /// <summary>
    ///     Логика взаимодействия для AddSymbolWindow.xaml
    /// </summary>
    public partial class AddSymbolWindow : Window
    {
        public AddSymbolWindow(string alphabet)
        {
            InitializeComponent();
            DataContext = new AddSymbolVm(alphabet);
        }
    }
}
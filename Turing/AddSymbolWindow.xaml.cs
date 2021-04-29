using System.Windows;

namespace Turing
{
    /// <summary>
    ///     Логика взаимодействия для AddSymbolWindow.xaml
    /// </summary>
    public partial class AddSymbolWindow : Window
    {
        public AddSymbolWindow(string Alphabet)
        {
            InitializeComponent();
            DataContext = new AddSymbolVM(Alphabet);
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Shapes;
using Gu.Wpf.DataGrid2D;

namespace Turing
{
    /// <summary>
    /// Логика взаимодействия для AddSymbolWindow.xaml
    /// </summary>
    public partial class AddSymbolWindow : Window
    {
        public AddSymbolWindow(string Alphabet)
        {
            InitializeComponent();
            this.DataContext = new AddSymbolVM(Alphabet);
        }

    }
}

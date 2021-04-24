using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Turing
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowVM();

        }

        private void DataWindow_Closing(object o, CancelEventArgs e)
        {
            /*
            Trace.WriteLine("========================================");
            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions['0'][0]);
            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions['0'][1]);
            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions['0'][2]);
            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions['0'][3]);
            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions['0'][4]);

            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions['1'][0]);
            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions['1'][1]);
            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions['1'][2]);
            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions['1'][3]);
            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions['1'][4]);

            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions[' '][0]);
            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions[' '][1]);
            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions[' '][2]);
            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions[' '][3]);
            Trace.WriteLine((this.DataContext as MainWindowVM).Machine.Instructions[' '][4]);*/
            //СДЕЛАТЬ НОРМАЛЬНЫЙ БИНДИНГ ТАБЛИЦЫ С ИНСТРУКЦИЯМИИ А ТО ТАМ УАНВЭЙ БИНДИНГ БЛЯТЬ А НАДО ТУВЭЙ

        }

    }
}

using System.ComponentModel;
using System.Windows;

namespace Turing
{
    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowVM();
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
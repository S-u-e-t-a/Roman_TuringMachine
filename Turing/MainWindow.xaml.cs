using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private TuringMachine machine = new TuringMachine();

        //private List<TapeItem> tapeItems;
        public List<TapeItem> TapeItems {
            get { return machine.TapeItems; }
            set
            {
                machine.TapeItems = value;
                OnPropertyChanged("TapeItems");
            }
        }
        public MainWindow()
        {
            InitializeComponent();


            #region filling

            machine.CurrentIndex = 99;
            machine.Instructions = new Dictionary<char, List<Comm>>();

            machine.Instructions['0'] = new List<Comm>();
            machine.Instructions['1'] = new List<Comm>();
            machine.Instructions[' '] = new List<Comm>();

            machine.Instructions['0'].Add(null);
            machine.Instructions['0'].Add(null);
            machine.Instructions['1'].Add(null);
            machine.Instructions['1'].Add(null);
            machine.Instructions[' '].Add(null);
            machine.Instructions[' '].Add(null);

            machine.Instructions['0'][1] = new Comm("1>2");
            machine.Instructions['1'][1] = new Comm("0>2");
            machine.Instructions[' '][0] = new Comm(" >2");
            machine.Instructions[' '][1] = new Comm(" >0");

            for (int i = -100; i < 100; i++)
            {
                machine.TapeItems.Add(new TapeItem(){Index = i, Letter = ' '});
            }

            machine.TapeItems[100].Letter = '1';
            machine.TapeItems[101].Letter = '0';
            machine.TapeItems[102].Letter = '1';
            machine.TapeItems[103].Letter = '1';
            machine.TapeItems[104].Letter = '0';
            machine.TapeItems[105].Letter = '1';

            #endregion

            Tape.ItemsSource = machine.TapeItems;



            int count = 0;
            foreach (var lst in machine.Instructions.Values)
            {
                if (lst.Count > count)
                {

                    for (int i = count; i < lst.Count; i++)
                    {
                        DataGridTextColumn column = new DataGridTextColumn();
                        column.Header = "Q" + i;
                        column.Binding = new Binding(string.Format("Value[{0}]", i));
                        dg.Columns.Add(column);
                    }
                    count = lst.Count;
                }
            }

            dg.ItemsSource = machine.Instructions;

        }

        private void FillTape()
        {
            //Tape.
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            machine.Calc();
            machine.CurrentIndex = 99;

        }
    }
}

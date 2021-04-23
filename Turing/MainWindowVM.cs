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
using System.Windows.Input;

namespace Turing
{
    class MainWindowVM : INotifyPropertyChanged
    {
        #region Fields

       /* public List<List<char>> kkk
        {
            get
            {
                return new List<List<char>>() {Machine.TapeItems.ToList()};
            }
        }*/



        private double top;
        private double left;

        public double Top
        {
            get { return top; }
            set
            {
                top = value;
                OnPropertyChanged();
            }
        }

        public double Left
        {
            get { return left; }
            set
            {
                left = value;
                OnPropertyChanged();
            }
        }

        private double _panelX;
        private double _panelY;

        public double PanelX
        {
            get { return _panelX; }
            set
            {
                _panelX = value;
                OnPropertyChanged();
            }
        }

        public double PanelY
        {
            get { return _panelY; }
            set
            {
                _panelY = value;
                OnPropertyChanged();
            }
        }

        private TuringMachine machine;

        public TuringMachine Machine

        {
            get => machine;
            set
            {
                machine = value;
                OnPropertyChanged();
            }
        }

        public List<string> ColumnHeaders
        {
            get
            {
                List<string> headers = new List<string>();
                for (int i = 0; i < Machine.Instructions.Keys.Count; i++)
                {
                    headers.Add($"Q{i + 1}");
                }
                return headers;
            }
        }
        public List<string> RowHeaders
        {
            get
            {
                return Machine.Instructions.Keys.ToList().Select(n => n.ToString()).ToList();
            }
        }


        private object selectedItem;
        public object SelectedItem
        {
            get => this.selectedItem;

            set
            {
                if (Equals(value, this.selectedItem))
                {
                    return;
                }

                this.selectedItem = value;
                this.OnPropertyChanged();
            }
        }


        private int selectedIndex;
        public int SelectedIndex
        {
            get => this.selectedIndex;

            set
            {
                if (Equals(value, this.selectedIndex))
                {
                    return;
                }

                this.selectedIndex = value;
                this.OnPropertyChanged();
            }
        }

        private TapeItem selectedTapeItem;
        public TapeItem SelectedTapeItem
        {
            get => this.selectedTapeItem;

            set
            {
                if (Equals(value, this.selectedTapeItem))
                {
                    return;
                }

                this.selectedTapeItem = value;
                this.OnPropertyChanged();
            }
        }


        #endregion

        RelayCommand calcCommand;
        public RelayCommand CalcCommand
        {
            get
            {
                return calcCommand ??
                       (calcCommand = new RelayCommand((o) =>
                       {
                           Machine.CurrentIndex = 99;
                           Machine.Calc();
                           Machine.CurrentIndex = 99;
                       }));
            }
        }

        RelayCommand cellSelectedCommand;
        public RelayCommand CellSelectedCommand
        {
            get
            {
                return cellSelectedCommand ??
                       (cellSelectedCommand = new RelayCommand((o) =>
                       {
                           if (SelectedIndex != -1)
                           {
                               var window = new AddSymbolWindow(Machine.Alpabet);

                               window.Left = PanelX + Left;
                               window.Top = PanelY + Top;

                               window.ShowDialog();
                               SelectedTapeItem.Letter = (window.DataContext as AddSymbolVM).SelectedItem;
                               SelectedIndex = -1;
                           }
                           
                       }));
            }
        }


        //private List<TapeItem> tapeItems;
        public MainWindowVM()
        {
            #region filling

            Machine = new TuringMachine();
            Machine.CurrentIndex = 99;
            Machine.Alpabet = "01 ";
            Machine.Instructions = new Dictionary<char, List<string>>();

            Machine.Instructions['0'] = new List<string>();
            Machine.Instructions['1'] = new List<string>();
            Machine.Instructions[' '] = new List<string>();

            Machine.Instructions['0'].Add(null);
            Machine.Instructions['0'].Add(null);
            Machine.Instructions['1'].Add(null);
            Machine.Instructions['1'].Add(null);
            Machine.Instructions[' '].Add(null);
            Machine.Instructions[' '].Add(null);

            Machine.Instructions['0'][1] = "1>2";
            Machine.Instructions['1'][1] = "0>2";
            Machine.Instructions[' '][0] = " >2";
            Machine.Instructions[' '][1] = " >0";

            for (int i = -100; i < 100; i++)
            {
                Machine.TapeItems.Add(new TapeItem() {Color = "#ffffff", Index = i, Letter = ' '});
            }

            Machine.TapeItems[100].Letter = '1';
            Machine.TapeItems[101].Letter = '0';
            Machine.TapeItems[102].Letter = '1';
            Machine.TapeItems[103].Letter = '1';
            Machine.TapeItems[104].Letter = '0';
            Machine.TapeItems[105].Letter = '1';

            #endregion
            

            Trace.WriteLine("------------------------------------------------");
            Trace.WriteLine(Machine.Instructions.Keys);

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

        private void Tape_SelectionChanged(object sender, DataGridBeginningEditEventArgs e)
        {
            machine.Alpabet = "01 ";
            //var window = new AddSymbolWindow(machine.Alpabet);

            //window.Left = Mouse.GetPosition(this).X + this.Left;
            //window.Top = Mouse.GetPosition(this).Y + this.Top;
            //window.ShowDialog();

        }


    }
}

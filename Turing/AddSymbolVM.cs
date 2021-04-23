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
using Gu.Wpf.DataGrid2D;

namespace Turing
{
    class AddSymbolVM : INotifyPropertyChanged
    {

        RelayCommand cellSelectedCommand;

        private char selectedItem;
        public char SelectedItem
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


        private List<List<string>> symbolList;

        public List<List<string>> SymbolList
        {
            get => symbolList;
            set
            {
                symbolList = value;
                OnPropertyChanged();
            }
        }



        private int gridWidth;
        private int gridHeight;
        private int windowWidth;
        private int windowHeight;


        public int GridWidth
        {
            get => gridWidth;
            set
            {
                gridWidth = value;
                OnPropertyChanged();
            }
        }
        public int GridHeight
        {
            get => gridHeight;
            set
            {
                gridHeight = value;
                OnPropertyChanged();
            }
        }
        public int WindowWidth
        {
            get => windowWidth;
            set
            {
                windowWidth = value;
                OnPropertyChanged();
            }
        }
        public int WindowHeight
        {
            get => windowHeight;
            set
            {
                windowHeight = value;
                OnPropertyChanged();
            }
        }


        public RelayCommand CellSelectedCommand
        {
            get
            {
                return cellSelectedCommand ??
                       (cellSelectedCommand = new RelayCommand((o) =>
                       {
                           foreach (Window item in Application.Current.Windows)
                           {
                               if (item.DataContext == this) item.Close();
                           }
                       }));
            }
        }

        public AddSymbolVM(string alphabet)
        {
            this.SymbolList = new List<List<string>>(){ alphabet.ToCharArray().Select(n => n.ToString()).ToList() };
            int j;
            if (this.SymbolList[0].Count < 12) // если элементов слишком мало для отображения названия окна
            {
                j = 12;
            }
            else
            {
                j = this.SymbolList[0].Count;
            }
            GridWidth = j * 20 + 2; // 20 это размер ячейки если в ней один символ, а 2 это размер границы
            GridHeight = 20; // 20 стандартная высота
            //symbols.ColumnWidth= DataGridLength.SizeToCells;

            WindowWidth = GridWidth + 15; // 15 это ширина окна стандартная вроде как
            WindowHeight = GridHeight + 40; // 40 это высота полочки над окном
            int a = 2;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}

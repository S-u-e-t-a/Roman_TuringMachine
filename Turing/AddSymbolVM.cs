using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Turing
{
    internal class AddSymbolVM : INotifyPropertyChanged
    {
        private RelayCommand cellSelectedCommand;
        private int gridHeight;


        private int gridWidth;

        private char selectedItem;


        private List<List<string>> symbolList;
        private int windowHeight;
        private int windowWidth;

        public AddSymbolVM(string alphabet)
        {
            SymbolList = new List<List<string>> {alphabet.ToCharArray().Select(n => n.ToString()).ToList()};
            int j;
            if (SymbolList[0].Count < 12) // если элементов слишком мало для отображения названия окна
            {
                j = 12;
            }
            else
            {
                j = SymbolList[0].Count;
            }

            GridWidth = j * 20 + 2; // 20 это размер ячейки если в ней один символ, а 2 это размер границы
            GridHeight = 20; // 20 стандартная высота
            //symbols.ColumnWidth= DataGridLength.SizeToCells;

            WindowWidth = GridWidth + 15; // 15 это ширина окна стандартная вроде как
            WindowHeight = GridHeight + 40; // 40 это высота полочки над окном
            int a = 2;
        }

        public char SelectedItem
        {
            get => selectedItem;

            set
            {
                if (Equals(value, selectedItem))
                {
                    return;
                }

                selectedItem = value;
                OnPropertyChanged();
            }
        }

        public List<List<string>> SymbolList
        {
            get => symbolList;
            set
            {
                symbolList = value;
                OnPropertyChanged();
            }
        }


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
                       (cellSelectedCommand = new RelayCommand(o =>
                       {
                           foreach (Window item in Application.Current.Windows)
                           {
                               if (item.DataContext == this) item.Close();
                           }
                       }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
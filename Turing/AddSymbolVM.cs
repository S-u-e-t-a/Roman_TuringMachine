using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Turing
{
    internal class AddSymbolVm : INotifyPropertyChanged
    {
        private RelayCommand _cellSelectedCommand;
        private int _gridHeight;


        private int _gridWidth;

        private char _selectedItem;


        private List<List<string>> _symbolList;
        private int _windowHeight;
        private int _windowWidth;

        public AddSymbolVm(string alphabet)
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
            get => _selectedItem;

            set
            {
                if (Equals(value, _selectedItem))
                {
                    return;
                }

                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public List<List<string>> SymbolList
        {
            get => _symbolList;
            set
            {
                _symbolList = value;
                OnPropertyChanged();
            }
        }


        public int GridWidth
        {
            get => _gridWidth;
            set
            {
                _gridWidth = value;
                OnPropertyChanged();
            }
        }

        public int GridHeight
        {
            get => _gridHeight;
            set
            {
                _gridHeight = value;
                OnPropertyChanged();
            }
        }

        public int WindowWidth
        {
            get => _windowWidth;
            set
            {
                _windowWidth = value;
                OnPropertyChanged();
            }
        }

        public int WindowHeight
        {
            get => _windowHeight;
            set
            {
                _windowHeight = value;
                OnPropertyChanged();
            }
        }


        public RelayCommand CellSelectedCommand
        {
            get
            {
                return _cellSelectedCommand ??
                       (_cellSelectedCommand = new RelayCommand(o =>
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
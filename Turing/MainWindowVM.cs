using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Gu.Wpf.DataGrid2D;

namespace Turing
{
    internal class MainWindowVm : INotifyPropertyChanged
    {
        #region Fields

        #region FieldsForMouse

        private double _top;
        private double _left;

        public double Top
        {
            get => _top;
            set
            {
                _top = value;
                OnPropertyChanged();
            }
        }

        public double Left
        {
            get => _left;
            set
            {
                _left = value;
                OnPropertyChanged();
            }
        }

        private double _panelX;
        private double _panelY;

        public double PanelX
        {
            get => _panelX;
            set
            {
                _panelX = value;
                OnPropertyChanged();
            }
        }

        public double PanelY
        {
            get => _panelY;
            set
            {
                _panelY = value;
                OnPropertyChanged();
            }
        }

        #endregion


        private bool _isPauseButtonEnabled;
        private bool _isStopButtonEnabled;
        private bool _isStartButtonEnabled;


        public bool IsPauseButtonEnabled
        {
            get => _isPauseButtonEnabled;
            set
            {
                _isPauseButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsStopButtonEnabled
        {
            get => _isStopButtonEnabled;
            set
            {
                _isStopButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsStartButtonEnabled
        {
            get => _isStartButtonEnabled;
            set
            {
                _isStartButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        private TuringMachine _machine;

        public TuringMachine Machine

        {
            get => _machine;
            set
            {
                _machine = value;
                OnPropertyChanged();
            }
        }

        public List<string> ColumnHeaders
        {
            get
            {
                int lenofinstrutions =
                    Machine.Instructions.ElementAt(0).Value
                        .Count; // ультра говнокод (получаем количество элементов в массиве)
                List<string> headers = new List<string>();
                for (int i = 0; i < lenofinstrutions; i++)
                {
                    headers.Add($"Q{i + 1}");
                }

                return headers;
            }
        }

        public List<string> RowHeaders
        {
            get { return Machine.Instructions.Keys.ToList().Select(n => n.ToString()).ToList(); }
        }


        private object _selectedItem;

        public object SelectedItem
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


        private int _selectedIndex;

        public int SelectedIndex
        {
            get => _selectedIndex;

            set
            {
                if (Equals(value, _selectedIndex))
                {
                    return;
                }

                _selectedIndex = value;
                OnPropertyChanged();
            }
        }


        private RowColumnIndex _selectedIndexIns;

        public RowColumnIndex SelectedIndexIns
        {
            get => _selectedIndexIns;

            set
            {
                if (Equals(value, _selectedIndexIns))
                {
                    return;
                }

                _selectedIndexIns = value;
                OnPropertyChanged();
            }
        }


        private TapeItem _selectedTapeItem;

        public TapeItem SelectedTapeItem
        {
            get => _selectedTapeItem;

            set
            {
                if (Equals(value, _selectedTapeItem))
                {
                    return;
                }

                _selectedTapeItem = value;
                OnPropertyChanged();
            }
        }

        /*
        public bool isPauseButtonEnabled;
        public bool isStopButtonEnabled;
        public bool isStartButtonEnabled;
        public bool IsPauseButtonEnabled
        {
            set
            {
                if (Machine.CurrentState == States.working)
                {
                    isPauseButtonEnabled = true;
                }
                else
                {
                    isPauseButtonEnabled = false;
                }
                OnPropertyChanged();
            }
            get => isPauseButtonEnabled;

        }

        public bool IsStopButtonEnabled
        {
            set
            {
                if (Machine.CurrentState != States.stopped)
                {
                    isStopButtonEnabled = true;
                }
                else
                {
                    isStopButtonEnabled = false;
                }
                OnPropertyChanged();
            }
            get => isPauseButtonEnabled;
        }

        public bool IsStartButtonEnabled
        {
            set
            {
                if (Machine.CurrentState != States.working)
                {
                    isStartButtonEnabled = true;
                }
                else
                {
                    isStartButtonEnabled = false;
                }
                OnPropertyChanged();
            }
            get => isPauseButtonEnabled;
        }*/

        #endregion

        #region Commands

        #region ClaculationsCommands

        private RelayCommand _calcCommand;

        public RelayCommand CalcCommand
        {
            get
            {
                return _calcCommand ??
                       (_calcCommand = new RelayCommand(o =>
                       {
                           //Machine.CurrentIndex = 99;
                           Machine.Calc();
                           //Machine.CurrentIndex = 99;
                       }));
            }
        }

        private RelayCommand _stepCommand;

        public RelayCommand StepCommand
        {
            get
            {
                return _stepCommand ??
                       (_stepCommand = new RelayCommand(o => { Machine.MakeStep(); }));
            }
        }

        #endregion

        #region TapeCommands

        private RelayCommand _moveLeftCommand;

        public RelayCommand MoveLeftCommand
        {
            get
            {
                return _moveLeftCommand ??
                       (_moveLeftCommand = new RelayCommand(o => { Machine.CurrentIndex -= 1; }));
            }
        }

        private RelayCommand _moveRightCommand;

        public RelayCommand MoveRightCommand
        {
            get
            {
                return _moveRightCommand ??
                       (_moveRightCommand = new RelayCommand(o => { Machine.CurrentIndex += 1; }));
            }
        }


        private RelayCommand _cellSelectedCommand;

        public RelayCommand CellSelectedCommand
        {
            get
            {
                return _cellSelectedCommand ??
                       (_cellSelectedCommand = new RelayCommand(o =>
                       {
                           if (SelectedIndex != -1)
                           {
                               var window = new AddSymbolWindow(Machine.Alphabet);

                               window.Left = PanelX + Left;
                               window.Top = PanelY + Top;

                               window.ShowDialog();
                               SelectedTapeItem.Letter = (window.DataContext as AddSymbolVm).SelectedItem;
                               SelectedIndex = -1;
                           }
                       }));
            }
        }

        #endregion

        #region InstructionsCommands

        private RelayCommand _pauseCommand;

        public RelayCommand PauseCommand
        {
            get
            {
                return _pauseCommand ??
                       (_pauseCommand = new RelayCommand(o =>
                       {
                           Machine.CurrentState = States.Paused;
                           OnPropertyChanged();
                       }));
            }
        }

        private RelayCommand _stopCommand;

        public RelayCommand StopCommand
        {
            get
            {
                return _stopCommand ??
                       (_stopCommand = new RelayCommand(o =>
                       {
                           Machine.CurrentState = States.Stopped;
                           OnPropertyChanged();
                       }));
            }
        }

        private RelayCommand _addLeftCommand;

        public RelayCommand AddLeftCommand
        {
            get
            {
                return _addLeftCommand ??
                       (_addLeftCommand = new RelayCommand(o =>
                       {
                           Machine.AddColumnLeft(SelectedIndexIns.Column);
                           OnPropertyChanged();
                           OnPropertyChanged("ColumnHeaders");
                       }));
            }
        }


        private RelayCommand _addRightCommand;

        public RelayCommand AddRightCommand
        {
            get
            {
                return _addRightCommand ??
                       (_addRightCommand = new RelayCommand(o =>
                       {
                           Machine.AddColumnRight(SelectedIndexIns.Column);
                           OnPropertyChanged();
                           OnPropertyChanged("ColumnHeaders");
                       }));
            }
        }

        private RelayCommand _delColumnCommand;

        public RelayCommand DelColumnCommand
        {
            get
            {
                return _delColumnCommand ??
                       (_delColumnCommand = new RelayCommand(o =>
                       {
                           Machine.DelColumn(SelectedIndexIns.Column);
                           OnPropertyChanged();
                           OnPropertyChanged("ColumnHeaders");
                       }));
            }
        }


        private RelayCommand _regenerateColumns;

        public RelayCommand RegenerateColumns
        {
            get
            {
                return _regenerateColumns ??
                       (_regenerateColumns = new RelayCommand(o =>
                       {
                           Machine.Regenerate();
                           //обновляем список (очень криво)
                           AddLeftCommand.Execute(null);
                           DelColumnCommand.Execute(null);
                           // обновляем названия строк
                           OnPropertyChanged("RowHeaders");
                       }));
            }
        }

        #endregion

        #endregion

        #region Methods

        public MainWindowVm()
        {
            #region MachineFilling

            Machine = new TuringMachine();
            Machine.StateChanged += UpdateButtons;
            Machine.InstructionIsNull += OnNullInstruction;
            Machine.CurrentState = States.Stopped;

            //Machine.CurrentIndex = 9;
            Machine.Alphabet = "01 ";
            Machine.Instructions = new Dictionary<char, ObservableCollection<InstructionsItem>>();

            Machine.Instructions['0'] = new ObservableCollection<InstructionsItem> {null, null, null, null};
            Machine.Instructions['1'] = new ObservableCollection<InstructionsItem> {null, null, null, null};
            Machine.Instructions[' '] = new ObservableCollection<InstructionsItem> {null, null, null, null};

            Machine.Instructions['0'][1] = new InstructionsItem("1>2");
            Machine.Instructions['1'][1] = new InstructionsItem("0>2");
            Machine.Instructions[' '][0] = new InstructionsItem(" >2");
            Machine.Instructions[' '][1] = new InstructionsItem(" >0");


            //Machine.Instructions1['0'][1] = "1>2";
            //Machine.Instructions1['1'][1] = "0>2";
            //Machine.Instructions1[' '][0] = " >2";
            //Machine.Instructions1[' '][1] = " >0";

            /*for (int i = 0; i < 100; i++)
            {
                Machine.TapeItems.Add(new TapeItem(i));
            }*/

            Machine.TapeItems[10].Letter = '1';
            Machine.TapeItems[11].Letter = '0';
            Machine.TapeItems[12].Letter = '1';
            Machine.TapeItems[13].Letter = '1';
            Machine.TapeItems[14].Letter = '0';
            Machine.TapeItems[15].Letter = '1';
            Machine.TapeItems[16].Letter = '1';
            Machine.TapeItems[17].Letter = '0';
            Machine.TapeItems[18].Letter = '1';
            //Machine.CurrentIndex = 9;
            //Machine.TapeItems[10].Color = "#ffff66";

            #endregion
        }

        public void UpdateButtons(object sender, StateChangedEventArgs e)
        {
            if (e.State == States.Paused)
            {
                IsStartButtonEnabled = true;
                IsStopButtonEnabled = true;
                IsPauseButtonEnabled = false;
            }
            else if (e.State == States.Working)
            {
                IsStartButtonEnabled = false;
                IsStopButtonEnabled = true;
                IsPauseButtonEnabled = true;
            }
            else if (e.State == States.Stopped)
            {
                IsStartButtonEnabled = true;
                IsStopButtonEnabled = false;
                IsPauseButtonEnabled = false;
            }
        }

        public void OnNullInstruction(object sender, InstructionIsNullEventArgs e)
        {
            MessageBox.Show($"Нет команды в ячейке ({e.Sym},Q{e.Q}.", "Ошибка", MessageBoxButton.OK);
        }

        #endregion


        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Gu.Wpf.DataGrid2D;

namespace Turing
{
    internal class MainWindowVM : INotifyPropertyChanged
    {
        #region Fields

        #region FieldsForMouse

        private double top;
        private double left;

        public double Top
        {
            get => top;
            set
            {
                top = value;
                OnPropertyChanged();
            }
        }

        public double Left
        {
            get => left;
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


        private bool isPauseButtonEnabled;
        private bool isStopButtonEnabled;
        private bool isStartButtonEnabled;


        public bool IsPauseButtonEnabled
        {
            get => isPauseButtonEnabled;
            set
            {
                isPauseButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsStopButtonEnabled
        {
            get => isStopButtonEnabled;
            set
            {
                isStopButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsStartButtonEnabled
        {
            get => isStartButtonEnabled;
            set
            {
                isStartButtonEnabled = value;
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
                int lenofinstrutions = Machine.Instructions.ElementAt(0).Value.Count; // ультра говнокод (получаем количество элементов в массиве)
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


        private object selectedItem;

        public object SelectedItem
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


        private int selectedIndex;

        public int SelectedIndex
        {
            get => selectedIndex;

            set
            {
                if (Equals(value, selectedIndex))
                {
                    return;
                }

                selectedIndex = value;
                OnPropertyChanged();
            }
        }


        private RowColumnIndex selectedIndexIns;

        public RowColumnIndex SelectedIndexIns
        {
            get => selectedIndexIns;

            set
            {
                if (Equals(value, selectedIndexIns))
                {
                    return;
                }

                selectedIndexIns = value;
                OnPropertyChanged();
            }
        }


        private TapeItem selectedTapeItem;

        public TapeItem SelectedTapeItem
        {
            get => selectedTapeItem;

            set
            {
                if (Equals(value, selectedTapeItem))
                {
                    return;
                }

                selectedTapeItem = value;
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

        private RelayCommand calcCommand;

        public RelayCommand CalcCommand
        {
            get
            {
                return calcCommand ??
                       (calcCommand = new RelayCommand(o =>
                       {
                           //Machine.CurrentIndex = 99;
                           Machine.Calc();
                           //Machine.CurrentIndex = 99;
                       }));
            }
        }

        private RelayCommand stepCommand;

        public RelayCommand StepCommand
        {
            get
            {
                return stepCommand ??
                       (stepCommand = new RelayCommand(o => { Machine.makeStep(); }));
            }
        }

        #endregion

        #region TapeCommands

        private RelayCommand moveLeftCommand;

        public RelayCommand MoveLeftCommand
        {
            get
            {
                return moveLeftCommand ??
                       (moveLeftCommand = new RelayCommand(o => { Machine.CurrentIndex -= 1; }));
            }
        }

        private RelayCommand moveRightCommand;

        public RelayCommand MoveRightCommand
        {
            get
            {
                return moveRightCommand ??
                       (moveRightCommand = new RelayCommand(o => { Machine.CurrentIndex += 1; }));
            }
        }


        private RelayCommand cellSelectedCommand;

        public RelayCommand CellSelectedCommand
        {
            get
            {
                return cellSelectedCommand ??
                       (cellSelectedCommand = new RelayCommand(o =>
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

        #endregion

        #region InstructionsCommands

        private RelayCommand pauseCommand;

        public RelayCommand PauseCommand
        {
            get
            {
                return pauseCommand ??
                       (pauseCommand = new RelayCommand(o =>
                       {
                           Machine.CurrentState = States.paused;
                           OnPropertyChanged();
                       }));
            }
        }

        private RelayCommand stopCommand;

        public RelayCommand StopCommand
        {
            get
            {
                return stopCommand ??
                       (stopCommand = new RelayCommand(o =>
                       {
                           Machine.CurrentState = States.stopped;
                           OnPropertyChanged();
                       }));
            }
        }

        private RelayCommand addLeftCommand;

        public RelayCommand AddLeftCommand
        {
            get
            {
                return addLeftCommand ??
                       (addLeftCommand = new RelayCommand(o =>
                       {
                           Machine.addColumnLeft(SelectedIndexIns.Column);
                           OnPropertyChanged();
                           OnPropertyChanged("ColumnHeaders");
                       }));
            }
        }


        private RelayCommand addRightCommand;

        public RelayCommand AddRightCommand
        {
            get
            {
                return addRightCommand ??
                       (addRightCommand = new RelayCommand(o =>
                       {
                           Machine.addColumnRight(SelectedIndexIns.Column);
                           OnPropertyChanged();
                           OnPropertyChanged("ColumnHeaders");
                       }));
            }
        }

        private RelayCommand delColumnCommand;

        public RelayCommand DelColumnCommand
        {
            get
            {
                return delColumnCommand ??
                       (delColumnCommand = new RelayCommand(o =>
                       {
                           Machine.delColumn(SelectedIndexIns.Column);
                           OnPropertyChanged();
                           OnPropertyChanged("ColumnHeaders");
                       }));
            }
        }


        private RelayCommand regenerateColumns;

        public RelayCommand RegenerateColumns
        {
            get
            {
                return regenerateColumns ??
                       (regenerateColumns = new RelayCommand(o =>
                       {
                           Machine.regenerate();
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

        public MainWindowVM()
        {
            #region MachineFilling

            Machine = new TuringMachine();
            Machine.StateChanged += UpdateButtons;
            Machine.InstructionIsNull += onNullInstruction;
            Machine.CurrentState = States.stopped;

            //Machine.CurrentIndex = 9;
            Machine.Alpabet = "01 ";
            Machine.Instructions = new Dictionary<char, ObservableCollection<string>>();

            Machine.Instructions['0'] = new ObservableCollection<string> {null, null, null, null};
            Machine.Instructions['1'] = new ObservableCollection<string> {null, null, null, null};
            Machine.Instructions[' '] = new ObservableCollection<string> {null, null, null, null};

            Machine.Instructions['0'][1] = "1>2";
            Machine.Instructions['1'][1] = "0>2";
            Machine.Instructions[' '][0] = " >2";
            Machine.Instructions[' '][1] = " >0";

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
            if (e.state == States.paused)
            {
                IsStartButtonEnabled = true;
                IsStopButtonEnabled = true;
                IsPauseButtonEnabled = false;
            }
            else if (e.state == States.working)
            {
                IsStartButtonEnabled = false;
                IsStopButtonEnabled = true;
                IsPauseButtonEnabled = true;
            }
            else if (e.state == States.stopped)
            {
                IsStartButtonEnabled = true;
                IsStopButtonEnabled = false;
                IsPauseButtonEnabled = false;
            }
        }

        public void onNullInstruction(object sender, InstructionIsNullEventArgs e)
        {
            MessageBox.Show($"Нет команды в ячейке ({e.sym},Q{e.q}.", "Ошибка", MessageBoxButton.OK);
        }

        public void DisplayMessage(object sender, TuringMachineEventArgs e)
        {
            MessageBox.Show(e.Message);
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
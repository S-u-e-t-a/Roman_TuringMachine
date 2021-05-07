using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using Gu.Wpf.DataGrid2D;
using Microsoft.Win32;

namespace Turing
{
    internal class MainWindowVm : INotifyPropertyChanged
    {
        private RelayCommand exitCommand;

        public RelayCommand ExitCommand
        {
            get
            {
                return exitCommand ??
                       (exitCommand = new RelayCommand(selectedItem => { Application.Current.Shutdown(); }));
            }
        }

        #region Fields

        #region FieldsForMouse

        private double _top;
        private double _left;

        public string PathToCurrentFile { get; set; }

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
        private bool _isControlsEnabled;

        public bool IsControlsEnabled
        {
            get => _isControlsEnabled;
            set
            {
                _isControlsEnabled = value;
                OnPropertyChanged();
            }
        }

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

        private List<TapeItem> _savedTape;

        private List<TapeItem> SavedTape
        {
            get => _savedTape;
            set
            {
                if (value == null)
                {
                    IsTapeSaved = false;
                }
                else
                {
                    IsTapeSaved = true;
                }

                _savedTape = value;
                OnPropertyChanged("IsTapeSaved");
            }
        }

        private bool _isTapeSaved;

        public bool IsTapeSaved
        {
            get => _isTapeSaved && IsControlsEnabled;
            set => _isTapeSaved = value;
        }

        public List<string> ColumnHeaders
        {
            get
            {
                int lenofinstrutions =
                    Machine.Instructions.ElementAt(0).Value
                        .Count; // получаем количество элементов в массиве
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

        #endregion

        #region Commands

        #region CalculationsCommands

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

        private RelayCommand _calcCommand;

        public RelayCommand CalcCommand
        {
            get
            {
                return _calcCommand ??
                       (_calcCommand = new RelayCommand(o => { Machine.Calc(); }));
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

        #region SaveLoadCommands

        #region SaveLoadTape

        private RelayCommand _saveTapeCommand;

        public RelayCommand SaveTapeCommand
        {
            get
            {
                return _saveTapeCommand ??
                       (_saveTapeCommand = new RelayCommand(o => { SavedTape = Machine.TapeItems.ToList(); }));
            }
        }

        private RelayCommand _loadTapeCommand;

        public RelayCommand LoadTapeCommand
        {
            get
            {
                return _loadTapeCommand ??
                       (_loadTapeCommand = new RelayCommand(o =>
                       {
                           Machine.TapeItems = new ObservableCollection<TapeItem>(SavedTape);
                           OnPropertyChanged("Machine.TapeItems");
                       }));
            }
        }

        #endregion

        #region SaveLoadMachine

        private RelayCommand _newMachineCommand;

        public RelayCommand NewMachineCommand
        {
            get
            {
                return _newMachineCommand ??
                       (_newMachineCommand = new RelayCommand(o =>
                       {
                           Machine = new TuringMachine();
                           OnPropertyChanged("RowHeaders");
                           OnPropertyChanged("ColumnHeaders");
                       }));
            }
        }


        private RelayCommand _saveMachineCommand;

        public RelayCommand SaveMachineCommand
        {
            get
            {
                return _saveMachineCommand ??
                       (_saveMachineCommand = new RelayCommand(o =>
                       {
                           if (PathToCurrentFile == null)
                           {
                               SaveAsMachineCommand.Execute(null);
                           }
                           else
                           {
                               BinaryFormatter formatter = new BinaryFormatter();
                               using (FileStream fs = new FileStream(PathToCurrentFile, FileMode.OpenOrCreate))
                               {
                                   formatter.Serialize(fs, Machine);
                               }
                           }
                       }));
            }
        }


        private RelayCommand _saveAsMachineCommand;

        public RelayCommand SaveAsMachineCommand
        {
            get
            {
                return _saveAsMachineCommand ??
                       (_saveAsMachineCommand = new RelayCommand(o =>
                       {
                           var dlg = new SaveFileDialog();
                           dlg.FileName = "Machine";
                           dlg.DefaultExt = ".machine";
                           dlg.Filter = "Файлы машины (.machine)|*.machine";
                           var result = dlg.ShowDialog();
                           if (result == true)
                           {
                               BinaryFormatter formatter = new BinaryFormatter();
                               using (FileStream fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate))
                               {
                                   formatter.Serialize(fs, Machine);
                               }

                               PathToCurrentFile = dlg.FileName;
                           }
                       }));
            }
        }

        private RelayCommand _loadMachineCommand;

        public RelayCommand LoadMachineCommand
        {
            get
            {
                return _loadMachineCommand ??
                       (_loadMachineCommand = new RelayCommand(o =>
                       {
                           var dlg = new OpenFileDialog();
                           dlg.DefaultExt = ".machine";
                           dlg.Filter = "Файлы машины (.machine)|*.machine";
                           var result = dlg.ShowDialog();
                           if (result == true)
                           {
                               BinaryFormatter formatter = new BinaryFormatter();
                               using (FileStream fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate))
                               {
                                   Machine = (TuringMachine) formatter.Deserialize(fs);
                               }
                           }

                           PathToCurrentFile = dlg.FileName;
                       }));
            }
        }

        #endregion

        #region SaveLoadIns

        private RelayCommand _saveAsInsCommand;

        public RelayCommand SaveAsInsCommand
        {
            get
            {
                return _saveAsInsCommand ??
                       (_saveAsInsCommand = new RelayCommand(o =>
                       {
                           var dlg = new SaveFileDialog();
                           dlg.FileName = "TableOfStates";
                           dlg.DefaultExt = ".tos";
                           dlg.Filter = "Файлы таблицы состояний (.tos)|*.tos";
                           var result = dlg.ShowDialog();
                           if (result == true)
                           {
                               BinaryFormatter formatter = new BinaryFormatter();
                               using (FileStream fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate))
                               {
                                   formatter.Serialize(fs, Machine.Instructions);
                               }
                           }
                       }));
            }
        }

        private RelayCommand _loadInsCommand;

        public RelayCommand LoadInsCommand
        {
            get
            {
                return _loadInsCommand ??
                       (_loadInsCommand = new RelayCommand(o =>
                       {
                           var dlg = new OpenFileDialog();
                           dlg.DefaultExt = ".tos";
                           dlg.Filter = "Файлы таблицы состояний (.tos)|*.tos";
                           var result = dlg.ShowDialog();
                           if (result == true)
                           {
                               BinaryFormatter formatter = new BinaryFormatter();
                               using (FileStream fs = new FileStream(dlg.FileName, FileMode.OpenOrCreate))
                               {
                                   Machine.Instructions =
                                       (SortedDictionary<char, ObservableCollection<InstructionsItem>>) formatter
                                           .Deserialize(fs);
                               }
                           }
                       }));
            }
        }

        #endregion

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

        #region ChangeInstrutrionsGrid

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

        #endregion

        #region Methods

        public MainWindowVm()
        {
            #region MachineFilling

            Machine = new TuringMachine();

            Machine.StateChanged += UpdateButtons;
            Machine.InstructionIsBad += OnBadInstruction;
            Machine.ProgramDone += ProgramDone;

            Machine.CurrentState = States.Stopped;
            Machine.CurrentIndex = 9;
            Machine.Alphabet = "01 ";
            Machine.Instructions = new SortedDictionary<char, ObservableCollection<InstructionsItem>>();

            Machine.Instructions['0'] = new ObservableCollection<InstructionsItem> {new InstructionsItem(), new InstructionsItem(), new InstructionsItem(), new InstructionsItem()};
            Machine.Instructions['1'] = new ObservableCollection<InstructionsItem> { new InstructionsItem(), new InstructionsItem(), new InstructionsItem(), new InstructionsItem() };
            Machine.Instructions[' '] = new ObservableCollection<InstructionsItem> { new InstructionsItem(), new InstructionsItem(), new InstructionsItem(), new InstructionsItem() };

            Machine.Instructions['0'][1] = new InstructionsItem("1>2");
            Machine.Instructions['1'][1] = new InstructionsItem("0>2");
            Machine.Instructions[' '][0] = new InstructionsItem(" >2");
            Machine.Instructions[' '][1] = new InstructionsItem(" >0");

            Machine.TapeItems[10].Letter = '1';
            Machine.TapeItems[11].Letter = '0';
            Machine.TapeItems[12].Letter = '1';
            Machine.TapeItems[13].Letter = '1';
            Machine.TapeItems[14].Letter = '0';
            Machine.TapeItems[15].Letter = '1';
            Machine.TapeItems[16].Letter = '1';
            Machine.TapeItems[17].Letter = '0';
            Machine.TapeItems[18].Letter = '1';

            #endregion
        }

        public void UpdateButtons(object sender, StateChangedEventArgs e)
        {
            if (e.State == States.Paused)
            {
                IsStartButtonEnabled = true;
                IsStopButtonEnabled = true;
                IsPauseButtonEnabled = false;
                IsControlsEnabled = false;
            }
            else if (e.State == States.Working)
            {
                IsStartButtonEnabled = false;
                IsStopButtonEnabled = true;
                IsPauseButtonEnabled = true;
                IsControlsEnabled = false;
            }
            else if (e.State == States.Stopped)
            {
                IsStartButtonEnabled = true;
                IsStopButtonEnabled = false;
                IsPauseButtonEnabled = false;
                IsControlsEnabled = true;
            }
        }

        public void ProgramDone(object sender, ProgramDoneEventArgs e)
        {
            MessageBox.Show($"{e.Message}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void OnBadInstruction(object sender, InstructionIsBadEventArgs e)
        {
            MessageBox.Show($"{e.Message} ({e.Sym},Q{e.Q}).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
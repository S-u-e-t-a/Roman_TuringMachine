﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Gu.Wpf.DataGrid2D;

namespace Turing
{
    class MainWindowVM : INotifyPropertyChanged
    {
        #region Fields
        #region FieldsForMouse
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

        #endregion

        public bool isPauseButtonEnabled { get; set; }
        public bool isStopButtonEnabled { get; set; }
        public bool isStartButtonEnabled { get; set; }
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
                int lenofinstrutions = Machine.Instructions[Machine.Instructions.ElementAt(0).Key].Count; // ультра говнокод (получаем количество элементов в массиве)
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

       
        private RowColumnIndex selectedIndexIns;
        public RowColumnIndex SelectedIndexIns
        {
            get => this.selectedIndexIns;

            set
            {
                if (Equals(value, this.selectedIndexIns))
                {
                    return;
                }

                this.selectedIndexIns = value;
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

        RelayCommand calcCommand;
        public RelayCommand CalcCommand
        {
            get
            {
                return calcCommand ??
                       (calcCommand = new RelayCommand((o) =>
                       {
                           //Machine.CurrentIndex = 99;
                           Machine.Calc();
                           //Machine.CurrentIndex = 99;
                       }));
            }
        }

        RelayCommand stepCommand;
        public RelayCommand StepCommand
        {
            get
            {
                return stepCommand ??
                       (stepCommand = new RelayCommand((o) =>
                       {
                           //Machine.CurrentIndex = 99;
                           Machine.makeStep();
                           //Machine.CurrentIndex = 99;
                       }));
            }
        }

        #endregion

        #region TapeCommands

        RelayCommand moveLeftCommand;
        public RelayCommand MoveLeftCommand
        {
            get
            {
                return moveLeftCommand ??
                       (moveLeftCommand = new RelayCommand((o) =>
                       {
                           Machine.CurrentIndex -= 1;
                       }));
            }
        }

        RelayCommand moveRightCommand;
        public RelayCommand MoveRightCommand
        {
            get
            {
                return moveRightCommand ??
                       (moveRightCommand = new RelayCommand((o) =>
                       {
                           Machine.CurrentIndex += 1;
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

        #endregion

        #region InstructionsCommands

        RelayCommand pauseCommand;
        public RelayCommand PauseCommand
        {
            get
            {
                return pauseCommand ??
                       (pauseCommand = new RelayCommand((o) =>
                       {
                           Machine.CurrentState = States.paused;
                           OnPropertyChanged();
                       }));
            }
        }

        RelayCommand stopCommand;
        public RelayCommand StopCommand
        {
            get
            {
                return stopCommand ??
                       (stopCommand = new RelayCommand((o) =>
                       {
                           Machine.CurrentState = States.stopped;
                           OnPropertyChanged();
                       }));
            }
        }

        RelayCommand addLeftCommand;
        public RelayCommand AddLeftCommand
        {
            get
            {
                return addLeftCommand ??
                       (addLeftCommand = new RelayCommand((o) =>
                       {
                           Machine.addColumnLeft(SelectedIndexIns.Column);
                           OnPropertyChanged();
                           OnPropertyChanged("ColumnHeaders");
                       }));
            }
        }


        RelayCommand addRightCommand;
        public RelayCommand AddRightCommand
        {
            get
            {
                return addRightCommand ??
                       (addRightCommand = new RelayCommand((o) =>
                       {
                           Machine.addColumnRight(SelectedIndexIns.Column);
                           OnPropertyChanged();
                           OnPropertyChanged("ColumnHeaders");
                       }));
            }
        }

        RelayCommand delColumnCommand;
        public RelayCommand DelColumnCommand
        {
            get
            {
                return delColumnCommand ??
                       (delColumnCommand = new RelayCommand((o) =>
                       {
                           Machine.delColumn(SelectedIndexIns.Column);
                           OnPropertyChanged();
                           OnPropertyChanged("ColumnHeaders");
                       }));
            }
        }


        RelayCommand regenerateColumns;
        public RelayCommand RegenerateColumns
        {
            get
            {
                return regenerateColumns ??
                       (regenerateColumns = new RelayCommand((o) =>
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
            //Machine.StateChanged += UpdateButtons;
            Machine.CurrentState = States.stopped;
            //Machine.CurrentIndex = 9;
            Machine.Alpabet = "01 ";
            Machine.Instructions = new Dictionary<char, ObservableCollection<string>>();

            Machine.Instructions['0'] = new ObservableCollection<string>() { null, null, null, null };
            Machine.Instructions['1'] = new ObservableCollection<string>() { null, null, null, null };
            Machine.Instructions[' '] = new ObservableCollection<string>() { null, null, null, null };

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
            //Machine.CurrentIndex = 9;
            //Machine.TapeItems[10].Color = "#ffff66";

            #endregion
        }

        public void UpdateButtons(object sender, StateChangedEventArgs e)
        {
            if (e.state == States.paused)
            {
                isStartButtonEnabled = true;
                isStopButtonEnabled = true;
                isPauseButtonEnabled = false;

            }
            else if (e.state == States.working)
            {
                isStartButtonEnabled = false;
                isStopButtonEnabled = true;
                isPauseButtonEnabled = true;
            }
            else if (e.state == States.stopped)
            {
                isStartButtonEnabled = true;
                isStopButtonEnabled = false;
                isPauseButtonEnabled = false;
            }
            
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

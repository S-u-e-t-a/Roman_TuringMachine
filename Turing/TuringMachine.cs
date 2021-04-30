﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Turing
{
    #region events
    
    internal class StateChangedEventArgs
    {
        public StateChangedEventArgs(States state)
        {
            this.State = state;
        }
        public States State { get; }
    }

    internal class InstructionIsNullEventArgs
    {
        public int Q;
        public char Sym;
        public InstructionIsNullEventArgs(char sym, int q)
        {
            this.Sym = sym;
            this.Q = q;
        }
    }

    #endregion


    internal enum States
    {
        Working,
        Paused,
        Stopped
    }

    internal class TuringMachine : INotifyPropertyChanged
    {
        public delegate void InstructionIsNullHandler(object sender, InstructionIsNullEventArgs e);

        public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);

        public TuringMachine()
        {
            CurrentState = States.Stopped;
            _q = 1;
            Delay = 100;
            Instructions = new Dictionary<char, ObservableCollection<InstructionsItem>>();
            TapeItems = new ObservableCollection<TapeItem>();
            for (int i = 0; i < _initialLenOfTape; i++)
            {
                TapeItems.Add(new TapeItem(i));
            }

            CurrentIndex = 0;
        }

        public event StateChangedHandler StateChanged;
        public event InstructionIsNullHandler InstructionIsNull;

        #region Fields

        private States _currentState;

        public States CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                if (_currentState == States.Stopped)
                {
                    _q = 1;
                }

                StateChanged?.Invoke(this, new StateChangedEventArgs(_currentState));
                OnPropertyChanged();
            }
        }

        private readonly int _initialLenOfTape = 200;
        private int _q;
        private string _alphabet;

        public string Alphabet
        {
            get => _alphabet;
            set
            {
                SortedSet<char> symbols = new SortedSet<char>(value.ToCharArray());
                if (!symbols.Contains(' '))
                {
                    symbols.Add(' ');
                }

                _alphabet = new string(symbols.ToArray());
                OnPropertyChanged();
            }
        }

        private string _comment;

        public string Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged();
            }
        }

        private int _currentIndex;

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (value == -1)
                {
                    TapeItems.Insert(0, new TapeItem(TapeItems[0].Index - 1));
                    TapeItems[1].IsSelected = false;
                    TapeItems[0].IsSelected = true;
                    _currentIndex = 0;
                }
                else if (value == TapeItems.Count)
                {
                    TapeItems.Add(new TapeItem(TapeItems.Count));
                    TapeItems[TapeItems.Count - 2].IsSelected = false;
                    TapeItems[TapeItems.Count - 1].IsSelected = true;
                    _currentIndex = value;
                }
                else
                {
                    TapeItems[_currentIndex].IsSelected = false;
                    TapeItems[value].IsSelected = true;
                    _currentIndex = value;
                }

                OnPropertyChanged();
            }
        }

        private int _delay;

        public int Delay
        {
            get => _delay;
            set
            {
                _delay = value;
                OnPropertyChanged();
            }
        }

        public int CountOfQ => Instructions.ElementAt(0).Value.Count;

        private KeyValuePair<char, int> _previousInstruction;
        private bool _isFirstStep = true;


        private Dictionary<char, ObservableCollection<InstructionsItem>> _instructions;

        public Dictionary<char, ObservableCollection<InstructionsItem>> Instructions
        {
            get => _instructions;
            set
            {
                _instructions = value;
                OnPropertyChanged();
            }
        }
        
        private ObservableCollection<TapeItem> _tapeItems;

        public ObservableCollection<TapeItem> TapeItems
        {
            get => _tapeItems;
            set
            {
                _tapeItems = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        #region Calculations

        private void MakeInstruction(int index, Comm ins)
        {
            TapeItems[index].Letter = ins.Sym;
            _q = ins.Condition;
            if (_q != 0)
            {
                if (ins.Direction == Direction.Right)
                {
                    CurrentIndex += 1;
                }
                else
                {
                    CurrentIndex -= 1;
                }
            }
        }


        private Comm _nextComm;

        public void MakeStep()
        {
            //TapeItems[CurrentIndex].IsSelected = true;
            if (_previousInstruction.Key != '\0')
            {
                Instructions[_previousInstruction.Key][_previousInstruction.Value].IsSelected = false;
            }

            //previousInstruction = new KeyValuePair<char, int>(currentTapeItem.Letter, q - 1);
            if (!_isFirstStep)
            {
                MakeInstruction(CurrentIndex, _nextComm);
            }

            _isFirstStep = false;
            /*else
            {
                nextComm = Instructions[currentTapeItem.Letter][q - 1];
                previousInstruction = new KeyValuePair<char, int>(currentTapeItem.Letter, q - 1);
                //CurrentState = States.paused;
            }*/
            var currentTapeItem = TapeItems[CurrentIndex];
            if (_q != 0)
            {
                Instructions[currentTapeItem.Letter][_q - 1].IsSelected = true;
                _nextComm = new Comm(Instructions[currentTapeItem.Letter][_q - 1].Str);

                string strQ = string.Empty;
                for (int i = 0; i < CountOfQ - 1; i++)
                {
                    strQ += i.ToString();
                }

                Regex commRegex = new Regex($"^[{Alphabet}][<>][{strQ}]$");
                if (_nextComm == null || !commRegex.IsMatch(_nextComm.ToString()))
                {
                    InstructionIsNull?.Invoke(this, new InstructionIsNullEventArgs(currentTapeItem.Letter, _q));
                    CurrentState = States.Stopped;
                    Instructions[currentTapeItem.Letter][_q - 1].IsSelected = false;
                }

                _previousInstruction = new KeyValuePair<char, int>(currentTapeItem.Letter, _q - 1);
            }

            if (_q == 0)
            {
                _q = 1;
                CurrentState = States.Stopped;
                Instructions[_previousInstruction.Key][_previousInstruction.Value].IsSelected = false;
                _previousInstruction = new KeyValuePair<char, int>();
                _isFirstStep = true;
            }
        }

        public async Task Calc()
        {
            CurrentState = States.Working;
            while (CurrentState == States.Working)
            {
                if (CurrentState == States.Working)
                {
                    MakeStep();
                    await Task.Delay(Delay);
                }
            }
        }

        #endregion

        #region ChangeInstructions

        public void Regenerate()
        {
            //Trace.WriteLine(Alphabet);
            var newKeys = Alphabet.ToCharArray();
            int lenOfLists = Instructions.ElementAt(0).Value.Count;
            var oldKeys = Instructions.Keys.ToArray();
            /*Trace.WriteLine("Old---------------------");
            foreach (var VARIABLE in Instructions)
            {
                Trace.Write(VARIABLE.Key + " ");
                foreach (var ls in VARIABLE.Value)
                {
                    Trace.Write(ls + " ");
                }
                Trace.WriteLine(Environment.NewLine);
            }*/

            foreach (var key in newKeys)
            {
                if (!Instructions.ContainsKey(key))
                {
                    Instructions[key] = new ObservableCollection<InstructionsItem>();
                    for (int i = 0; i < lenOfLists; i++)
                    {
                        Instructions[key].Add(null);
                    }
                }
            }

            foreach (var t in oldKeys)
            {
                if (!newKeys.Contains(t))
                {
                    Instructions.Remove(t);
                }
            }
        }

        public void AddColumnLeft(int currentColumn)
        {
            foreach (var keypair in Instructions)
            {
                keypair.Value.Insert(currentColumn, null);
            }
        }

        public void AddColumnRight(int currentColumn)
        {
            foreach (var keypair in Instructions)
            {
                keypair.Value.Insert(currentColumn + 1, null);
            }
        }


        public void DelColumn(int currentColumn)
        {
            foreach (var keypair in Instructions)
            {
                keypair.Value.RemoveAt(currentColumn);
            }
        }

        #endregion

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

    #region Misc

    internal enum Direction
    {
        Left = 0,
        Right
    }

    internal class InstructionsItem : INotifyPropertyChanged
    {
        private string _color;


        private bool _isSelected;
        private string _str;

        public InstructionsItem(string str)
        {
            Str = str;
            IsSelected = false;
        }

        public string Str
        {
            get => _str;
            set
            {
                _str = value;
                OnPropertyChanged();
            }
        }

        public string Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value)
                {
                    Color = "#99ff99";
                }
                else
                {
                    Color = null;
                }

                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }


    internal class Comm : INotifyPropertyChanged
    {
        private string _color;

        private bool _isSelected;

        public Comm(char sym, Direction dir, int condition)
        {
            IsSelected = false;
            Sym = sym;
            Condition = condition;
            Direction = dir;
        }

        public Comm(string command)
        {
            IsSelected = false;
            Sym = command[0];
            Condition = int.Parse(command[2].ToString());
            if (command[1] == '<')
            {
                Direction = Direction.Left;
            }
            else
            {
                Direction = Direction.Right;
            }
        }

        public string StrValue
        {
            get => ToString();
            set
            {
                var newCom = new Comm(value);
                Condition = newCom.Condition;
                Direction = newCom.Direction;
                OnPropertyChanged();
            }
        }

        public string Color
        {
            get => _color;
            private set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value)
                {
                    Color = "#99ff99";
                }
                else
                {
                    Color = null;
                }

                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            string dir;
            if (Direction == Direction.Right)
            {
                dir = ">";
            }
            else
            {
                dir = "<";
            }

            return Sym + dir + Condition;
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #region fields

        private char _sym;
        private Direction _direction; // 0-left 1-right
        private int _condition;


        public char Sym
        {
            get => _sym;
            set
            {
                _sym = value;
                OnPropertyChanged();
            }
        }

        public Direction Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                OnPropertyChanged();
            }
        } // 0-left 1-right

        public int Condition
        {
            get => _condition;
            set
            {
                _condition = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }

    #endregion
}
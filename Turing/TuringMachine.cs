using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Turing
{
    internal enum States
    {
        Working,
        Paused,
        Stopped
    }

    [Serializable]
    internal class TuringMachine : INotifyPropertyChanged
    {
        public delegate void InstructionIsBadHandler(object sender, InstructionIsBadEventArgs e);

        public delegate void ProgramDoneHandler(object sender, ProgramDoneEventArgs e);

        public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);

        public TuringMachine()
        {
            CurrentState = States.Stopped;
            _q = 1; 
            Delay = 1000;
            Instructions = new SortedDictionary<char, ObservableCollection<InstructionsItem>>();
            Instructions[' '] = new ObservableCollection<InstructionsItem> {new InstructionsItem()};
            TapeItems = new ObservableCollection<TapeItem>();
            for (int i = 0; i < _initialLenOfTape; i++)
            {
                TapeItems.Add(new TapeItem(i));
            }

            _isFirstStep = true;
            CurrentIndex = 0;
        }

        [field: NonSerializedAttribute] public event StateChangedHandler StateChanged;

        [field: NonSerializedAttribute] public event InstructionIsBadHandler InstructionIsBad;

        [field: NonSerializedAttribute] public event ProgramDoneHandler ProgramDone;
        #region Fields

        [NonSerialized] private States _currentState;

        public States CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                if (_currentState == States.Stopped)
                {
                    _q = 1;
                    if (_previousInstruction.Key != '\0')
                    {
                        Instructions[_previousInstruction.Key][_previousInstruction.Value].IsSelected = false;
                    }

                    _previousInstruction = new KeyValuePair<char, int>();
                }

                StateChanged?.Invoke(this, new StateChangedEventArgs(_currentState));
                OnPropertyChanged();
            }
        }

        private readonly int _initialLenOfTape = 200;

        [NonSerialized] private int _q;

        private string _alphabet;

        [NonSerialized] private Comm _nextComm;

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


        private string _condition;

        public string Condition
        {
            get => _condition;
            set
            {
                _condition = value;
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

        private bool _isFirstStep;

        private bool IsFirstStep
        {
            get => _isFirstStep;
            set => _isFirstStep = value;
        }


        private SortedDictionary<char, ObservableCollection<InstructionsItem>> _instructions;

        public SortedDictionary<char, ObservableCollection<InstructionsItem>> Instructions
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
            if (ins == null)
            {
                return;
            }
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


        public void MakeStep()
        {
            if (CurrentState != States.Working)
            {
                CurrentState = States.Paused;
            }

            if (_previousInstruction.Key != '\0')
            {
                Instructions[_previousInstruction.Key][_previousInstruction.Value].IsSelected = false;
            }

            if (!_isFirstStep)
            {
                MakeInstruction(CurrentIndex, _nextComm);
            }

            _isFirstStep = false;
            var currentTapeItem = TapeItems[CurrentIndex];
            if (_q != 0)
            {
                Instructions[currentTapeItem.Letter][_q - 1].IsSelected = true;
                _nextComm = new Comm(Instructions[currentTapeItem.Letter][_q - 1].Str);

                string strQ = string.Empty;
                for (int i = 0; i < CountOfQ + 1; i++)
                {
                    strQ += i.ToString();
                }

                Regex commRegex = new Regex($"^[{Alphabet}][<>][{strQ}]$");
                if (_nextComm == null || !commRegex.IsMatch(_nextComm.ToString()))
                {
                    if (_nextComm == null || _nextComm.ToString() == "")
                    {
                        InstructionIsBad?.Invoke(this,
                            new InstructionIsBadEventArgs("Нет инструкции в ячейке", currentTapeItem.Letter, _q));
                    }
                    else if (!commRegex.IsMatch(_nextComm.ToString()))
                    {
                        InstructionIsBad?.Invoke(this,
                            new InstructionIsBadEventArgs("Некорректная инструкция в ячейке", currentTapeItem.Letter,
                                _q));
                    }

                    CurrentState = States.Stopped;
                    Instructions[currentTapeItem.Letter][_q - 1].IsSelected = false;
                }

                _previousInstruction = new KeyValuePair<char, int>(currentTapeItem.Letter, _q - 1);
            }

            if (_q == 0)
            {
                ProgramDone?.Invoke(this, new ProgramDoneEventArgs("Работа программы завершена."));
                _q = 1;
                CurrentState = States.Stopped;
                _isFirstStep = true;
            }
        }

        public async Task Calc()
        {
            //_isFirstStep = true;
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
            var newKeys = Alphabet.ToCharArray();
            int lenOfLists = Instructions.ElementAt(0).Value.Count;
            var oldKeys = Instructions.Keys.ToArray();
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
                keypair.Value.Insert(currentColumn, new InstructionsItem());
            }
        }

        public void AddColumnRight(int currentColumn)
        {
            foreach (var keypair in Instructions)
            {
                keypair.Value.Insert(currentColumn + 1, new InstructionsItem());
            }
        }


        public void DelColumn(int currentColumn)
        {
            if (CountOfQ >= 2)
            {
                foreach (var keypair in Instructions)
                {
                    keypair.Value.RemoveAt(currentColumn);
                }
            }
        }

        #endregion

        #endregion

        #region PropertyChanged

        [field: NonSerializedAttribute] public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }

    #region Misc

    #region events

    internal class StateChangedEventArgs
    {
        public StateChangedEventArgs(States state)
        {
            State = state;
        }

        public States State { get; }
    }

    internal class ProgramDoneEventArgs
    {
        public ProgramDoneEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    internal class InstructionIsBadEventArgs
    {
        public string Message;
        public int Q;
        public char Sym;

        public InstructionIsBadEventArgs(string message, char sym, int q)
        {
            Message = message;
            Sym = sym;
            Q = q;
        }
    }

    #endregion

    internal enum Direction
    {
        Left = 0,
        Right
    }

    [Serializable]
    internal class InstructionsItem : INotifyPropertyChanged
    {
        private string _color;


        private bool _isSelected;
        private string _str;

        public InstructionsItem(){}
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

        [field: NonSerializedAttribute] public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    internal class Comm : INotifyPropertyChanged
    {
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

        private string _color;
        private bool _isSelected;
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

        #endregion
    }

    #endregion
}
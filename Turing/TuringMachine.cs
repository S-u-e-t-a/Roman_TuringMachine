using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Turing
{
    internal class TuringMachineEventArgs
    {
        public TuringMachineEventArgs(string mes)
        {
            Message = mes;
        }

        // Сообщение
        public string Message { get; }
    }

    internal class StateChangedEventArgs
    {
        public StateChangedEventArgs(States state)
        {
            this.state = state;
        }

        // Сообщение
        public States state { get; }
    }

    internal class InstructionIsNullEventArgs
    {
        public int q;

        // Сообщение
        public char sym;

        public InstructionIsNullEventArgs(char sym, int q)
        {
            this.sym = sym;
            this.q = q;
        }
    }

    internal enum States
    {
        working,
        paused,
        stopped
    }

    internal class TuringMachine : INotifyPropertyChanged
    {
        public delegate void InstructionIsNullHandler(object sender, InstructionIsNullEventArgs e);

        public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);

        public delegate void TuringMachineHandler(object sender, TuringMachineEventArgs e);

        public TuringMachine()
        {
            CurrentState = States.stopped;
            q = 1;
            Delay = 100;
            Instructions = new Dictionary<char, ObservableCollection<string>>();
            TapeItems = new ObservableCollection<TapeItem>();
            for (int i = 0; i < initialLenOfTape; i++)
            {
                TapeItems.Add(new TapeItem(i));
            }

            CurrentIndex = 0;
        }

        public event TuringMachineHandler Notify;
        public event StateChangedHandler StateChanged;
        public event InstructionIsNullHandler InstructionIsNull;

        #region Fields

        private States currentState;

        public States CurrentState
        {
            get => currentState;
            set
            {
                currentState = value;
                if (currentState == States.stopped)
                {
                    q = 1;
                }

                StateChanged?.Invoke(this, new StateChangedEventArgs(currentState));
                OnPropertyChanged();
            }
        }

        private readonly int initialLenOfTape = 200;
        private int q;
        private string alpabet;

        public string Alpabet
        {
            get => alpabet;
            set
            {
                SortedSet<char> symbols = new SortedSet<char>(value.ToCharArray());
                if (!symbols.Contains(' '))
                {
                    symbols.Add(' ');
                }

                alpabet = new string(symbols.ToArray());
                OnPropertyChanged();
            }
        }

        private string comment;

        public string Comment
        {
            get => comment;
            set
            {
                comment = value;
                OnPropertyChanged();
            }
        }

        private int currentIndex;

        public int CurrentIndex
        {
            get => currentIndex;
            set
            {
                if (value == -1)
                {
                    TapeItems.Insert(0, new TapeItem(TapeItems[0].Index - 1));
                    TapeItems[1].IsSelected = false;
                    TapeItems[0].IsSelected = true;
                    currentIndex = 0;
                }
                else if (value == TapeItems.Count)
                {
                    TapeItems.Add(new TapeItem(TapeItems.Count));
                    TapeItems[TapeItems.Count - 2].IsSelected = false;
                    TapeItems[TapeItems.Count - 1].IsSelected = true;
                    currentIndex = value;
                }
                else
                {
                    TapeItems[currentIndex].IsSelected = false;
                    TapeItems[value].IsSelected = true;
                    currentIndex = value;
                }

                OnPropertyChanged();
            }
        }

        private int delay;

        public int Delay
        {
            get => delay;
            set
            {
                delay = value;
                OnPropertyChanged();
            }
        }

        public int CountOfQ => Instructions.ElementAt(0).Value.Count;

        private Dictionary<char, ObservableCollection<string>> instructions;

        public Dictionary<char, ObservableCollection<string>> Instructions
        {
            get => instructions;
            set
            {
                instructions = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<TapeItem> tapeItems;

        public ObservableCollection<TapeItem> TapeItems
        {
            get => tapeItems;
            set
            {
                tapeItems = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        #region Calculations

        public void makeStep()
        {
            var currentTapeItem = TapeItems[CurrentIndex];
            string strInstruction = Instructions[currentTapeItem.Letter][q - 1];
            string strQ = string.Empty;
            for (int i = 0; i < CountOfQ - 1; i++)
            {
                strQ += i.ToString();
            }

            Regex commRegex = new Regex($"^[{Alpabet}][<>][{strQ}]$");
            if (strInstruction == null || !commRegex.IsMatch(strInstruction))
            {
                InstructionIsNull.Invoke(this, new InstructionIsNullEventArgs(currentTapeItem.Letter, q));
                CurrentState = States.stopped;
            }
            else
            {
                var instruction = new Comm(strInstruction);
                TapeItems[CurrentIndex].Letter = instruction.Sym;
                q = instruction.Condition;
                if (q != 0)
                {
                    if (instruction.Direction == direction.left)
                    {
                        CurrentIndex--;
                    }
                    else
                    {
                        CurrentIndex++;
                    }
                }

                if (q == 0)
                {
                    q = 1;
                    CurrentState = States.stopped;
                }
            }
        }


        public async Task Calc()
        {
            CurrentState = States.working;
            while (CurrentState == States.working)
            {
                if (CurrentState == States.working)
                {
                    makeStep();
                    await Task.Delay(Delay);
                }
            }
        }

        #endregion

        #region ChangeInstructions

        public void regenerate()
        {
            //Trace.WriteLine(Alpabet);
            var newKeys = Alpabet.ToCharArray();
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
                    Instructions[key] = new ObservableCollection<string>();
                    for (int i = 0; i < lenOfLists; i++)
                    {
                        Instructions[key].Add(null);
                    }
                }
            }

            for (int i = 0; i < oldKeys.Length; i++)
            {
                if (!newKeys.Contains(oldKeys[i]))
                {
                    Instructions.Remove(oldKeys[i]);
                }
            }

            /*Trace.WriteLine("New---------------------");
            foreach (var VARIABLE in Instructions)
            {
                Trace.Write(VARIABLE.Key + " ");
                foreach (var ls in VARIABLE.Value)
                {
                    Trace.Write(ls + " ");
                }
                Trace.WriteLine(Environment.NewLine);
            }*/
        }

        public void addColumnLeft(int currentColumn)
        {
            foreach (var keypair in Instructions)
            {
                keypair.Value.Insert(currentColumn, null);
            }
        }

        public void addColumnRight(int currentColumn)
        {
            foreach (var keypair in Instructions)
            {
                keypair.Value.Insert(currentColumn + 1, null);
            }
        }


        public void delColumn(int currentColumn)
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

    internal enum direction
    {
        left = 0,
        right
    }

    internal class Comm : INotifyPropertyChanged
    {
        public Comm(char sym, direction dir, int condition)
        {
            Sym = sym;
            Condition = condition;
            Direction = dir;
        }


        public Comm(string command)
        {
            Sym = command[0];
            Condition = int.Parse(command[2].ToString());
            if (command[1] == '<')
            {
                Direction = direction.left;
            }
            else
            {
                Direction = direction.right;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            string dir;
            if (Direction == direction.right)
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

        private char sym;
        private direction direction; // 0-left 1-right
        private int condition;


        public char Sym
        {
            get => sym;
            set
            {
                sym = value;
                OnPropertyChanged();
            }
        }

        public direction Direction
        {
            get => direction;
            set
            {
                direction = value;
                OnPropertyChanged();
            }
        } // 0-left 1-right

        public int Condition
        {
            get => condition;
            set
            {
                condition = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }

    #endregion
}
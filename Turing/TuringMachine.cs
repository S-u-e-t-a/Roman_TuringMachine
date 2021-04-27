using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Turing
{

    class TuringMachineEventArgs
    {
        // Сообщение
        public string Message { get; }

        public TuringMachineEventArgs(string mes)
        {
            Message = mes;
        }
    }

    class StateChangedEventArgs
    {
        // Сообщение
        public States state { get; }

        public StateChangedEventArgs(States state)
        {
            this.state = state;
        }
    }

    enum States
    {
        working,
        paused,
        stopped
    }
    class TuringMachine : INotifyPropertyChanged
    {
        public delegate void TuringMachineHandler(object sender, TuringMachineEventArgs e);
        public event TuringMachineHandler Notify;

        public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);
        public event StateChangedHandler StateChanged;

        #region Fields
        
        private States currentState;
        public States CurrentState
        {
            get => currentState;
            set
            {
                currentState = value;
                /*if (currentState == States.stopped)
                {
                    q = 1;
                }*/
                //StateChanged?.Invoke(this,new StateChangedEventArgs(currentState));
                OnPropertyChanged();
            }
        }
        private int initialLenOfTape = 200;
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
                    TapeItems.Insert(0, new TapeItem(TapeItems[0].Index-1));
                    TapeItems[1].IsSelected = false;
                    TapeItems[0].IsSelected = true;
                    currentIndex = 0;
                }
                else if (value == TapeItems.Count)
                {
                    TapeItems.Add(new TapeItem(TapeItems.Count));
                    TapeItems[TapeItems.Count-2].IsSelected = false;
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

        #region Methods


        #region Calculations

        public void makeStep()
        {
            var currentState = TapeItems[CurrentIndex];
            var instruction = new Comm(Instructions[currentState.Letter][q - 1]);


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

            if (q==0)
            {
                q = 1;
                CurrentState = States.stopped;
            }
        }


        public async Task Calc()
        {
            CurrentState = States.working;
            while (CurrentState== States.working)
            {
                if (CurrentState == States.working)
                {
                    await Task.Delay(Delay);

                    makeStep();
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





    enum direction
    {
        left = 0,
        right
    }
    class Comm : INotifyPropertyChanged
    {
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


        public Comm(char sym, direction dir, int condition)
        {
            Sym = sym;
            Condition = condition;
            Direction = dir;
        }



        public Comm(string command)
        {
            Sym = command[0];
            Condition = Int32.Parse(command[2].ToString());
            if (command[1] == '<')
            {
                Direction = direction.left;
            }
            else
            {
                Direction = direction.right;
            }
        }

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
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    #endregion

}

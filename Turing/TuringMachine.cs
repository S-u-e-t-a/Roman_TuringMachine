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
    class TuringMachine : INotifyPropertyChanged
    {

        #region Fields
        private int q;
        private string alpabet;
        public string Alpabet
        {
            get => alpabet;
            set
            {
                alpabet = value;
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
                try { TapeItems[CurrentIndex].Color = "#ffffff"; }
                catch { }

                currentIndex = value;
                try { TapeItems[currentIndex].Color = "#ffff66"; }
                catch { }

                OnPropertyChanged();
            }
        }

       /* private int previosIndex
        {
            get => previosIndex;
        }*/
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



        private List<TapeItem> tapeItems;
        public List<TapeItem> TapeItems
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
            q = 1;
            Instructions = new Dictionary<char, ObservableCollection<string>>();
            TapeItems = new List<TapeItem>();
        }

        #region Methods


        #region Calculations

        public void makeStep()
        {
            var currentState = TapeItems[CurrentIndex];
            var instruction = new Comm(Instructions[currentState.Letter][q - 1]);


            TapeItems[CurrentIndex].Letter = instruction.Sym;
            if (instruction.Direction == direction.left)
            {
                CurrentIndex--;
            }
            else
            {
                CurrentIndex++;
            }

            q = instruction.Condition;
        }

        public async void Calc()
        {
            Delay = 100;

            while (q != 0)
            {
                await Task.Delay(5000);
                makeStep();
            }

            q = 1;
        }

        #endregion

        #region ChangeInstructions

        public void regenerate()
        {
            Trace.WriteLine(Alpabet);
            var newKeys = Alpabet.ToCharArray();
            int lenOfLists = Instructions.ElementAt(0).Value.Count;
            var oldKeys = Instructions.Keys.ToArray();
            Trace.WriteLine("Old---------------------");
            foreach (var VARIABLE in Instructions)
            {
                Trace.Write(VARIABLE.Key + " ");
                foreach (var ls in VARIABLE.Value)
                {
                    Trace.Write(ls + " ");
                }
                Trace.WriteLine(Environment.NewLine);
            }

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
            Trace.WriteLine("New---------------------");
            foreach (var VARIABLE in Instructions)
            {
                Trace.Write(VARIABLE.Key + " ");
                foreach (var ls in VARIABLE.Value)
                {
                    Trace.Write(ls + " ");
                }
                Trace.WriteLine(Environment.NewLine);
            }

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

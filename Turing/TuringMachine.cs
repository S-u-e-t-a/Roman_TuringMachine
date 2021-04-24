using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Turing
{
    class TuringMachine : INotifyPropertyChanged
    {

        public TuringMachine()
        {
            q = 1;
            Instructions = new Dictionary<char, ObservableCollection<string>>();
            TapeItems = new List<TapeItem>();
        }
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
                currentIndex = value;
                try { TapeItems[currentIndex].Color = "#ffff66"; }
                catch { }
                OnPropertyChanged();
            }
        }

        private int previosIndex
        {
            get => previosIndex;
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
        /*private List<Comm> CloneListComm(List<Comm> list)
        {
            var clone = new List<Comm>();
            foreach (var comm in list)
            {
                clone.Add(new Comm(comm.Sym,comm.Direction,comm.Condition));
            }
            return clone;
        }
        private Dictionary<char, List<Comm>> CloneInstrutions()
        {
            var clone = new Dictionary<char, List<Comm>>();
            foreach (var cond in instructions)
            {
                clone[cond.Key] = CloneListComm(cond.Value);
            }

            return clone;
        }*/
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


        public void makeStep()
        {
            TapeItems[currentIndex].Color = "#ffffff";
            var currentState = TapeItems[CurrentIndex];
            var instruction = new Comm (Instructions[currentState.Letter][q - 1]);


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

            while(q!=0)
            {
                makeStep();
            }

            q = 1;
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
                keypair.Value.Insert(currentColumn+1,null);

            }
        }


        public void delColumn(int currentColumn)
        {
            foreach (var keypair in Instructions)
            {
                keypair.Value.RemoveAt(currentColumn);

            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }


    enum direction
    {
        left =0,
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
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Turing
{

    public class TapeItem : INotifyPropertyChanged
    {

        public TapeItem(int index, char sym=' ')
        {
            this.index = index;
            this.Letter = sym;
        }

        private bool isSelected;

        public bool IsSelected
        {
            set
            {
                if (value)
                    Color = "#ffff66"; // желтый
                else
                    Color = "#ffffff"; // белый
                isSelected = value;
                OnPropertyChanged();
            }
            get => isSelected;
        }

        private string color;
        public string Color
        {
            private set
            {
                color = value;
                OnPropertyChanged();
            }
            
            get => color;

        }

        private char letter;
        public char Letter
        {
            get => letter;
            set
            {
                letter = value;
                OnPropertyChanged();
            }
        }
        private int index;
        public int Index
        {
            get => index;
            set
            {
                index = value;
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
}

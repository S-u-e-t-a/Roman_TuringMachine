using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Turing
{
    public class TapeItem : INotifyPropertyChanged
    {
        private string color;
        private int index;

        private bool isSelected;

        private char letter;

        public TapeItem(int index, char sym = ' ')
        {
            this.index = index;
            Letter = sym;
        }

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

        public string Color
        {
            private set
            {
                color = value;
                OnPropertyChanged();
            }

            get => color;
        }

        public char Letter
        {
            get => letter;
            set
            {
                letter = value;
                OnPropertyChanged();
            }
        }

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
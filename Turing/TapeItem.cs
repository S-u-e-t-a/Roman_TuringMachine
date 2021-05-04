using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Turing
{
    [Serializable]
    public class TapeItem : INotifyPropertyChanged
    {
        private string _color;
        private int _index;

        private bool _isSelected;

        private char _letter;

        public TapeItem(int index, char sym = ' ')
        {
            _index = index;
            Letter = sym;
        }

        public bool IsSelected
        {
            set
            {
                if (value)
                    Color = "#ffff66"; // желтый
                else
                    Color = null; // белый
                _isSelected = value;
                OnPropertyChanged();
            }
            get => _isSelected;
        }

        public string Color
        {
            private set
            {
                _color = value;
                OnPropertyChanged();
            }

            get => _color;
        }

        public char Letter
        {
            get => _letter;
            set
            {
                _letter = value;
                OnPropertyChanged();
            }
        }

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
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
}
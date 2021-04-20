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
        private string color;
        public string Color
        {
            get => color;
            set
            {
                color = value;
                OnPropertyChanged();
            }
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

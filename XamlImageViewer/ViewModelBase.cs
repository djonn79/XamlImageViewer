﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XamlImageViewer
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {      

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
            catch (Exception err)
            {
                MessageBox.Show(err?.InnerException.Message ?? err.Message, err.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool EnableEditMode { get; set; } = true;
    }
}

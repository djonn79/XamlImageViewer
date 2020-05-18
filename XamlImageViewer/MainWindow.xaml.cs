using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace XamlImageViewer
{

    public class MainWindow
    {       
        
        public MainWindow(MainWindowVM vm)
        {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}

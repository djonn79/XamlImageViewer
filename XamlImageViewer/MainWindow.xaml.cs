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

    public class MainWindowVM : ViewModelBase
    {       
        
        public MainWindow(MainWindowVM vm)
        {
            this.DataContext = vm;
            InitializeComponent();
        }
        

        private bool IsWhiteTheme { get; set; } = true;
        public string SelectedFolder { get; set; }
        public Viewbox MainImage { get; set; }
        public List<ListBoxItem> ImagesList { get; set; } = new List<ListBoxItem>();
        public Brush Theme => (IsWhiteTheme) ? Brushes.White : Brushes.DarkGray;
        private RelayCommand changeTheme;
        public RelayCommand ChangeTheme => changeTheme ?? (
             changeTheme = new RelayCommand((p) => {
                 IsWhiteTheme = !IsWhiteTheme;
                 OnPropertyChanged("Theme");
             }));
        private RelayCommand openFolder;
        public RelayCommand OpenFolder => openFolder ?? (
             openFolder = new RelayCommand((p) => {
                 FolderBrowserDialog fbd = new FolderBrowserDialog();

                 if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                 {
                     SelectedFolder = fbd.SelectedPath;
                     OnPropertyChanged("SelectedFolder");
                     LoadingXamlFiles(SelectedFolder);
                 }
             }));


        #region Select Directory
        private async void LoadingXamlFiles(string folder)
        {
            ImagesList?.Clear();
            var progress = new Progress<double>();
            progress.ProgressChanged += (s, a) =>
            {
                //progressBar.Value = a;
            };
            if (Directory.Exists(folder))
            {
                LoadFilesAsync(progress, folder);
            }
        }

        /*private async void LoadFilesAsync(IProgress<double> progress, string folder)
        {
            await Task.Run(() =>
            {
                progress.Report(5);
                var files = Directory.GetFiles(folder, "*.xaml", SearchOption.AllDirectories)?.Select(x => new FileInfo(x))?.ToList();
                progress.Report(20);
                if (files.Count > 0)
                {
                    double step = 80 / files.Count;
                    double counter = 0;
                    foreach (var file in files)
                    {
                        using (var reader = new StreamReader(file.FullName))
                        {
                            Dispatcher.CurrentDispatcher.Invoke(() =>
                            {
                                Viewbox vb = new Viewbox() { Height = 16, Width = 16 };
                                var ui = XamlReader.Load(reader.BaseStream);
                                vb.Child = ui as UIElement;

                                StackPanel sp = new StackPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal };
                                sp.Children.Add(vb);
                                sp.Children.Add(new TextBlock() { Text = file.Name, Margin = new Thickness(5, 0, 0, 0) });

                                var item = new ListBoxItem() { Content = sp, Tag = file };
                                item.MouseDoubleClick += (s, e) => { if (s is ListBoxItem i) { ReadImage(i.Tag as FileInfo); }; };
                                ImagesList.Add(item);
                                reader.Close();
                            });
                        }
                        counter += step;
                        progress.Report(counter);
                    }
                    progress.Report(0);
                }
            }
            );
        } */

        private void LoadFilesAsync(IProgress<double> progress, string folder)
        {
            //await Task.Run(() =>
            {
                progress.Report(5);
                var files = Directory.GetFiles(folder, "*.xaml", SearchOption.AllDirectories)?.Select(x => new FileInfo(x))?.ToList();
                progress.Report(20);
                if (files.Count > 0)
                {
                    double step = 80 / files.Count;
                    double counter = 0;
                    foreach (var file in files)
                    {
                        using (var reader = new StreamReader(file.FullName))
                        {
                            Dispatcher.CurrentDispatcher.Invoke(() =>
                            {
                                Viewbox vb = new Viewbox() { Height = 16, Width = 16 };
                                var ui = XamlReader.Load(reader.BaseStream);
                                vb.Child = ui as UIElement;

                                StackPanel sp = new StackPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal };
                                sp.Children.Add(vb);
                                sp.Children.Add(new TextBlock() { Text = file.Name, Margin = new Thickness(5, 0, 0, 0) });

                                var item = new ListBoxItem() { Content = sp, Tag = file };
                                item.MouseDoubleClick += (s, e) => { if (s is ListBoxItem i) { ReadImage(i.Tag as FileInfo); }; };
                                ImagesList.Add(item);
                                reader.Close();
                            });
                        }
                        counter += step;
                        progress.Report(counter);
                    }
                    progress.Report(0);
                }
            }
            //);
        }

        private void ReadImage(FileInfo fi)
        {
            using (var reader = new StreamReader(fi.FullName))
            {
                MainImage.Child = null;
                var ui = XamlReader.Load(reader.BaseStream);
                MainImage.Child = ui as UIElement;
                OnPropertyChanged("MainImage");
            }
        }

        //private void OpenFolder()
        //{
        //    FolderBrowserDialog fbd = new FolderBrowserDialog();

        //    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        SelectedFolder = fbd.SelectedPath;
        //        LoadingXamlFiles(SelectedFolder);
        //    }
        //}

        #endregion


        //private void ReadImage(FileInfo fi)
        //{
        //    using (var reader = new StreamReader(fi.FullName))
        //    {
        //        imageViewBox.Child = null;
        //        var ui = XamlReader.Load(reader.BaseStream);
        //        imageViewBox.Child = ui as UIElement;
        //    }
        //}
        /*  private void xamlFilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
          {
              if (xamlFilesList.SelectedItem is ListBoxItem item)
              {
                  if (item.Tag is FileInfo fi)
                      ReadImage(fi);
              }
          } */

        //private void CopyResource(object sender, RoutedEventArgs e)
        //{
        //    if (xamlFilesList.SelectedItem is ListBoxItem item)
        //    {
        //        if (item.Tag is FileInfo fi)
        //        {
        //            var text = File.ReadAllText(fi.FullName);

        //            text = text.Replace("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"", $"x:Key=\"{fi.Name.Replace(fi.Extension, "")}\"");
        //            System.Windows.Clipboard.SetText(text);
        //        }
        //    }
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    IsWhiteTheme = !IsWhiteTheme;
        //    gridBackground.Background = (IsWhiteTheme) ? Brushes.White : Brushes.DarkGray;
        //}
    }
}

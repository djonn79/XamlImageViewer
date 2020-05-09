using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace XamlImageViewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsWhiteTheme { get; set; } = true;

        public MainWindow()
        {
            InitializeComponent();
        }
        #region Select Directory
        private async void LoadingXamlFiles(string folder)
        {
            xamlFilesList.Items.Clear();
            var progress = new Progress<double>();
            progress.ProgressChanged += (s, a) =>
            {
                progressBar.Value = a;
            };
            if (Directory.Exists(folder))
            {
                LoadFilesAsync(progress, folder);
            }
        }

        private async void LoadFilesAsync(IProgress<double> progress, string folder)
        {
            await Task.Run(() =>
            {
                var files = new List<FileInfo>();
                progress.Report(5);
                files = Directory.GetFiles(folder, "*.xaml", SearchOption.AllDirectories)?.Select(x => new FileInfo(x))?.ToList();
                progress.Report(20);
                if (files.Count > 0)
                {
                    double step = 80 / files.Count;
                    double counter = 0;
                    foreach (var file in files)
                    {
                        using (var reader = new StreamReader(file.FullName))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Viewbox vb = new Viewbox() { Height = 16, Width = 16 };
                                var ui = XamlReader.Load(reader.BaseStream);
                                vb.Child = ui as UIElement;

                                StackPanel sp = new StackPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal };
                                sp.Children.Add(vb);
                                sp.Children.Add(new TextBlock() { Text = file.Name, Margin = new Thickness(5, 0, 0, 0) });

                                var item = new ListBoxItem() { Content = sp, Tag = file };
                                item.MouseDoubleClick += (s, e) => { if (s is ListBoxItem i) { ReadImage(i.Tag as FileInfo); }; };
                                xamlFilesList.Items.Add(item);
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
        }

        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBlockFolder.Text = fbd.SelectedPath;
                LoadingXamlFiles(textBlockFolder.Text);
            }
        }

        #endregion


        private void ReadImage(FileInfo fi)
        {
            using (var reader = new StreamReader(fi.FullName))
            {
                imageViewBox.Child = null;
                var ui = XamlReader.Load(reader.BaseStream);
                imageViewBox.Child = ui as UIElement;
            }
        }
        private void xamlFilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xamlFilesList.SelectedItem is ListBoxItem item)
            {
                if (item.Tag is FileInfo fi)
                    ReadImage(fi);
            }
        }

        private void CopyResource(object sender, RoutedEventArgs e)
        {
            if (xamlFilesList.SelectedItem is ListBoxItem item)
            {
                if (item.Tag is FileInfo fi)
                {
                    var text = File.ReadAllText(fi.FullName);

                    text = text.Replace("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"", $"x:Key=\"{fi.Name.Replace(fi.Extension, "")}\"");
                    System.Windows.Clipboard.SetText(text);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsWhiteTheme = !IsWhiteTheme;
            gridBackground.Background = (IsWhiteTheme) ? Brushes.White : Brushes.DarkGray;
        }
    }
}
